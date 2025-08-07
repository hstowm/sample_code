using System.Collections;
using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    /// <summary>
    /// Activating my ultimate ability increases all damage dealt
    /// from my abilities and attacks by 25% for 10 seconds
    /// </summary>
    
    [CreateAssetMenu(fileName = "VoidMeld", menuName = "ROI/Data/AbilityPassiveCards/VoidMeld")]
    public class VoidMeld : BasePassiveAbilityCard
    {
       
        
        public override void OnInit(ChampionData champion)
        {
            var voidMeid = new VoidMeldInject(champion);
            champion.handles.OnUseCards.Add(voidMeid);
            champion.handles.OnHitEnemies.Add(voidMeid);
        }


        class VoidMeldInject : IOnUseCard, IOnHitEnemy
        {
            
            private ChampionData _championData;
            public int gainDuration = 10;
            public bool gainAttack = false;

            public VoidMeldInject(ChampionData championData)
            {
                _championData = championData;
                gainAttack = false;
                gainDuration = 10;
            }
            
            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (cardSkillType.cardSkillType == CardSkillType.ChampionUltimate)
                {
                    if (!gainAttack)
                    {
                        gainAttack = true;
                        _championData.StartCoroutine(CountGainTime());

                    }
                }
            }

            private IEnumerator CountGainTime()
            {
                yield return new WaitForSeconds(gainDuration);
                gainAttack = false;
            }

            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                if (gainAttack)
                {
                    damageDealtData.AddBonusDamage(0.25f,StatValueType.Percent);
                }
            }
        }

        
    }
}


