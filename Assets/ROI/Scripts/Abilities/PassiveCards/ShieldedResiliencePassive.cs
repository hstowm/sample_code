using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "ShieldedResiliencePassive",
        menuName = "ROI/Data/AbilityPassiveCards/ShieldedResilience")]
    public class ShieldedResiliencePassive : BasePassiveAbilityCard, IAbilityCard
    {
        public int numberHitBlock;

        public override void OnInit(ChampionData champion)
        {
            var handle = new ShieldedResilienceInject(champion, numberHitBlock);
            champion.handles.OnAttacked.Remove(handle);
            champion.handles.OnAttacked.Add(handle);
            
        }

        class ShieldedResilienceInject : IOnAttacked
        {
            int numberHitBlock;
            int countHit;
            private ChampionData _championData;


            public ShieldedResilienceInject(ChampionData championData, int numberHitBlock)
            {
                this.numberHitBlock = numberHitBlock;
                countHit = 0;

                _championData = championData;
            }

            //chua check duoc vi khong sua duoc healthReduction
            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                if(damageDealtData.damageSource != DamageSources.BasicAttack) 
                    return;
                
                if (countHit < numberHitBlock)
                {
                    countHit++;
                    return;
                }

                countHit = 0;
                damageDealtData.dodgeChancePercent = 1;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                return obj is ShieldedResilienceInject shielded && shielded._championData.Equals(_championData);
            }
        }
    }
}