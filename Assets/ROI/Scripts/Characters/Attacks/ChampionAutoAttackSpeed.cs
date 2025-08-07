using System;
using System.Collections;
using UnityEngine;
using Mirror;

namespace ROI
{
    [RequireComponent(typeof(ChampionData))]
    public class ChampionAutoAttackSpeed : NetworkBehaviour, IOnStartAutoAttack, IOnStopAutoAttack
    {
        private ChampionData _championData;

        //  private float _delayNextAutoAttack;
        private bool _isAutoAttack;
        private Coroutine _autoAttackHandle;

        private IEnumerator _autoAttackEnumerator;

        private float _delayNextAutoAttack;

#if UNITY_EDITOR
        private int _attackAnimID;
        private DateTime _startTime;
        private Coroutine _coroutine;

        [SerializeField] private bool _enableTest;
#endif

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();

#if UNITY_EDITOR
            _attackAnimID = Animator.StringToHash(_championData.attackAnim.animName);
#endif
        }

        // [Command(requiresAuthority = false)]
        public void OnStartAutoAttack()
        {
            if (isServer == false)
                return;

            CalculateAnimSpeed();

            _autoAttackEnumerator = RunAutoAttackTick();
            SetAutoAttack(true);
        }

        public void OnStopAutoAttack()
        {
            if (isServer == false)
                return;

            SetAutoAttack(false);
        }
        
        public void OnAttackSpeedChanged() 
        { 
            if(_championData.state != ChampionStates.Attacking)
                return;

            CalculateAnimSpeed();

//            _autoAttackEnumerator = RunAutoAttackTick();
        }

        private void CalculateAnimSpeed()
        {
            if (false == isServer)
                return;

            // calculate total time per attack of anim (second / attack)
            var t = _championData.attackAnim.animFrames / _championData.attackAnim.animSpeed;

            // convert from sec/ attack to attack / sec
            var v0 = 1f / t;

            // champion attack / sec
            var v1 = _championData.attackData.speed;
            var v = v1 - v0;

            // v1 == v0: current speed == anim speed ==> dont scale anim speed
            // v1 > v0: current speed > anim speed ==> need scale up animation speed
            // v1 < v0: current speed == anim speed and wait for next auto attack trigger
            var speedMultiplier = v <= 0 ? 1 : v1 / v0;

            //_delayNextAutoAttack = v < 0.01f ? 1f / v1 : 0;
            _delayNextAutoAttack =  1f / v1 ;

            _championData.animatorNetwork.animator.SetFloat(AnimHashIDs.AttackSpeed, speedMultiplier);
        }

        public virtual void SetAutoAttack(bool isAutoAttack)
        {
            if (!isServer)
                return;

            _isAutoAttack = isAutoAttack;
            _championData.animatorNetwork.animator.SetBool(AnimHashIDs.IsAutoAttack, false);

            if (_autoAttackHandle != null)
            {
                StopCoroutine(_autoAttackHandle);
                _autoAttackHandle = null;
            }

            if (_isAutoAttack == false)
            {
                return;
            }

            _championData.animatorNetwork.animator.SetBool(AnimHashIDs.IsAutoAttack, _delayNextAutoAttack <= 0);
            _championData.animatorNetwork.SetTrigger(AnimHashIDs.Attack);

            if (_delayNextAutoAttack > 0.01f)
                _autoAttackHandle = StartCoroutine(_autoAttackEnumerator);
        }

        IEnumerator RunAutoAttackTick()
        {
            // Logs.Info($"_delayNextAutoAttack: {_delayNextAutoAttack}.");
            while (_isAutoAttack)
            {
                yield return new WaitForSeconds(_delayNextAutoAttack);
                
                if(_championData.state != ChampionStates.Attacking)
                    yield break;
                
                _championData.animatorNetwork.SetTrigger(AnimHashIDs.Attack);
            }
        }

        private void OnDisable()
        {
            _isAutoAttack = false;
            if (_autoAttackHandle != null)
            {
                StopCoroutine(_autoAttackHandle);
                _autoAttackHandle = null;
            }
        }


#if UNITY_EDITOR

        private IEnumerator StartTestAutoAttackTime()
        {
            // on transition
            _startTime = DateTime.Now;

            Debug.Log($"{gameObject.name}. Start Transition..");

            while (_championData.animatorNetwork.animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _attackAnimID)
                yield return null;

            Debug.Log($"{gameObject.name}.Transition Time. {(DateTime.Now - _startTime).TotalSeconds}");

            _startTime = DateTime.Now;
        }

        private void EndTestAutoAttackTime()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            var state = _championData.animatorNetwork.animator.GetCurrentAnimatorStateInfo(0);
            if (state.shortNameHash != _attackAnimID)
            {
                Debug.LogError("EndTest Without Attack Anim State");
                return;
            }

            var animNormalizedTime = state.normalizedTime;
            var totalAttackTime = (DateTime.Now - _startTime).TotalSeconds;

            Debug.LogWarning(
                $"{gameObject.name}. Animation Normalize Time: {animNormalizedTime}. Attack Time: {totalAttackTime}. Speed: {_championData.attackData.speed}");
        }
#endif
    }
}