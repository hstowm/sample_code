using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// Gain 25% armor penetration
    /// </summary>
    
    [CreateAssetMenu(fileName = "PenetratingShot", menuName = "ROI/Data/AbilityPassiveCards/PenetratingShot")]
    public class PenetratingShot : BasePassiveAbilityCard
    {
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new PenetratingShotInject());
        }

        
        public class PenetratingShotInject : IOnHitEnemy
        {
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                damageDealtData.armorPenetration += damageDealtData.armorPenetration * 0.25f;
            }
        }

       
    }

}

