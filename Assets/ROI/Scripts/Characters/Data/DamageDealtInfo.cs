namespace ROI
{
	/// <summary>
	/// Damage Dealt Info. After The Damage applied on champion
	/// </summary>
	public readonly struct DamageDealtInfo
	{
		public readonly  bool IsCrit;
		public readonly bool AttackDamage;
		public readonly bool BaseDamage;
		public readonly bool CritDamage;
		public readonly bool CritChance;
		public readonly float FinalDamage;
		public readonly bool isDodge;

		public readonly DamageSources source;
		public readonly DamageTypes damageType;
		
		
	}
}