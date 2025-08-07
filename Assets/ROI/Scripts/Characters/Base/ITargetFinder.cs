namespace ROI
{
    public interface ITargetFinder
    {
        bool FindTarget(out ChampionData target);
    }
}