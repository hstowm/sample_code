using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{

    /// <summary>
    /// When I activate my ultimate, a random card in my hand's mana cost is reduced by half until played (rounded up)
    /// </summary>
    [CreateAssetMenu(fileName = "UltimateMomentum", menuName = "ROI/Data/AbilityPassiveCards/UltimateMomentum")]

    public class UltimateMomentum : BasePassiveAbilityCard
    {
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnUseCards.Add(new ReduceCardsManaOnPlayUltimate(champion));
        }

        public class ReduceCardsManaOnPlayUltimate : IOnUseCard
        {
            private ChampionData _championData;

            public ReduceCardsManaOnPlayUltimate(ChampionData championData)
            {
                _championData = championData;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (cardSkillType.cardSkillType == CardSkillType.ChampionUltimate)
                {
                    int randomCardIndex = Random.Range(0, CardPoolManager.Instance.cardUsing.Count);
                    Debug.Log($"RandomIndex: {randomCardIndex}");
                    SkillCard cardReduceMana = CardPoolManager.Instance.cardUsing[randomCardIndex];
                    ChampionData owner = cardReduceMana.championData;
                    int manaReduce = (int)(cardReduceMana.cardSkillData.manaCost) / 2;
                    owner.AddManaCostBonus(cardReduceMana.cardSkillData.KeyName, -manaReduce);
                    owner.handles.OnUseCards.Add(new AddManaOnCardOnUse(owner, cardReduceMana, manaReduce));
                }

            }
        }

        public class AddManaOnCardOnUse : IOnUseCard
        {
            private ChampionData _championData;
            private SkillCard skillCard;
            private int manaInCrease;

            public AddManaOnCardOnUse(ChampionData championData, SkillCard skillCard, int manaInCrease)
            {
                _championData = championData;
                this.skillCard = skillCard;
                this.manaInCrease = manaInCrease;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (cardSkillType.KeyName == skillCard.cardSkillData.KeyName)
                {
                    _championData.AddManaCostBonus(skillCard.cardSkillData.KeyName, manaInCrease);
                    _championData.handles.OnUseCards.Remove(this);
                }
            }
        }


    }
}