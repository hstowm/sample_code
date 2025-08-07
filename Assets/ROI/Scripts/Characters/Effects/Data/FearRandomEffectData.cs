
namespace ROI{
    using System.Collections;

    public class FearRandomEffectData : IEffectData
    {
        public float timeRandomPosition = 2;
        public float numberLoop = 3;
        public IEnumerator handle;
        public uint netID;
        public bool Equals(IEffectData other)
        {
            return other is FearRandomEffectData o && o.netID == netID;
        }

        public ChampionEffects Effect => ChampionEffects.Fear;
    }
}
