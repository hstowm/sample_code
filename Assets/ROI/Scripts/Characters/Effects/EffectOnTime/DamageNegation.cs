using ROI;

namespace ROI
{
    
}
public class DamageNegation : DisplayableEffect, IEffectSystem, IOnAttacked
{
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        champion.handles.OnAttacked.Add(this);
    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        champion.handles.OnAttacked.Remove(this);
        RemoveEffect();
    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
    }

    public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
    {
        damageDealtData.blockPercent += 1;
    }
}
