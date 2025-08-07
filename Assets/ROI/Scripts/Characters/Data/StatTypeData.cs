using System;

namespace ROI
{

    /// <summary>
    /// Stat Type and Value
    /// </summary>
    [Serializable]
    public struct StatTypeData : IEquatable<StatTypeData>
    {
        public StatTypes statType;
        public float value;
        public StatValueTypes valueType;

        public bool Equals(StatTypeData other)
        {
            return other.statType == statType && Math.Abs(value - other.value) <= 0.001f && valueType == other.valueType ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(statType, value, valueType);
        }
        public override bool Equals(object obj)
        {
            if (obj is StatTypeData ob)
                return Equals(ob);

            return false;
        }

        public StatTypeData(StatTypes type, float v, StatValueTypes vType = StatValueTypes.Flat)
        {
            statType = type;
            value = v;
            valueType = vType;
        }
    }
}