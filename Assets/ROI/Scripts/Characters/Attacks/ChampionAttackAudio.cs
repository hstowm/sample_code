using UnityEngine;

namespace ROI
{


    [RequireComponent(typeof(ChampionData))]
    public class ChampionAttackAudio : MonoBehaviour, IOnAttackEvent, IOnAttacked
    {
        [SerializeField]private ChampionData _championData;
        [SerializeField] private AudioClip physicsAttacked, magicAttacked, autoAttack;

        private void Start()
        {
            _championData = gameObject.GetComponent<ChampionData>();
        }


        public void OnStartAlive()
        {
            /*if (!_championData)
            {
                Logs.Error("Champion data is null");
                return;
            }
            _championData.handles.OnAttackEvents.Add(this);
            _championData.handles.OnAttacked.Add(this);*/
        }

        public void OnAttackEvent()
        {
            if (autoAttack != null)
                SoundManager.sfx_basicAtk.PlayOneShot(autoAttack);
        }

        public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
        {
            if (!physicsAttacked || !magicAttacked) return;
            if (damageDealtData.damageSource.IsBasicAttack())
            {
                if (damageDealtData.damageType == DamageTypes.Magic)
                {
                    SoundManager.sfx_basicAtk.PlayOneShot(magicAttacked);
                }
                else if (damageDealtData.damageType == DamageTypes.Physic)
                {
                    SoundManager.sfx_basicAtk.PlayOneShot(physicsAttacked);
                }
                if(_championData.HasUltimateEnergy())
                _championData.AddBonusUltimateEnergy(0.33f);
            }
        }
    }
}