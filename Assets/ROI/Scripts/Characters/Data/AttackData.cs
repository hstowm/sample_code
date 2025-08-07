using System;

namespace ROI
{
    public enum DamageTypes : byte
    {
        Physic,
        Magic,
        True
    }

    /// <summary>
    /// Attack Data
    /// </summary>
    [Serializable]
    public class AttackData
    {
        /// <summary>
        /// base damage
        /// </summary>
        public float baseDamage = 1;

        public float damage = 1;

        /// <summary>
        /// Range
        /// </summary>
        public float baseRange = 1;

        public float range = 1;

        
        /// <summary>
        /// % Chance to make critical damage (deal 150% instead of 100% damage)
        /// </summary>
        public float critDamageChance = 0.1f;
        
        /// <summary>
        ///  % bonus to critical damage ( % critical damage + 150% critical damage)
        /// </summary>
        public float critDamage = 1f;
        
        /// <summary>
        /// Attack Speed
        /// </summary>
        public float baseSpeed = 1;

        public float speed = 1;

        /// <summary>
        /// Damage Type
        /// </summary>
        public DamageTypes damageType = DamageTypes.Physic;

        
        /// <summary>
        /// the angle of 2 forward (champion and enemy) to start attack
        /// </summary>
        public float startAttackAngle = 20;
    }
}