using ROI.DataEntity;
using UnityEngine;
namespace ROI
{
	//Increase my armor and magic resistance by 5% for each enemy around me (within range 1)
	[CreateAssetMenu(fileName = "BattlefieldAdaptationPassive", menuName = "ROI/Data/AbilityPassiveCards/BattlefieldAdaptation")]
	public class BattlefieldAdaptationPassive : BasePassiveAbilityCard, IAbilityCard
	{
		public float percentDef;


		// [Server]
		public override void OnInit(ChampionData champion)
		{
			champion.handles.OnAttacked.Add(new BattlefieldAdaptationInject(champion, percentDef));
		}

		class BattlefieldAdaptationInject : IOnAttacked //,IOnDamageCalculated
		{
			ChampionData championData;
			float percentDef;
			public BattlefieldAdaptationInject(ChampionData championData, float percentDef)
			{
				this.championData = championData;
				this.percentDef = percentDef;
			}

			//chua check duoc vi khong sua duoc healthReduction
			public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
			{
				int countEnemyInRange = 0;
				foreach (var enemy in championData.enemies)
				{
					if (Vector3.Distance(championData.transform.position, enemy.transform.position) <= 2)
					{
						countEnemyInRange++;
					}
				}

				var armorBonus = percentDef * countEnemyInRange;

				// var bonusDefense = championData.defenseData;
				//
				// bonusDefense.magicDef = armorBonus * bonusDefense.baseMagicDef;
				// bonusDefense.physicDef = armorBonus * bonusDefense.basePhysicDef;
				//
				//damageDealtData.Print();

				damageDealtData.AddBonusTargetArmor(armorBonus, StatValueType.Percent);

				//damageDealtData.OnCalculateds.Add(this);
				//
				//
				// var statTypeData = new StatModifyData(SourceTypes.PassiveSkill);
				//
				// statTypeData.stats = new List<StatTypeData>();
				// StatTypeData dataMagicAdd = new StatTypeData(StatTypes.MagicDef, percentDef, StatValueTypes.Percent);
				// StatTypeData dataAmorAdd = new StatTypeData(StatTypes.Armor, percentDef, StatValueTypes.Percent);
				// statTypeData.stats.Add(dataMagicAdd);
				// statTypeData.stats.Add(dataAmorAdd);
				//
				// championData.statModifier.AddModify(statTypeData);
				//
				// damageDealtData.healthReduction = DamageDealtCalculator.CalcHeathReduction(championData,
				// 	attacker.attackData,
				// 	out damageDealtData.attackDamage,
				// 	out damageDealtData.critDamagePercent,
				// 	out damageDealtData.isCit);
				//
				// championData.statModifier.RemoveModify(statTypeData);

			}
			public void OnDamageCalculated(DamageDealtData damageDealtData)
			{
				//damageDealtData.Print();
			}
		}
	}
}