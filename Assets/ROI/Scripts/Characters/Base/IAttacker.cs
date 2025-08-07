namespace ROI
{
    public interface IAttacker
    {
        /// <summary>
        /// Basic Attack
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="damageDealtData"></param>
        void AttackEnemy(ChampionData enemy, out DamageDealtData damageDealtData);

        /// <summary>
        /// Is not basic Attack
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="rawDamage"></param>
        /// <param name="damageSource"></param>
        /// <param name="dameType"></param>
        void AttackEnemy(ChampionData enemy, float rawDamage, DamageSources damageSource, DamageTypes dameType);

        /// <summary>
        /// Apply Direct Damage To Enemy
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="damageDealtData"></param>
        void ApplyDamageDealt(ChampionData enemy, DamageDealtData damageDealtData);

    }
}