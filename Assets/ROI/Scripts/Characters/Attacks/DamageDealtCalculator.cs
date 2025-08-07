using System.Runtime.CompilerServices;
using ROI.DataEntity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ROI
{
	static class DamageDealtCalculator
	{
		public static bool HasChance(float criticalChance)
		{
			var percent = Random.Range(0, 100) + 1;
			var chance = Mathf.FloorToInt(criticalChance * 100);

			return percent <= chance;
		}

		/// <summary>
		/// Post Mitigation Damage Formula
		/// </summary>
		/// <param name="rawDamage"></param>
		/// <param name="armor"></param>
		/// <returns></returns>
		public static float CalcPostMitigationDamage(float rawDamage, float armor)
		{
			return rawDamage * 100 / (100 + armor);
		}

		/// <summary>
		///  Add a bonus damage. Percent Or Flat Number
		/// </summary>
		/// <param name="damageDealtData"></param>
		/// <param name="bonusDamage"></param>
		/// <param name="percentOrFlat"></param>
		public static void AddBonusDamage(this DamageDealtData damageDealtData, float bonusDamage, StatValueType percentOrFlat = StatValueType.Flat)
		{
			if (percentOrFlat == StatValueType.Percent)
				damageDealtData.attackDamagePercentBonus += bonusDamage;
			else
				damageDealtData.attackDamage += bonusDamage;

			// damageDealtData.attackDamage += (percentOrFlat == StatValueType.Fat) ? bonusDamage : bonusDamage * damageDealtData.baseDamage;
		}

		/// <summary>
		/// add base target armor
		/// </summary>
		/// <param name="damageDealtData"></param>
		/// <param name="bonusArmor"></param>
		/// <param name="percentOrFlat"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddBonusTargetArmor(this DamageDealtData damageDealtData, float bonusArmor, StatValueType percentOrFlat = StatValueType.Flat)
		{
			if (percentOrFlat == StatValueType.Flat)
				damageDealtData.targetArmor += bonusArmor;
			else
				damageDealtData.targetArmorBonusPercent += bonusArmor;

			// damageDealtData.targetArmor += (percentOrFlat == StatValueType.Fat) ? bonusArmor : bonusArmor * damageDealtData.baseTargetArmor;
		}

		/// <summary>
		/// Add Bonus target armor
		/// </summary>
		/// <param name="damageDealtData"></param>
		/// <param name="bonusDefenseData"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddBonusTargetArmor(this DamageDealtData damageDealtData, DefenseData bonusDefenseData)
		{
			damageDealtData.targetArmor += damageDealtData.damageType == DamageTypes.Magic ? bonusDefenseData.magicDef : bonusDefenseData.physicDef;
		}

		public static float GetFinalDamage(this DamageDealtData damageDealtData)
		{
			if (damageDealtData.isCalculated)
				return damageDealtData.finalDamage;

			Logs.Error("Cant Get Final Damage when it's not calculated");
			return 0;
		}

		/// <summary>
		/// Auto Attack Damage Final = ((ChampBase + FlatDamage) * (1+Total Bonus Percent) * (1+CritDamagePercent)  * (100/(100+(TotalArmor(1-TotalArmor)))) - TotalAbsorb ) *DamageScale
		/// Ability Final Damage = (Ability Damage * (1+CritDamagePercent) * (100/(100+(TotalArmor(1-TotalArmor)))) - TotalAbsorb ) * DamageScale * (1 + (AD or AP)/100);
		///	Notes:
		///  - AP for magic damage, AD for physic damage
		///  - AD = ((ChampBase + FlatDamageBonus) * (1+Total Bonus Percent) 
		///  - AP = (Ability Power + FlatDamageBonus) * (1+Total Bonus Percent)
		/// </summary>
		/// <param name="damageDealtData"></param>
		/// <param name="isServerSide"></param>
		public static void CalculateFinalDamage(this DamageDealtData damageDealtData, bool isServerSide)
		{
			// Dont Re-calculate damage
			if (damageDealtData.isCalculated)
			{
				Logs.Error("Cant re-calculate");
				return;
			}

			// true damage
			damageDealtData.damageOnHit = 0;
			damageDealtData.isCalculated = true;

			var totalAbsorb = damageDealtData.targetAbsorb + damageDealtData.targetShield;

			// TRUE DAMAGE
			if (damageDealtData.damageType == DamageTypes.True)
			{
				// true ability damage
				if (damageDealtData.damageSource.IsAbility() || damageDealtData.damageSource.IsEffect())
					damageDealtData.attackDamage = damageDealtData.abilityDamage;

				totalAbsorb += damageDealtData.attackDamage * damageDealtData.targetAbsorbDamagePercent;
				damageDealtData.finalDamage = damageDealtData.damageOnHit = Mathf.Max(0, damageDealtData.attackDamage - totalAbsorb);
				damageDealtData.UpdateShieldTaken(damageDealtData.attackDamage);

				return; // damageDealtData.damageOnHit;
			}

			// APPLY DODGE
			damageDealtData.isDodge = HasChance(damageDealtData.dodgeChancePercent);
			damageDealtData.finalDamage = 0;

			if (damageDealtData.isDodge)
				return;

			// Apply Crit Damage Bonus
			var critDamageBonus = 0f;

			// can be crit
			if (damageDealtData.canCrit)
			{
				// apply crit Chance
				damageDealtData.isCrit = HasChance(damageDealtData.critDamageChance);

				// calculate crit damage bonus
				if (damageDealtData.isCrit)
					critDamageBonus = damageDealtData.critDamageBonus;

				// call Critical hooks
				if (isServerSide && damageDealtData.isCrit)
					foreach (var onCritical in damageDealtData.hooks.OnCriticals)
					{
						onCritical.OnCriticalApplied(damageDealtData);
					}
			}

			// attack damage & Damage scale with ability

			var attackDamage = damageDealtData.attackDamage * (1 + damageDealtData.attackDamagePercentBonus);
			var damageScale = 1f;

			// re-apply when damage is ability
			if (damageDealtData.isAbility)
			{
				// recalculate magic damage
				if (damageDealtData.damageType == DamageTypes.Magic)
				{
					var damageFlatBonus = damageDealtData.attackDamage - damageDealtData.baseDamage;

					attackDamage = (damageDealtData.abilityPower + damageFlatBonus) * (1 + damageDealtData.attackDamagePercentBonus);
				}
				if (!damageDealtData.damageSource.IsEffect())
				{
					damageScale = (1f + attackDamage / 100f);
				}

				// set current attack damage is ability damage
				attackDamage = damageDealtData.abilityDamage;
			}

			attackDamage *= (1 + critDamageBonus);

			// Logs.Info($"damageScale: {damageScale}");

			// calculate armor
			var armor = damageDealtData.targetArmor * (1 + damageDealtData.targetArmorBonusPercent) * (1 - damageDealtData.armorPenetration);

			// calculate post damage
			var postDamage = CalcPostMitigationDamage(attackDamage, armor);

			// update shield with shield taken
			damageDealtData.UpdateShieldTaken(postDamage);

			// apply Absorb damage percent
			totalAbsorb += damageDealtData.targetAbsorbDamagePercent * postDamage;

			// post damage - absorb
			postDamage = Mathf.Max(0, postDamage - totalAbsorb);

			// damage on hit
			damageDealtData.damageOnHit = postDamage * damageDealtData.damageTaken;

			// damage scale: damage taken * block percent
			damageScale *= damageDealtData.damageTaken * (1 - Mathf.Clamp01(damageDealtData.blockPercent));

			// mark as calculated
			damageDealtData.finalDamage = postDamage * damageScale;
		}

		public static DamageDealtData Clone(this DamageDealtData damageDealtData, DamageSources damageSources, DamageTypes damageType, float newBaseDamage, float newBaseArmor)
		{
			var damageData = new DamageDealtData(damageSources, damageType, newBaseDamage, newBaseArmor);
			damageData.attackDamage = damageDealtData.attackDamage;

			damageData.attackDamagePercentBonus = damageDealtData.attackDamagePercentBonus;

			damageData.canCrit = damageDealtData.canCrit;
			damageData.critDamageBonus = damageDealtData.critDamageBonus;
			damageData.critDamageChance = damageDealtData.critDamageChance;
			damageData.isCrit = damageDealtData.isCrit;

			// public float damagePower;
			damageData.targetShield = damageDealtData.targetShield;
			damageData.targetAbsorb = damageDealtData.targetAbsorb;
			damageData.targetAbsorbDamagePercent = damageDealtData.targetAbsorbDamagePercent;
			// public float targetHealthReduction;

			damageData.targetArmor = damageDealtData.targetArmor;
			damageData.targetArmorBonusPercent = damageDealtData.targetArmorBonusPercent;

			damageData.armorPenetration = damageDealtData.armorPenetration;

			damageData.damageTaken = damageDealtData.damageTaken;

			damageData.blockPercent = damageDealtData.blockPercent;

			damageData.isDodge = damageDealtData.isDodge;
			damageData.dodgeChancePercent = damageDealtData.dodgeChancePercent;

			damageData.abilityPower = damageDealtData.abilityPower;
			damageData.abilityDamage = damageDealtData.abilityDamage;

			damageData.damageOnHit = damageDealtData.damageOnHit;

			damageData.isCalculated = damageDealtData.isCalculated;

			damageData.shieldTaken = damageDealtData.shieldTaken;

			damageData.finalDamage = damageDealtData.finalDamage;

			if (damageDealtData.hooks != null)
			{
				damageData.hooks.OnCriticals.AddRange(damageDealtData.hooks.OnCriticals);
				damageData.hooks.OnFinalDamages.AddRange(damageDealtData.hooks.OnFinalDamages);
			}

			return damageData;
		}

		private static void UpdateShieldTaken(this DamageDealtData damageDealtData, float postDamage)
		{
			if (damageDealtData.isCalculated == false)
			{
				Logs.Error("Cant Update when Damage Dealt Data have been not calculated");
				return;
			}

			damageDealtData.shieldTaken =
				damageDealtData.finalDamage > 0 || postDamage >= damageDealtData.targetShield
					?
					damageDealtData.targetShield :
					postDamage;
		}

	}
}