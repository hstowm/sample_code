
using ROI;

public class BaseDisableActionEffect : DisplayableEffect,IEffectSystem, IPauseHandle
{
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.type = StatusData.EffectType.DeBuff;
        champion.controller.SetTrigger(AnimHashIDs.Stun);
        ApplyIcon(champion,1, ChampionEffects.Stun, arg.remain_duration, arg.setting.duration);
      //  champion.AddEffect(ChampionEffects.Stun);
        ApplyVFX(champion);
        champion.controller.Pause(this);
    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        RemoveIcon(champion,ChampionEffects.Stun, arg.level + 1);
       // champion.RemoveEffect(ChampionEffects.Stun);
        ClearVFX();
        IsPaused = false;
        RemoveEffect();
        champion.controller.SetTrigger(AnimHashIDs.Idle);
    }

    public bool IsPaused
    {
        get; set;
    }
}
