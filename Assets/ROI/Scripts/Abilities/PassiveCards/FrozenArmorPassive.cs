using UnityEngine;

namespace ROI
{
    /// <summary>
    /// When hit by an auto-attack, 5% chance to apply Chill to the enemy
    /// </summary>
    /// 
    [CreateAssetMenu(fileName = "FrozenArmorPassive", menuName = "ROI/Data/AbilityPassiveCards/FrozenArmor")]
    public class FrozenArmorPassive : BasePassiveAbilityCard
    {
        public float percentActive = 0.05f;
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnDamageds.Add(new FrozenArmorInject(champion,percentActive));
        }


        class FrozenArmorInject : IOnDamaged
        {
            private ChampionData _championData;
            private float _percentActive;
            
            public FrozenArmorInject(ChampionData championData, float percentActive)
            {
                _championData = championData;
                _percentActive = percentActive;
            }
            
            public void OnDamaged(ChampionData attacker, DamageDealtData damageDealtData)
            {
                if (damageDealtData.damageSource.IsBasicAttack())
                {
                    var active = Random.RandomRange(0, 100) < _percentActive * 100;
                    if (active)
                    {
                        GeneralEffectSystem.Instance.ApplyEffect(attacker, new StatusData("Chilled", _championData, Vector3.zero));
                    }
                }
            }
        }

        
    }
}
