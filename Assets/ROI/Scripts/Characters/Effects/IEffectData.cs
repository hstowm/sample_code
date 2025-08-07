using System;

namespace ROI
{
    /// <summary>
    /// An Effect Data
    /// </summary>
    public interface IEffectData : IEquatable<IEffectData>
    {
        public ChampionEffects Effect { get; }
    }
}