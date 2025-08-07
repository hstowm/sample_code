using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    //Poisoned enemies deal 10% less damage to me.
    [CreateAssetMenu(fileName = "CorrosiveInfusion", menuName = "ROI/Data/AbilityPassiveCards/CorrosiveInfusion", order = 1)]
    public class CorrosiveInfusion : BasePassiveAbilityCard
    {
        [SerializeField]private float damageReduce = 0.1f;
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnAttacked.Add(new ReduceDamageFromPoisonedEnemy(damageReduce));
        }
        
        private class ReduceDamageFromPoisonedEnemy : IOnAttacked
        {
            private float _damageReduce;
            public ReduceDamageFromPoisonedEnemy(float damageReduce)
            {
                _damageReduce = damageReduce;
            }
            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                // Poisoned enemies deal 10% less damage to me.
                if (attacker.currentEffect.HasEffect(ChampionEffects.Poisoned))
                {
                    damageDealtData.AddBonusDamage(-_damageReduce, StatValueType.Percent);
                }
            }
        }
    }
}

