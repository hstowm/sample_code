namespace ROI
{
    public enum KnockDirections { Down,Up, Back, Pull }
    
    public class KnockEffectData:IEffectData
    {
        public ChampionEffects eff;
        public ChampionData impact;
        public KnockDirections direction;
        public float lastSpeedAtk, lastMoveSpeed;
        public float height;
        public float powerKnock;
        public float range;
        public float time;
        public bool Equals(IEffectData other)
        {
            return false;
        }

        public ChampionEffects Effect { get; }
    }
}