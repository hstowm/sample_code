namespace ROI
{
	/// <summary>
	/// Call After applying Critical Damage have been determined. Run Only Server side
	/// </summary>
	public interface IOnCriticalCalculated
	{
		void OnCriticalApplied(DamageDealtData damageDealtData);
	}
}