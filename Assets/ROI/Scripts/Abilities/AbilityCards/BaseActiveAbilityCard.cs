using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    public abstract class BaseActiveAbilityCard : MonoBehaviour, IAbilityCard, ISkillCardTrigger, IPauseHandle, IOnDead, IOnStartAlive
    {
        public CardSkillData cardSkillData;
        [HideInInspector] public ChampionData _championData;
        protected List<ChampionData> championsEffectBySkill = new List<ChampionData>();
        public Vector3 targetPosition;
        protected List<GameObject> objectsDestroyOnSkillDone = new List<GameObject>();
        [SerializeField] public SkillsPlayer skillsPlayer;
        private bool _isServer;
        private bool isCasting = false;
        
        public virtual void OnInit(ChampionData champion)
        {
            _championData = champion;
            _championData.handles.OnDeads.Add(this);
            skillsPlayer.Events.OnPlay.AddListener(() =>
            {
                _championData.controller.Pause(this);
            });
            skillsPlayer.Events.OnComplete.AddListener(CancelSkill);
            _championData.handles.OnStartAlive.Add(this);
        }

        // public virtual void InitObjectPool( IObjectPool objectPool)
        // {
        //     skillsPlayer.InitObjectPool(objectPool);
        // }
        
        public virtual void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            targetPosition = new Vector3(inputPosition.x, inputPosition.y, inputPosition.z);
            _isServer = isServer;
            isCasting = true;

            championsEffectBySkill = targets;
            _championData.animatorNetwork.animator.fireEvents = false;
        }
        protected virtual void CancelSkill()
        {
            Debug.Log($"Stop skill {cardSkillData.KeyName}");
            isCasting = false;
            foreach (var obj in objectsDestroyOnSkillDone)
            {
                Destroy(obj);
            }
            objectsDestroyOnSkillDone.Clear();
            IsPaused = false;
            skillsPlayer.StopFeedbacks();
            _championData.animatorNetwork.animator.fireEvents = true;
        }

        public void CheckApplyCC(StatusData statusData)
        {
            if(!isCasting) return;
            if (statusData.type == StatusData.EffectType.DeBuff)
            {
                Debug.Log("champion apply cc");
                if (_championData.currentEffect.IsCC())
                {
                    Debug.Log($"stop skill {cardSkillData.name} because effect by effect {statusData.key_name}");
                    HandleOnApplyCC();
                }
            }
        }
        
        public bool IsPaused { get; set; }
        public virtual void OnDead()
        {
            CancelSkill();
            
            gameObject.Recycle();
        }

        protected virtual void HandleOnApplyCC()
        {
            
        }

        public void OnStartAlive()
        { 
            if(_isServer == false)
                return;
            
            // Debug.Log("register skill on champion alive");
            if(GeneralEffectSystem.Instance.applyEffectActions.TryGetValue(_championData.netId,out _) == false )
                GeneralEffectSystem.Instance.applyEffectActions.Add(_championData.netId, data => {});
            
            GeneralEffectSystem.Instance.applyEffectActions[_championData.netId] += CheckApplyCC;
        }
    }
}
