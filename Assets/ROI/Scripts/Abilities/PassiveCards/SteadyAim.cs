using ROI.DataEntity;
using UnityEngine;



namespace ROI
{
    
    /// <summary>
    /// Increase attack damage by 10%.
    /// </summary>
    
    
    [CreateAssetMenu(fileName = "SteadyAim", menuName = "ROI/Data/AbilityPassiveCards/SteadyAim")]
    public class SteadyAim : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new SteadyAimInject(champion));
        }



        class SteadyAimInject : IOnHitEnemy
        {
            private ChampionData _championData;
            
            public SteadyAimInject(ChampionData championData)
            {
                this._championData = championData;
            }
            
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                damageDealtData.AddBonusDamage(0.1f, StatValueType.Percent);
            }
        }
        
    
    }
}


