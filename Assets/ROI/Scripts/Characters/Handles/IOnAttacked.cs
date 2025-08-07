namespace ROI
{
    public interface IOnAttacked
    {
        void OnHit(ChampionData attacker, DamageDealtData damageDealtData);
    }
}