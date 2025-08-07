using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    public abstract class BasePassiveAbilityCard : ScriptableObject, IAbilityCard
    {
        public CardSkillData cardSkillData;
        public abstract void OnInit(ChampionData champion);
    }

}
