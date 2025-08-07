using UnityEngine;

namespace ROI.Scripts.AbilityCards.Passives
{
    
    /// <summary>
    /// My critical strikes deal an additional 25% damage.
    /// </summary>
    
    [CreateAssetMenu(fileName = "FatalPrecision", menuName = "ROI/Data/AbilityPassiveCards/FatalPrecision", order = 1)]
    public class FatalPrecisionPassive : BasePassiveAbilityCard
    {

        public override void OnInit(ChampionData champion)
        {
            // var gainCritDamage = new GainCritFinalDamage(champion);
            champion.handles.OnHitEnemies.Add( new GainCritFinalDamage());
        }
    }
    
    class GainCritFinalDamage : IOnHitEnemy//, IOnFinalDamage
    {
      //  public ChampionData championData;
       // public ChampionData enemy;

        // public GainCritFinalDamage(ChampionData championData)
        // {
        //     this.championData = championData;
        // }


        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
           // this.enemy = enemy;
            // damageDealtData.hooks.OnFinalDamages.Add(this);
            damageDealtData.critDamageBonus += 0.25f;
        }

        // public void OnDamageCalculated(DamageDealtData damageDealtData)
        // {
        //     Debug.Log("damage crit: "  +damageDealtData.isCrit);
        //     if (damageDealtData.isCrit)
        //     {
        //         Debug.Log("add damage");
        //         damageDealtData.AddBonusDamage(0.25f, StatValueType.Percent);
        //     }
        // }
    }
    
    
    
}
