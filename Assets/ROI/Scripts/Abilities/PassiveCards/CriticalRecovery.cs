using System.Collections;
using UnityEngine;

namespace ROI
{
    //When I crit or am crit, heal allies (including self) 10% their max health. (triggers only once every 30 seconds)
    [CreateAssetMenu(fileName = "CriticalRecovery", menuName = "ROI/Data/AbilityPassiveCards/CriticalRecovery", order = 1)]
    
    public class CriticalRecovery : BasePassiveAbilityCard, IAbilityCard
    {
        [SerializeField]private float cooldownTime = 30;
        public StatusSetting healHpSetting;
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnAttacked.Add(new RecoveryHealOnFinalCrit(champion, cooldownTime, healHpSetting));
        }
        private class RecoveryHealOnFinalCrit : IOnAttacked, IOnHitEnemy, IOnFinalDamage
        {
            private ChampionData _champion;
            private float _cooldownTime;
            private bool _canApplied = true;
            private StatusSetting healHpSetting;
            public RecoveryHealOnFinalCrit(ChampionData championData, float cooldownTime, StatusSetting healHpSetting)
            {
                _champion = championData;
                _cooldownTime = cooldownTime;
                this.healHpSetting = healHpSetting;
                championData.handles.OnHitEnemies.Add(this);
            }
            
            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                damageDealtData.hooks.OnFinalDamages.Add(this);

            }
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                damageDealtData.hooks.OnFinalDamages.Add(this);
            }
            IEnumerator RecoveryHealth()
            {
                foreach (var ally in _champion.allies)
                {
                    HealthChampion(ally);
                }
                yield return new WaitForSeconds(_cooldownTime);
                _canApplied = true;
            }
            private void HealthChampion(ChampionData championData)
            {
                GeneralEffectSystem.Instance.ApplyEffect(championData, new StatusData(healHpSetting, _champion, new Vector3()));
            }

            public void OnDamageCalculated(DamageDealtData damageDealtData)
            {
                if (!damageDealtData.isCrit) return;
                if (_canApplied)
                {
                    _canApplied = false;
                    _champion.StartCoroutine(RecoveryHealth());
                }
            }
        }
        
    }
}

