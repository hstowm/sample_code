using ROI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessedEffect : DisplayableEffect, IEffectSystem
{
    private ChampionDamageText _championDamageText;
    private Coroutine handle;
    public int tick = 1;
    private float healthRemain = 0;
    private float AoERadius = 3;
    public void ApplyEffect(ChampionData champion, StatusData arg)
    {
        if (effectSound)
        {
            SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
        }
        arg.type = StatusData.EffectType.Buff;
        StatusParam current_level = arg.GetCurrentParam();

        if (_championDamageText == null)
        {
            _championDamageText = ChampionDamageText.instance;
        }
        foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
        {
            switch (entry.Key)
            {
                case StatusParamKeyWord.Bless:
                    {
                        handle = StartCoroutine(ApplyBlessed(champion, entry.Value, arg));
                        ApplyIcon(champion, arg.level + 1, ChampionEffects.Blessed, arg.remain_duration, arg.setting.duration);
                        ApplyVFX(champion);
                        break;
                    }
                case StatusParamKeyWord.BlessSlash:
                    {
                        //
                        ApplyBlessSlash(champion, entry.Value, arg);
                        break;
                    }
            }

        }

    }

    private IEnumerator ApplyBlessed(ChampionData champion, float value, StatusData arg)
    {
        float tick_percent = tick / arg.setting.duration;
        healthRemain = value;
        while (arg.remain_duration > 0)
        {
            float heal = value * tick_percent;
            healthRemain -= heal;
            int healData = (int)(heal * champion.healthData.maxHealth);
            StatTypeData damage_dealed = new StatTypeData
            {
                statType = StatTypes.Health,
                value = healData,
                valueType = StatValueTypes.Flat
            };
            champion.statModifier.ApplyModify(damage_dealed);
            _championDamageText.ShowHealDamage(champion, (int)healData);
            yield return new WaitForSeconds(tick);
        }

        yield return null;
    }

    public void ApplyBlessSlash(ChampionData champion, float value, StatusData statusData)
    {
        StopAllCoroutines();
        //ApplyVFX(champion);
        var healRegen = value + healthRemain;

        StatTypeData damage_dealed = new StatTypeData
        {
            statType = StatTypes.Health,
            value = healRegen,
            valueType = StatValueTypes.Percent
        };


        int actual_dmg = (int)(healRegen * champion.healthData.maxHealth);
        champion.statModifier.ApplyModify(damage_dealed);
        _championDamageText.ShowHealDamage(champion, (int)actual_dmg);
        foreach (var enemy in champion.enemies)
        {
            if (!enemy.IsDeath && Vector3.Distance(enemy.transform.position, champion.transform.position) <= AoERadius)
            {
                // Deal damage to enemy
                champion.attacker.AttackEnemy(enemy, actual_dmg, DamageSources.Effect, DamageTypes.Magic);
                GeneralEffectSystem.Instance.ApplyEffect(enemy,
                    new StatusData("PushBack", champion, champion.transform.position));
                // Push enemy back
            }
        }
        statusData.remain_duration = 0;
        statusData.remain_duration_unscaled = 0;
    }

    public void ReApplyEffect(ChampionData champion, StatusData arg)
    {
        arg.level++;
        
        arg.remain_duration = arg.setting.duration;
        StopCoroutine(handle);

        ApplyEffect(champion, arg);
    }

    public void RemoveEffect(ChampionData champion, StatusData arg)
    {
        RemoveIcon(champion, ChampionEffects.Blessed, arg.level + 1);
        StopCoroutine(handle);
        ClearVFX();
        RemoveEffect();
    }

    // Start is called before the first frame update

}
