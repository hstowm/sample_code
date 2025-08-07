using UnityEngine;


namespace ROI
{
    /// <summary>
    /// Gain 5% ability power
    /// </summary>
    
    [CreateAssetMenu(fileName = "EnhancedProtection", menuName = "ROI/Data/AbilityPassiveCards/EnhancedProtection", order = 1)]
    public class EnhancedProtection : BasePassiveAbilityCard, IOnHitEnemy
    {
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(this);
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if (damageDealtData.damageSource.IsAbility())
            {
                damageDealtData.abilityPower += damageDealtData.abilityPower * 0.05f;
            }
        }
    }
}


