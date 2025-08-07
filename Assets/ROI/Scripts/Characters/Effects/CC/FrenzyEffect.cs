using System.Collections.Generic;
using ROI;

public class FrenzyEffect : DisplayableEffect,IEffectSystem
{
    
    private DealAdditionalDamageOnCrit _dealAdditionalDamageOnCrit;
    private List<StatTypeData> _statTypeDatas = new List<StatTypeData>();
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        champion.currentEffect.AddEffect(ChampionEffects.Frenzied);
        arg.type = StatusData.EffectType.Buff;
        StatusParam current_level = arg.GetCurrentParam();

        foreach (var param in current_level.base_stat_params)
        {
            var statApply = new StatTypeData(param.Key, param.Value.value, param.Value.valueType);
            champion.statModifier.ApplyModify(statApply);
            _statTypeDatas.Add(statApply);
        }
        foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
        {
            switch (entry.Key)
            {
                case StatusParamKeyWord.FrenzyAwaken:
                {
                    ApplyFrenzyPassive(champion);
                    break;
                }
            }
        }
        
        ApplyIcon(champion, arg.level+1, ChampionEffects.Frenzied, arg.remain_duration, arg.setting.duration);

        ApplyVFX(champion);
    }
    public void ApplyFrenzyPassive(ChampionData championData)
    {
        if(_dealAdditionalDamageOnCrit == null)
            _dealAdditionalDamageOnCrit= new DealAdditionalDamageOnCrit(championData);
        championData.handles.OnHitEnemies.Add(_dealAdditionalDamageOnCrit);
    }
    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.remain_duration_unscaled = arg.remain_duration = arg.setting.duration;
        if (arg.MaxLevel)
        {
            if (effectSound)
            {
                SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
            }
            return;
        };
        arg.level++;
        ApplyEffect(champion, arg);
    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        RemoveIcon(champion, ChampionEffects.Frenzied, arg.level + 1);
        RestoreStat(champion);
        champion.currentEffect.RemoveEffect(ChampionEffects.Frenzied);
        ClearVFX();
        if (champion.handles.OnHitEnemies.Contains(_dealAdditionalDamageOnCrit))
            champion.handles.OnHitEnemies.Remove(_dealAdditionalDamageOnCrit);
        _dealAdditionalDamageOnCrit = null;
        _statTypeDatas.Clear();
        RemoveEffect();
    }

    private void RestoreStat(ChampionData champion)
    {
        foreach (var statType in _statTypeDatas)
        {
            champion.statModifier.ApplyModify(new StatTypeData(statType.statType, -statType.value, statType.valueType));
        }
    }
    public bool IsPaused { get; set; }
}
