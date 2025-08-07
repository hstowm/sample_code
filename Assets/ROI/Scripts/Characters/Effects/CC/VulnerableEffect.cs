using Mirror;
using NSubstitute;
using ROI;
using System.Collections.Generic;
using UnityEngine;

public class VulnerableEffect : DisplayableEffect, IEffectSystem
{

    public float _vul;
    private const int MaxLevel = 2;
    public GameObject darknessVFX;
    public GameObject vulnerableVFX;
    private DealAdditionalDamageToAlliesAround _dealAdditionalDamageToAlliesAround;
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.type = StatusData.EffectType.DeBuff;
        StatusParam current_level = arg.GetCurrentParam();
        foreach (var statParam in current_level.base_stat_params)
        {
            var statApply = new StatTypeData(statParam.Key, statParam.Value.value, statParam.Value.valueType);
            champion.statModifier.ApplyModify(statApply);
            _vul += statParam.Value.value;
        }
        foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
        {
            switch (entry.Key)
            {
                case StatusParamKeyWord.Vulnerable:
                {
                    ShowVulnerableVFX();
                    ApplyIcon(champion, arg.level+1, ChampionEffects.Vulnerable, arg.remain_duration, arg.setting.duration);
                    SetVFX(ChampionEffects.Vulnerable);
                    ApplyVFX(champion);
                    break;
                }
                case StatusParamKeyWord.Darkness:
                {
                    ApplyIcon(champion, 1, ChampionEffects.Darkness, arg.remain_duration, arg.setting.duration);
                    ApplyDarkness(champion);
                    break;
                }
            }
        }
    }


    [ClientRpc]
    public void SetVFX(ChampionEffects t)
    {
        if(t == ChampionEffects.Vulnerable)
        {
            VFX_prefab = vulnerableVFX;
        }
    }

    

    private void ApplyDarkness(ChampionData champion)
    {
        //ClearVFX();
        RemoveIcon(champion, ChampionEffects.Vulnerable, 1);
        ClearVFX();
        ShowDarknessVFX();
        ApplyVFX(champion);
        if (_dealAdditionalDamageToAlliesAround == null)
        {
            _dealAdditionalDamageToAlliesAround = new DealAdditionalDamageToAlliesAround(champion);
            champion.handles.OnAttacked.Add(_dealAdditionalDamageToAlliesAround);
        }

    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.remain_duration = arg.setting.duration;
        if (!arg.MaxLevel)
        {
            arg.level++;
            ApplyEffect(champion, arg);
        }
        else
        {
            if (effectSound)
            {
                SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
            }
        }
    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        ChampionEffects iconEffRemove = arg.level > 0 ? ChampionEffects.Darkness : ChampionEffects.Vulnerable;
        RemoveIcon(champion, iconEffRemove, arg.level + 1);
        ClearVFX();
        if (champion.handles.OnAttacked.Contains(_dealAdditionalDamageToAlliesAround))
            champion.handles.OnAttacked.Remove(_dealAdditionalDamageToAlliesAround);
        _dealAdditionalDamageToAlliesAround = null;
        RestoreStat(champion);
        RemoveEffect();
    }

    private void RestoreStat(ChampionData champion)
    {
        StatTypeData vulnerable = new StatTypeData
        {
            statType = StatTypes.DamageTaken,
            value = -_vul,
            valueType = StatValueTypes.Flat
        };
        _vul = 0;
        champion.statModifier.ApplyModify(vulnerable);
    }
    [ClientRpc]
    public void ShowVulnerableVFX()
    {
        VFX_prefab = vulnerableVFX;
    }
    [ClientRpc]
    public void ShowDarknessVFX()
    {
        VFX_prefab = darknessVFX;
    }
}
