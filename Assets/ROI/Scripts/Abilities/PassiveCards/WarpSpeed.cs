using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    /// <summary>
    /// I have +10% attack damage.
    /// </summary>
    
    [CreateAssetMenu(fileName = "WarpSpeed", menuName = "ROI/Data/AbilityPassiveCards/WarpSpeed", order = 1)]
    public class WarpSpeed : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new WarpSpeedInject());
        }

        class WarpSpeedInject : IOnHitEnemy 
        {
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                damageDealtData.AddBonusDamage(0.1f, StatValueType.Percent);
            }
        }

      
    }
}


