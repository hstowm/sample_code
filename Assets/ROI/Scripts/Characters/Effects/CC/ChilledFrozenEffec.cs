using Mirror;
using ROI;
using System.Collections.Generic;
using UnityEngine;

public class ChilledFrozenEffect : DisplayableEffect,IEffectSystem, IPauseHandle
{

    public GameObject VFX_Chilled_prefab;
    public GameObject VFX_Frozen_prefab;
    private float _atk_slow;
    private float _move_slow;
    
    
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.type = StatusData.EffectType.DeBuff;
        StatusParam current_level = arg.GetCurrentParam();
        
        foreach(KeyValuePair <StatusParamKeyWord, float> entry in current_level.param_list)
        {
            if(entry.Key == StatusParamKeyWord.Slow)
            {
                ApplySlow(champion, entry.Value);            
            }
            if (entry.Key == StatusParamKeyWord.AttackSlow)
            {
                ApplyAttackSlow(champion, entry.Value);
            }
            if(entry.Key == StatusParamKeyWord.Frozen)
            {
                ClearVFX();
                RemoveIcon(champion,ChampionEffects.Chilled, arg.level + 1);
                ApplyFrozen(champion,arg);
                ApplyIcon(champion,arg.level+1,ChampionEffects.Frozen, arg.remain_duration, arg.setting.duration);
                ShowFrozenEffect();
                ApplyVFX(champion);
                return;
            }
            ApplyIcon(champion, arg.level+1,ChampionEffects.Chilled, arg.remain_duration, arg.setting.duration);
            ShowChilledEffect();
            ApplyVFX(champion);
        }
    }
    private void ApplyFrozen(ChampionData champion, StatusData arg)
    {
        Debug.Log($"Pause champion on frozen {champion.name}");
        champion.animatorNetwork.animator.speed = 0;
        champion.controller.Pause(this);
    }

    [ClientRpc]
    public void SetVFX(ChampionEffects t)
    {
        if(t == ChampionEffects.Frozen)
        {
            VFX_prefab = VFX_Frozen_prefab;
            return;
        }

        VFX_prefab = VFX_Chilled_prefab;
    }


    private void ApplySlow(ChampionData champion, float slow)
    {
        StatTypeData move_slow = new StatTypeData
        {
            statType = StatTypes.MoveSpeed,
            value = -slow,
            valueType = StatValueTypes.Flat
        };
        _move_slow = slow;
        champion.statModifier.ApplyModify(move_slow);
    }

    private void ApplyAttackSlow(ChampionData champion, float slow)
    {
        StatTypeData atk_slow = new StatTypeData
        {
            statType = StatTypes.AttackSpeed,
            value = -slow,
            valueType = StatValueTypes.Flat
        };
        _atk_slow = slow;
        champion.statModifier.ApplyModify(atk_slow);
    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        
        arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
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
        RemoveIcon(champion,ChampionEffects.Chilled, arg.level + 1);
        RemoveIcon(champion, ChampionEffects.Frozen, arg.level + 1);
        ClearVFX();
        RestoreStat(champion);
        Debug.Log("Remove frozen");
        champion.animatorNetwork.animator.speed = 1;
        IsPaused = false;
        RemoveEffect();
    }

    private void RestoreStat(ChampionData champion)
    {
        StatTypeData move_slow = new StatTypeData
        {
            statType = StatTypes.MoveSpeed,
            value = _move_slow,
            valueType = StatValueTypes.Flat
        };
        champion.statModifier.ApplyModify(move_slow);

        StatTypeData atk_slow = new StatTypeData
        {
            statType = StatTypes.AttackSpeed,
            value = _atk_slow,
            valueType = StatValueTypes.Flat
        };

        champion.statModifier.ApplyModify(atk_slow);
    }
    [ClientRpc]
    public void ShowChilledEffect()
    {
        VFX_prefab = VFX_Chilled_prefab;
    }
    [ClientRpc]
    public void ShowFrozenEffect()
    {
        VFX_prefab = VFX_Frozen_prefab;
    }
    
    public bool IsPaused { get; set; }
}
