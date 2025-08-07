using System.Collections.Generic;
using UnityEngine;
namespace ROI
{
    public interface ISkillCardTrigger 
    {
        public void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer);
    }
}
