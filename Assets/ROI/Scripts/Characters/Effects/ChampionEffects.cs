namespace ROI
{
    [System.Flags]
    public enum ChampionEffects : uint
    {
        None = 0,
        KnockDown = 1,
        KnockUp = 1 << 1,
        KnockBack = 1 << 2,
        Pull = 1 << 3,
        Stun = 1 << 4,
        Fear = 1 << 5,
        Blind = 1 << 6,
        Silence = 1 << 7,
        Chilled = 1 << 8,
        Frozen = 1 << 9,
        Poisoned = 1 << 10,
        Vulnerable = 1 << 11,
        Darkness = 1 << 12,
        Frenzied = 1 << 13,
        Engulfing = 1 << 14,
        Blessed = 1 << 15,
        ModifyStat = 1 << 16
    }
}