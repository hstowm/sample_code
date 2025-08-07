using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ROI
{
	/// <summary>
	/// Champion Stat Modifier
	/// </summary>
	public readonly struct ChampionStatModifier
	{
		private readonly ChampionData _championData;
		private readonly ChampionMoveSpeed _championMoveSpeed;
		private readonly ChampionAutoAttackSpeed _autoAttackSpeed;

		/// <summary>
		/// List all stat modifiers which have been apply to the champion
		/// </summary>
		public readonly Dictionary<SourceTypes, List<StatModifyData>> statModifiers;

		public ChampionStatModifier(ChampionData championData, ChampionMoveSpeed championMoveSpeed, ChampionAutoAttackSpeed autoAttackSpeed)
		{
			_championData = championData;
			statModifiers = new Dictionary<SourceTypes, List<StatModifyData>>(16);
			_championMoveSpeed = championMoveSpeed;
			_autoAttackSpeed = autoAttackSpeed;
		}

		/// <summary>
		/// Applying a Modifier
		/// </summary>
		/// <param name="statModifyData"></param>
		public void AddModify(StatModifyData statModifyData)
		{
			if (statModifiers.TryGetValue(statModifyData.sourceType, out var modifiers) == false)
			{
				modifiers = new List<StatModifyData>();
				statModifiers.Add(statModifyData.sourceType, modifiers);
			}

			var statModify = new StatModifyData(statModifyData.sourceType);
			statModify.stats.AddRange(statModifyData.stats.ToArray());

			ApplyModify(statModify);
			modifiers.Add(statModify);
		}

		/// <summary>
		/// Remove a modifier
		/// </summary>
		/// <param name="statModifyData"></param>
		public void RemoveModify(StatModifyData statModifyData)
		{
			if (statModifiers.TryGetValue(statModifyData.sourceType, out var modifiers) == false)
			{
				return;
			}

			var index = modifiers.FindIndex(s => s.sourceType == statModifyData.sourceType);
			if (index < 0)
				return;

			var listStats = modifiers[index].stats;

			foreach (var statTypeData in statModifyData.stats)
			{
				for (int i = listStats.Count - 1; i >= 0; i--)
				{
					if (listStats[i].Equals(statTypeData))
					{
						ApplyModify(statTypeData, -1);
						listStats.RemoveAt(i);
						break;
					}
				}
			}
		}

		public void AddModify(SourceTypes sourceType, StatTypeData statTypeData)
		{
			var statModifyData = new StatModifyData(sourceType)
			{
				stats = new List<StatTypeData>() { statTypeData }
			};

			if (statModifiers.TryGetValue(sourceType, out _) == false)
			{
				statModifiers.Add(sourceType, new List<StatModifyData>()
				{
					statModifyData
				});

				ApplyModify(statTypeData);
				return;
			}

			statModifiers[sourceType].Add(statModifyData);
			ApplyModify(statModifyData);
		}

		public void RemoveModify(SourceTypes sourceType, StatTypeData statTypeData)
		{
			var statModifyData = new StatModifyData(sourceType)
			{
				stats = new List<StatTypeData>() { statTypeData }
			};

			RemoveModify(statModifyData);
		}

		public void ApplyModify(StatModifyData statModifyData, int valueMultiplier = 1)
		{
			ApplyModify(statModifyData.stats, valueMultiplier);
		}

		public void ApplyModify(List<StatTypeData> statChangeDatas, int valueMultiplier = 1)
		{
			foreach (var data in statChangeDatas)
			{
				ApplyModify(data, valueMultiplier);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float GetValue(StatTypeData statData, float baseValue)
		{
			return statData.valueType == StatValueTypes.Flat ? statData.value : statData.value * baseValue;
		}

		// [Command(requiresAuthority = false)]
		// public void ApplyStatToServer(StatTypeData statTypeData)
		// {
		//     ApplyModify(statTypeData, 1);
		// }

		public void ApplyModify(StatTypeData data, int valueMultiplier = 1)
		{
			switch (data.statType)
			{
				// increase
				case StatTypes.Health:
					var health = _championData.healthData;
					var healthValue = Mathf.FloorToInt(GetValue(data, health.maxHealth) * valueMultiplier);
					var h = Mathf.Min(healthValue + health.health, health.maxHealth);

					_championData.healthData = new HealthData(h, health.maxHealth);
					break;

				// increase max health
				case StatTypes.MaxHealth:
					var v = Mathf.FloorToInt(GetValue(data, _championData.healthData.maxHealth) * valueMultiplier);
					var healthData = _championData.healthData;
					_championData.healthData = new HealthData(healthData.health + v, healthData.maxHealth + v);
					break;

				// attack damage
				case StatTypes.AttackDamage:
					_championData.attackData.damage +=
						GetValue(data, _championData.attackData.baseDamage) * valueMultiplier;
					break;

				// attack speed
				case StatTypes.AttackSpeed:
					_championData.attackData.speed +=
						GetValue(data, _championData.attackData.baseSpeed) * valueMultiplier;
					
					_autoAttackSpeed.OnAttackSpeedChanged();
					break;

				// Move Speed
				case StatTypes.MoveSpeed:
					_championData.moveData.moveSpeed +=
						GetValue(data, _championData.moveData.baseMoveSpeed) * valueMultiplier;

					_championMoveSpeed.OnMoveSpeedChanged();
					break;

				// Armor
				case StatTypes.Armor:
					_championData.defenseData.physicDef +=
						GetValue(data, _championData.defenseData.basePhysicDef) * valueMultiplier;
					break;

				// magic def
				case StatTypes.MagicDef:
					_championData.defenseData.magicDef +=
						GetValue(data, _championData.defenseData.baseMagicDef) * valueMultiplier;
					break;

				// crit chance
				case StatTypes.CritChance:
					// data.valueType = StatValueTypes.Flat;
					_championData.attackData.critDamageChance +=
						GetValue(data, _championData.attackData.critDamageChance) * valueMultiplier;
					break;

				// crit damage
				case StatTypes.CritDamage:
					// data.valueType = StatValueTypes.Flat;
					_championData.attackData.critDamage +=
						GetValue(data, _championData.attackData.critDamage) * valueMultiplier;
					break;

				// dodge chance
				case StatTypes.DodgeChance:
					// data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.dodgeChance +=
						GetValue(data, _championData.specialStatData.dodgeChance) * valueMultiplier;
					break;

				// ability power
				case StatTypes.AbilityPower:
					//  data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.abilityPower +=
						GetValue(data, _championData.specialStatData.abilityPower) * valueMultiplier;
					break;

				// health regen per second
				case StatTypes.HealthRegen:
					// data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.healthRegen +=
						GetValue(data, _championData.specialStatData.healthRegen) * valueMultiplier;
					break;

				case StatTypes.CritChanceOnAbility:
					//   data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.critChanceOnAbility +=
						GetValue(data, _championData.specialStatData.critChanceOnAbility) * valueMultiplier;
					break;

				case StatTypes.AttackRange:
					_championData.attackData.range +=
						GetValue(data, _championData.attackData.baseRange) * valueMultiplier;
					break;

				case StatTypes.MaxUltimateEnergy:
					//   data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.maxUltimateEnergy +=
						GetValue(data, _championData.specialStatData.maxUltimateEnergy) * valueMultiplier;
					break;

				case StatTypes.LifeSteal:
					//   data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.lifeSteal +=
						GetValue(data, _championData.specialStatData.lifeSteal) * valueMultiplier;
					break;

				case StatTypes.ArmorPenetration:
					//    data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.armorPenetration +=
						GetValue(data, _championData.specialStatData.armorPenetration) * valueMultiplier;
					break;

				case StatTypes.MagicPenetration:
					//  data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.magicPenetration +=
						GetValue(data, _championData.specialStatData.magicPenetration) * valueMultiplier;
					break;

				case StatTypes.DamageTaken:
					//  data.valueType = StatValueTypes.Flat;
					_championData.specialStatData.damageTaken +=
						GetValue(data, _championData.specialStatData.damageTaken) * valueMultiplier;
					break;
				default:
					Debug.LogWarning("Type is not supported yet!");
					break;
			}
		}

		public void RemoveModify(StatTypeData data, int valueMultiplier = 1)
		{
			ApplyModify(data, -valueMultiplier);
		}
	}
}