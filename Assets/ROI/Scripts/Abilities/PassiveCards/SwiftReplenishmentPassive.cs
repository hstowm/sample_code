using System.Collections.Generic;
using UnityEngine;
using ROI.DataEntity;

namespace ROI
{
    
    /// <summary>
    /// Whenever an ability with cost 3 or less is casted, increase my Ultimate Energy by an additional 1
    /// </summary>
    
    [CreateAssetMenu(fileName = "SwiftReplenishmentPassive",
        menuName = "ROI/Data/AbilityPassiveCards/SwiftReplenishment")]
    public class SwiftReplenishmentPassive : BasePassiveAbilityCard, IAbilityCard
    {
        public override void OnInit(ChampionData champion)
        {
            SwiftReplenishmentInject swiftReplenishmentInject = new SwiftReplenishmentInject(champion);
            champion.handles.OnUseCards.Add(swiftReplenishmentInject);
        }

        class SwiftReplenishmentInject : IOnUseCard
        {
            ChampionData championData;
            public SwiftReplenishmentInject(ChampionData championData)
            {
                this.championData = championData;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (!isServerSide) return;
                if(cardSkillType.manaCost >= 3)
                {
                    championData.AddBonusUltimateEnergy(1);
                    Debug.Log("ultimate: " + championData.GetUltimateEnergy());;
                }
            }
        }
    }


}
