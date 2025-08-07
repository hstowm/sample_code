using System.Collections.Generic;
using UnityEngine;
using ROI.DataEntity;

namespace ROI
{
    
    /// <summary>
    /// After I use an ultimate, gain 25% ultimate energy.
    /// </summary>
    
    
    [CreateAssetMenu(fileName = "NimbleStrikePassive", menuName = "ROI/Data/AbilityPassiveCards/NimbleStrike")]
    public class NimbleStrikePassive : BasePassiveAbilityCard, IAbilityCard
    {
        public float percentAddUltimateEnergy = 0.25f; // bao nhieu % cua max energy
        
        public override void OnInit(ChampionData champion)
        {
            NimbleStrikeInject nimbleStrikeInject = new NimbleStrikeInject(champion, percentAddUltimateEnergy);
            champion.handles.OnUseCards.Add(nimbleStrikeInject);
        }

        class NimbleStrikeInject : IOnUseCard
        {
            ChampionData championData;
            float percentAddUltimateEnergy;
            public NimbleStrikeInject (ChampionData championData, float percentAddUltimateEnergy)
            {
                this.championData = championData;
                this.percentAddUltimateEnergy = percentAddUltimateEnergy;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (cardSkillType.cardSkillType == CardSkillType.ChampionUltimate)
                {
                    championData.AddBonusUltimateEnergy(Mathf.RoundToInt(championData.GetMaxUltimateEnergy() * percentAddUltimateEnergy));
                }
            }
        }
    }
}
