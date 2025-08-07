using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// 25% chance to dodge auto attacks
    /// </summary>
    
    [CreateAssetMenu(fileName = "ElusiveReflexes", menuName = "ROI/Data/AbilityPassiveCards/ElusiveReflexes")]
    public class ElusiveReflexes : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnAttacked.Add(new ElusiveReflexesInject(champion));
        }

        
        public class ElusiveReflexesInject : IOnAttacked
        {
            private ChampionData _championData;
            
            public ElusiveReflexesInject(ChampionData championData)
            {
                _championData = championData;
            }
            
            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                if (damageDealtData.damageSource.IsBasicAttack())
                {
                    damageDealtData.dodgeChancePercent += 0.25f;
                }
            }
        }

     
    }
}


