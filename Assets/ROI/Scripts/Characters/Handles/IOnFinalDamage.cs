namespace ROI
{
	/// <summary>
	/// On Damage Calculated. Only Run On Server Side
	/// </summary>
	public interface IOnFinalDamage
	{
		/// <summary>
		/// Only run on server side
		/// </summary>
		/// <param name="damageDealtData"></param>
		void OnDamageCalculated(DamageDealtData damageDealtData);
	}
}