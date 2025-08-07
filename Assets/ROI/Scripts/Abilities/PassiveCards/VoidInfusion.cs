using UnityEngine;



namespace ROI
{
    
    /// <summary>
    /// When I Crit, apply Vulnerable if the target doesn't already have it.
    /// </summary>
    
    [CreateAssetMenu(fileName = "VoidInfusion", menuName = "ROI/Data/AbilityPassiveCards/VoidInfusion")]
    public class VoidInfusion : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new VoidInfusionInject(champion));
        }

        class VoidInfusionInject : IOnHitEnemy, IOnFinalDamage
        {
            
            private ChampionData _championData;
            private ChampionData enemy;
            
            public VoidInfusionInject(ChampionData championData)
            {
                _championData = championData;
            }
            
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                damageDealtData.hooks.OnFinalDamages.Add(this);
                this.enemy = enemy;
            }

            public void OnDamageCalculated(DamageDealtData damageDealtData)
            {
                if (enemy.currentEffect.HasEffect(ChampionEffects.Vulnerable)) return;
                if (damageDealtData.isCrit)
                {
                    GeneralEffectSystem.Instance.ApplyEffect(enemy, new StatusData("Vulnerable", _championData, Vector3.zero));
                }
            }
        }

       
    }

}

