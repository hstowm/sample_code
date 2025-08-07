using System.Collections.Generic;
namespace ROI
{
	/// <summary>
	/// All hooks in damage dealt calculate
	/// </summary>
	public class DamageCalculateHook
	{
		/// <summary>
		/// Call On
		/// </summary>
		public readonly List<IOnFinalDamage> OnFinalDamages = new List<IOnFinalDamage>();
		public readonly List<IOnCriticalCalculated> OnCriticals = new List<IOnCriticalCalculated>();

		public void Clear()
		{
			OnFinalDamages.Clear();
			OnCriticals.Clear();
		}
	}
}