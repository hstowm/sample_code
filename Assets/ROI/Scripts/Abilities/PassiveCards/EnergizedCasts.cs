using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
/// <summary>
/// When I activate my ultimate, a random card in my hand's mana cost is reduced by half until played (rounded up)
/// </summary>
[CreateAssetMenu(fileName = "EnergizedCasts", menuName = "ROI/Data/AbilityPassiveCards/EnergizedCasts", order = 1)]
    public class EnergizedCasts : BasePassiveAbilityCard
    {
        public int cardMana = 5;
        public int manaBonus = 1;
        public ManaManager manager;
        public override void OnInit(ChampionData champion)
        {
            if (PlayerNetwork.Instance.isServer)
            {
                manager = FindObjectOfType<ManaManager>();
                champion.handles.OnUseCards.Add(new BonusManaOnUseCard(champion, cardMana, manaBonus, manager));
            }
        }
        public class BonusManaOnUseCard:IOnUseCard
        {
            private int cardMana;
            private int manaBonus;
            private ChampionData _championData;
            private ManaManager _manaManager;
            public BonusManaOnUseCard(ChampionData championData, int manaCost, int manaBonus, ManaManager manaManager)
            {
                _championData = championData;
                cardMana = manaCost;
                this.manaBonus = manaBonus;
                _manaManager = manaManager;
            }
            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (cardSkillType.manaCost >= cardMana)
                {
                    if(_manaManager)
                        _manaManager.AddMana(_championData.creatorNetId,manaBonus);
                    else
                    {
                        _manaManager = FindObjectOfType<ManaManager>();
                    }
                }
            }
        }
    }
}