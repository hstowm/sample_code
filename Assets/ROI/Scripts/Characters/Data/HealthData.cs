using System;
using Mirror;

namespace ROI
{
    [Serializable]
    public readonly struct HealthData : IEquatable<HealthData>
    {
        [SyncVar] public readonly int health; // = 100; 
        [SyncVar] public readonly int maxHealth; // = 100;

        public bool Equals(HealthData other)
        {
            return health == other.health && maxHealth == other.maxHealth;
        }

        public HealthData(int h, int m)
        {
            health = h;
            maxHealth = m;
        } 
    }
}