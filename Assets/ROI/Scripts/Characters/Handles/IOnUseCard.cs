using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;
namespace ROI
{
	public interface IOnUseCard
	{
		void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide);
	}
}