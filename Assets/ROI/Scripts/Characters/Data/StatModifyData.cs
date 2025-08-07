using System;
using System.Collections.Generic;

namespace ROI
{
	/// <summary>
	/// Stat Modify Data
	/// </summary>
	[Serializable]
	public struct StatModifyData : IEquatable<StatModifyData>
	{
		public SourceTypes sourceType;
		public List<StatTypeData> stats;

		public StatModifyData(SourceTypes srcType)
		{
			sourceType = srcType;
			stats = new List<StatTypeData>(4);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(sourceType, stats);
		}

		public bool Equals(StatModifyData other)
		{
			Logs.Warning($"Dont Use Equals Function For This Type: {typeof(StatModifyData).FullName}");

			return other.sourceType == sourceType;
		}
		public override bool Equals(object obj)
		{
			if (obj is StatModifyData data)
				return Equals(data.sourceType);

			return false;
		}
	}
}