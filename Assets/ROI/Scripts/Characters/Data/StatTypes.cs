using System.Runtime.Serialization;

namespace ROI
{
    /// <summary>
    /// Stat types
    /// </summary>
    public enum StatTypes : byte
    {
        [EnumMember(Value = "localize_stat_attackDamage_name")] AttackDamage = 0,
        [EnumMember(Value = "localize_stat_attackSpeed_name")] AttackSpeed = 1,
        [EnumMember(Value = "localize_stat_attackRange_name")] AttackRange,
        [EnumMember(Value = "localize_stat_health_name")] Health,
        [EnumMember(Value = "localize_stat_maxHealth_name")] MaxHealth,
        [EnumMember(Value = "localize_stat_armor_name")] Armor,
        [EnumMember(Value = "localize_stat_magicDef_name")] MagicDef,
        [EnumMember(Value = "localize_stat_critChance_name")] CritChance,
        [EnumMember(Value = "localize_stat_maxFury_name")] MaxUltimateEnergy,
        [EnumMember(Value = "localize_stat_moveSpeed_name")] MoveSpeed,
        [EnumMember(Value = "localize_stat_healthRegen_name")] HealthRegen,
        [EnumMember(Value = "localize_stat_critDamage_name")] CritDamage,
        [EnumMember(Value = "localize_stat_abilityCritChance_name")] CritChanceOnAbility,
        [EnumMember(Value = "localize_stat_dodgeChance_name")] DodgeChance,
        [EnumMember(Value = "localize_stat_lifeSteal_name")] LifeSteal,
        [EnumMember(Value = "localize_stat_abilityPower_name")] AbilityPower,
        [EnumMember(Value = "localize_stat_armorPenetration_name")] ArmorPenetration,
        [EnumMember(Value = "localize_stat_magicPenetration_name")] MagicPenetration,
        [EnumMember(Value = "localize_stat_damageTaken_name")] DamageTaken,
        [EnumMember(Value = "localize_stat_passiveFuryGeneration_name")] IncreasePasiveFurry,
        [EnumMember(Value = "localize_stat_damageReduction_name")] DamageReduction,
        [EnumMember(Value = "localize_stat_chanceCounterAttack_name")] ChanceToCounterAttackOnHit,
        [EnumMember(Value = "localize_stat_furySkill_name")] InCreaseFurryGenerationWhenPlayCard,
        [EnumMember(Value = "localize_stat_burnAura_name")] BurningAura,
        [EnumMember(Value = "localize_stat_thorns_name")] ReflectPhysicalDamage,
        [EnumMember(Value = "localize_stat_reduceAttackerAtkSpeed_name")] ReduceAttackerAttackSpeed,
        [EnumMember(Value = "localize_stat_skillBlessChance_name")] ChanceToApplyBleedOnSpell,
        [EnumMember(Value = "localize_stat_shieldOnStart_name")] ShieldOnStartCombat,
        [EnumMember(Value = "localize_stat_chanceToStun_name")] ChanceToStunOnHit,
        [EnumMember(Value = "localize_stat_chanceToChill_name")] ChanceToChillOnHit,
        [EnumMember(Value = "localize_stat_chanceToPoison_name")] ChanceToPoisonOnHit,
        [EnumMember(Value = "localize_stat_chanceToFrenzy_name")] ChanceToFrenzyOnHit,
        [EnumMember(Value = "localize_stat_chanceToEngulf_name")] ChanceToEngulfSelfOnHit,
        [EnumMember(Value = "localize_stat_chanceToBless_name")] ChanceToBlessSelfOnHit,
        [EnumMember(Value = "localize_stat_chanceToVulnerable_name")] ChanceToVulnerableOnHit,
        
        COUNT
    }

    /// <summary>
    /// Fat number or percent from base
    /// </summary>
    public enum StatValueTypes { Flat, Percent }
}