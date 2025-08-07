using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// My abilities heal me for 10% of the damage dealt
    /// </summary>
    
    
    [CreateAssetMenu(fileName = "VampiricSpellcraft", menuName = "ROI/Data/AbilityPassiveCards/VampiricSpellcraft")]
    public class VampiricSpellcraft : BasePassiveAbilityCard
    {
        private ChampionDamageText _championDamageText;
        public override void OnInit(ChampionData champion)
        {
            _championDamageText = ChampionDamageText.instance;
            champion.handles.OnHitEnemies.Add(new VampiricSpellcraftInject(champion,this));
        }


        public class VampiricSpellcraftInject : IOnHitEnemy
        {
            private ChampionData _championData;
            private VampiricSpellcraft _vampiricSpellcraft;
            
            public VampiricSpellcraftInject(ChampionData championData, VampiricSpellcraft vampiricSpellcraft)
            {
                _championData = championData;
                _vampiricSpellcraft = vampiricSpellcraft;
            }
            
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                if (damageDealtData.damageSource.IsAbility())
                {
                    var health = damageDealtData.attackDamage * 0.1f;
                    Debug.Log("damage: " + health);
                    _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.Health,
                        health));
                    _vampiricSpellcraft._championDamageText.ShowHealDamage(_championData, (int)health);
                }
            }
        }

      
    }
}


