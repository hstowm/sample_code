using System.Linq;
using ROI.DataEntity;
using ROI;
using UnityEngine;

namespace ROI
{
	/// <summary>
	/// Increase attack damage dealt to tanks by 20%.
	/// </summary>
	
	[CreateAssetMenu(fileName = "InspiringCourage", menuName = "ROI/Data/AbilityPassiveCards/InspiringCourage", order = 1)]
	public class InspiringCouragePassive : BasePassiveAbilityCard, IAbilityCard
	{
		public override void OnInit(ChampionData champion)
		{
			champion.handles.OnHitEnemies.Add(new GainDamageToTank(champion));
		}

	}

	class GainDamageToTank : IOnHitEnemy
	{
		private ChampionData championData;

		public GainDamageToTank(ChampionData championData)
		{
			this.championData = championData;
		}

		public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
		{
			if (enemy.userChampionUID != "")
			{
				var baseID =
					PlayerNetwork.Instance.listOpponentChampions.Find(data =>
						data.userChampionUID == enemy.userChampionUID).championBaseUID;

				if (baseID != null)
				{
					var enemyBase =
						GameData.chamBaseDB.listChampionBasesInTool.FirstOrDefault(cham =>
							cham.Value.KeyName == baseID).Value;

					if (enemyBase.Class.KeyName == "Tank")
					{
						damageDealtData.AddBonusDamage(0.2f, StatValueType.Percent);
					}
				}
			}
			
		}
	}

}