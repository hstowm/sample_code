using UnityEngine;

namespace ROI
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "RuthlessDecimation", menuName = "ROI/Data/AbilityPassiveCards/RuthlessDecimation", order = 1)]
    public class RuthlessDecimationPassive : BasePassiveAbilityCard, IAbilityCard
    {
        public override void OnInit(ChampionData championData)
        {
            championData.handles.OnHitEnemies.Add(new GainAttackDamage(championData));
        }
    }

    class GainAttackDamage : IOnHitEnemy
    {
        private ChampionData championData;

        public GainAttackDamage(ChampionData championData)
        {
            this.championData = championData;
        }
        
        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if (damageDealtData.damageSource.IsBasicAttack() == false) 
                return;
         
            var gainDamage = enemy.healthData.maxHealth * 0.02f;
            damageDealtData.AddBonusDamage(gainDamage);
            
        }
    }
    
}
