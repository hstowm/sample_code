using ROI;
using ROI.DataEntity;
using UnityEngine;
[CreateAssetMenu(fileName = "CryomancerStrike", menuName = "ROI/Data/AbilityPassiveCards/CryomancerStrike", order = 1)]
public class CryomancerStrike : BasePassiveAbilityCard
{
    //My attacks and abilities deal 10% more damage to chilled or frozen enemies
    [SerializeField] private float damageBonus;

    public override void OnInit(ChampionData champion)
    {
        champion.handles.OnHitEnemies.Add(new AddDamageOnChampionChill(damageBonus));
    }

    private class AddDamageOnChampionChill : IOnHitEnemy {
        private float _damageBonus;
        public AddDamageOnChampionChill(float damageBonus)
        {
            this._damageBonus = damageBonus;
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if (damageDealtData.damageSource.IsAbility() || damageDealtData.damageSource.IsBasicAttack())
            {
                if (enemy.currentEffect.HasEffect(ChampionEffects.Chilled) || enemy.currentEffect.HasEffect(ChampionEffects.Frozen) )
                {
                    damageDealtData.AddBonusDamage(0.1f,StatValueType.Percent);
                }
            }

        }
    }
}
