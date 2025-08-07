using System;

namespace ROI
{
	/// <summary>
	/// Damage Dealt Data
	/// </summary>
	[Serializable]
	public class DamageDealtData
	{
		public readonly DamageSources damageSource;
		public readonly DamageTypes damageType;

		public readonly bool isAbility;

		/// <summary>
		/// Current Total Attack damage
		/// </summary>
		public readonly float baseDamage;
		public readonly float baseTargetArmor;

		public float abilityDamage;
		public float attackDamage;

		public float attackDamagePercentBonus;

		public bool canCrit;
		public float critDamageBonus;
		public float critDamageChance;
		public bool isCrit;

		// public float damagePower;
		public float targetShield;
		public float targetAbsorb;
		public float targetAbsorbDamagePercent;
		// public float targetHealthReduction;

		public float targetArmor;
		public float targetArmorBonusPercent;

		public float armorPenetration;

		public float damageTaken;

		public float blockPercent;

		public bool isDodge;
		public float dodgeChancePercent;

		public float abilityPower;

		/// <summary>
		/// Damage On hit (with dodge, without block) For LifeSteal, Reflect
		/// </summary>
		public float damageOnHit;

		public bool isCalculated;

		public float shieldTaken;

		public float finalDamage;

		public readonly DamageCalculateHook hooks = new DamageCalculateHook();

		public void Print()
		{
#if UNITY_EDITOR
			Logs.Info($"Damage Source: {damageSource}.\r\n" +
					  $"DamgeType:{damageType}.\r\n" +
					  $"baseRawDamage:{baseDamage}.\r\n" +
					  $"AbilityDamage:{abilityDamage}.\r\n" +
					  $"baseTargetArmor:{baseTargetArmor}.\r\n" +
					  $"attackDamage:{attackDamage}.\r\n" +
					  $"attackDamagePercentBonus:{attackDamagePercentBonus}.\r\n" +
					  $"canCrit:{canCrit}.\r\n" +
					  $"critDamageBonus:{critDamageBonus}.\r\n" +
					  $"critDamageChance:{critDamageChance}.\r\n" +
					  $"isCrit:{isCrit}.\r\n" +
					  $"AbilityPower:{abilityPower}.\r\n" +
					  $"targetShield:{targetShield}.\r\n" +
					  $"targetAbsorb:{targetAbsorb}.\r\n" +
					  $"targetAbsorbDamagePercent:{targetAbsorbDamagePercent}.\r\n" +
					  //$"targetHealthReduction:{targetHealthReduction}.\r\n" +
					  $"targetArmor:{targetArmor}.\r\n" +
					  $"amorPenetration:{armorPenetration}.\r\n" +
					  $"damageTaken:{damageTaken}.\r\n" +
					  $"blockPercent:{blockPercent}.\r\n" +
					  $"isDodge:{isDodge}.\r\n" +
					  $"dodgeChancePercent:{dodgeChancePercent}.\r\n" +
					  $"damageOnHit:{damageOnHit}.\r\n" +
					  $"isCalculated:{isCalculated}.\r\n" +
					  $"shieldTaken:{shieldTaken}.\r\n" +
					  $"finalDamageCalculated:{finalDamage}\r\n");

#endif
		}

		/// <summary>
		/// Damage Dealt Data
		/// </summary>
		/// <param name="source"></param>
		/// <param name="type"></param>
		/// <param name="baseDamage"></param>
		/// <param name="baseArmor"></param>
		public DamageDealtData(DamageSources source, DamageTypes type, float baseDamage, float baseArmor)
		{
			damageSource = source;
			damageType = type;

			isAbility = damageSource.IsAbility() || damageSource.IsEffect();

			this.baseDamage = baseDamage;
			attackDamage = baseDamage;

			attackDamagePercentBonus = 0;

			canCrit = true;
			critDamageBonus = 0;
			critDamageChance = 0;
			isCrit = false;

			armorPenetration = 0;
			targetAbsorb = 0;
			targetAbsorbDamagePercent = 0;
			targetShield = 0;

			baseTargetArmor = baseArmor;
			targetArmor = 0;

			abilityPower = 1;
			abilityDamage = 0;

			damageTaken = 1;

			blockPercent = 0;

			isDodge = false;
			dodgeChancePercent = 0;

			damageOnHit = 0;

			isCalculated = false;
			shieldTaken = 0;

			hooks.Clear();
		}

		public DamageDealtData(
			DamageSources source,
			DamageTypes type,
			float baseDamage,
			DefenseData enemyDefenseData)
			: this(source, type, baseDamage, 0)
		{

			if (type == DamageTypes.True)
				return;

			baseTargetArmor = type == DamageTypes.Magic ? enemyDefenseData.baseMagicDef : enemyDefenseData.basePhysicDef;
			targetArmor = type == DamageTypes.Magic ? enemyDefenseData.magicDef : enemyDefenseData.physicDef;
		}

		public DamageDealtData(DamageSources source, AttackData attackData, DefenseData enemyDefenseData)
			: this(source, attackData.damageType, attackData.baseDamage, enemyDefenseData)
		{
			attackDamage = attackData.damage;
		}
	}

	[System.Flags]
	public enum DamageSources : ushort
	{
		BasicAttack = 1,
		CoreActiveSkill = 1 << 1,
		CorePassiveSkill = 1 << 2,
		UltimateSkill = 1 << 3,
		PassiveCardSkill = 1 << 4,
		ActiveCardSkill = 1 << 5,
		Trait = 1 << 6,
		Effect = 1 << 7,

		ActiveAbility = CoreActiveSkill | UltimateSkill | ActiveCardSkill,
		PassiveAbility = CorePassiveSkill | PassiveCardSkill | Trait,

		Ability = ActiveAbility | PassiveAbility,
		Everything = ushort.MaxValue
	}
}