namespace ROI
{
    public interface IEffectSystem
    {
        void ApplyEffect(ChampionData champion, StatusData arg);
        void RemoveEffect(ChampionData champion, StatusData arg);

        void ReApplyEffect(ChampionData champion, StatusData arg);
    }
    
}