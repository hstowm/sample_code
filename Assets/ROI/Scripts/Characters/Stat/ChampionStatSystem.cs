using UnityEngine;
namespace ROI
{

	[RequireComponent(typeof(ChampionDamageText))]
	public class ChampionStatSystem : MonoBehaviour
	{
		private ChampionDamageText _championDamageText;

		private void Awake()
		{
			_championDamageText = GetComponent<ChampionDamageText>();
		}

		public void InitHiddenStat(ChampionData championData)
		{
			// init ch
			var lifeSteal = new ChampionLifeSteal(championData, _championDamageText);
		}
	}
}