using System.Collections.Generic;
using Mirror;
using ROI;

public class ApplyStatEffect : NetworkBehaviour, IEffectSystem
{
    private List<StatTypeData> _statTypeDatas = new List<StatTypeData>();
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        StatusParam current_level = arg.GetCurrentParam();
        foreach (var entry in current_level.base_stat_params)
        {
            if (entry.Key is StatTypes.AttackSpeed or StatTypes.MoveSpeed)
            {
                arg.type = entry.Value.value > 0 ? StatusData.EffectType.Buff : StatusData.EffectType.DeBuff;
            }
            var statApply = new StatTypeData(entry.Key, entry.Value.value, entry.Value.valueType);
            champion.statModifier.ApplyModify(statApply);
            _statTypeDatas.Add(statApply);
        }

    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        foreach (var statType in _statTypeDatas)
        {
            champion.statModifier.ApplyModify(new StatTypeData(statType.statType, -statType.value, statType.valueType));
        }

        _statTypeDatas.Clear();
        gameObject.Recycle();
    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        //ApplyEffect(champion, arg);
    }

}
