using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "VenomousDrain", menuName = "ROI/Data/AbilityPassiveCards/VenomousDrain", order = 1)]
    public class VenomousDrain : BasePassiveAbilityCard
    {
        public float lifeStealBonus;
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new AddLifeStealOnAttackPoison(champion, lifeStealBonus));

        }
        private class AddLifeStealOnAttackPoison : IOnHitEnemy
        {
            private ChampionData _championData;
            private float _lifeStealBonus;
            public bool onAdded = false;
            public AddLifeStealOnAttackPoison(ChampionData championData, float lifeStealBonus)
            {
                _championData = championData;
                _lifeStealBonus = lifeStealBonus;
            }
            

            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                // Add status on hit
                if (damageDealtData.damageSource.IsBasicAttack() && enemy.currentEffect.HasEffect(ChampionEffects.Poisoned))// Check target is poisoned
                {
                    if (!onAdded)
                    {
                        _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.LifeSteal, _lifeStealBonus, StatValueTypes.Percent));
                        onAdded = true;
                    }
                }
                else
                {
                    if (onAdded)
                    {
                        _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.LifeSteal, -_lifeStealBonus, StatValueTypes.Percent));
                        onAdded = false;
                    }
                }
            }
        }
    }

}

