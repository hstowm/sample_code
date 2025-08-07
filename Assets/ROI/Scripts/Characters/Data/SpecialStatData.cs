using System;
namespace ROI
{
    /// <summary>
    /// All Bonus Data when applied
    /// </summary>
   
    [Serializable]
    public struct SpecialStatData
    {
        /// <summary>
        /// Max Ultimate Energy
        /// </summary>
        public float maxUltimateEnergy;

        /// <summary>
        /// Health Regen per second
        /// </summary>
        public float healthRegen;// = 0;

        /// <summary>
        /// Critical chance on ability
        /// </summary>
        public float critChanceOnAbility;// = 0;

        /// <summary>
        /// Auto Attack Dodge Chance Percent
        /// </summary>
        public float dodgeChance;// = 0;

        /// <summary>
        /// life steal
        /// </summary>
        public float lifeSteal;// = 0;
        
        /// <summary>
        /// Ability power 
        /// </summary>
        public float abilityPower;// = 1;
        
        /// <summary>
        /// Armor penetration Percent
        /// </summary>
        public float armorPenetration;// = 0;
        
        /// <summary>
        /// Magic Penetration. Percent
        /// </summary>
        public float magicPenetration;// = 0;
        
        /// <summary>
        /// Damage taken
        /// </summary>
        public float damageTaken;//  = 1;
        /// <summary>
        /// increase passive furry
        /// </summary>
        public float increasePassiveFurry;
        /// <summary>
        /// Increase fury generation when playing cards/level
        /// </summary>
        public float increaseFurryGenerationWhenPlayCard;
        /// <summary>
        /// Reflect X% of damage back to the attacker/level
        /// </summary>
        public float reflectDamage;
        /// <summary>
        /// 
        /// </summary>
        public float chanceToApplyBlessOnSpell;
        /// <summary>
        /// Starts combat with a shield equal to X% of maxium health/level
        /// </summary>
        public float shieldOnStartCombat;
        /// <summary>
        /// Chance to Stun for 2 seconds on hit
        /// </summary>
        public float chanceToStunOnHit;
        /// <summary>
        /// Chance to Chill on-hit/level
        /// </summary>
        public float chanceToChillOnHit;
        /// <summary>
        /// Chance to Poison on-hit/level
        /// </summary>
        public float chanceToPoisonedOnHit;
        /// <summary>
        /// Reduce attacker's attack speed by X% for 3 seconds
        /// </summary>
        public float reduceAttackerAtkSpeed;
        /// <summary>
        /// Chance to Frenzy on-hit (self gain)/level
        /// </summary>
        public float chanceToFrenzyOnHit;
        /// <summary>
        /// Chance to Engulf on-hit (self gain)/level
        /// </summary>
        public float chanceToEngulfOnHit;
        /// <summary>
        /// Chance to Blessed on-hit (self gain)/level
        /// </summary>
        public float chanceToBlessOnHit;
        /// <summary>
        /// Chance to Vulnerable on-hit/level
        /// </summary>
        public float chanceToVulnerableOnHit;

        
    }
}