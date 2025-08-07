namespace ROI
{
    public interface IEffectApplier
    {
    }

    public interface IEffectApplier<in T> : IEffectApplier
    {
        void ApplyEffect(ChampionData championData, T arg);
    }

    public interface IEffectRemover<in T> : IEffectApplier
    {
        void RemoveEffect(ChampionData championData, T arg);
    }
}