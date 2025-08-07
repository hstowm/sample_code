using System.Collections.Generic;
using System.Linq;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    
    /// <summary>
    /// When I play a card that costs 5 or more,
    /// heal your lowest health character by 10% based on max health.
    /// </summary>
    
    [CreateAssetMenu(fileName = "EmpoweringExpenditure", menuName = "ROI/Data/AbilityPassiveCards/EmpoweringExpenditure")]

    public class EmpoweringExpenditure : BasePassiveAbilityCard, IOnUseCard
    {
        private ChampionData _championData;
        private ChampionDamageText _championDamageText;
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            _championDamageText = ChampionDamageText.instance;
            champion.handles.OnUseCards.Add(this);
        }

        public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
        {
            if (cardSkillType.cardSkillType is CardSkillType.AbilityActive or CardSkillType.ChampionActive)
            {
                if (cardSkillType.manaCost >= 5)
                {
                    var lstAlly = _championData.allies.OrderBy(champ => champ.healthData.health).ToList();
                    var championLowestHealth = lstAlly[0];
                    
                    var health = championLowestHealth.healthData.maxHealth * 0.1f;
                    championLowestHealth.statModifier.ApplyModify(new StatTypeData(StatTypes.Health,
                        health));
                    _championDamageText.ShowHealDamage(championLowestHealth, (int)health);
                    
                }
                
                
            }
        }
    }
}


