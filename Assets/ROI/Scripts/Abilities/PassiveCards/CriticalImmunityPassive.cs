using UnityEngine;

namespace ROI
{
    //Cannot be critical striked
    [CreateAssetMenu(fileName = "CriticalImmunityPassive", menuName = "ROI/Data/AbilityPassiveCards/CriticalImmunity")]
    public class CriticalImmunityPassive : BasePassiveAbilityCard, IAbilityCard
    {
        //[Server]
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnAttacked.Add(new CriticalImmunityInject(champion));
        }

        class CriticalImmunityInject : IOnAttacked
        {
            public ChampionData championData;
            public CriticalImmunityInject(ChampionData championData)
            {
                this.championData = championData;
            }

            //chua check duoc vi khong sua duoc healthReduction
            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                // cant be crit
                damageDealtData.canCrit = false;
                //
                // if (damageDealtData.isCit)
                // {
                //     damageDealtData.healthReduction = 
                //         DamageDealtCalculator.CalcHeathReduction(championData,
                //             damageDealtData.attackDamage - damageDealtData.critDamagePercent,
                //             damageDealtData.damageType);
                // }
            }
        }
    }
}
