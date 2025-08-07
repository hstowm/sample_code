using System.Collections.Generic;
using ROI;
using UnityEngine;

public class EngulfingEffect : DisplayableEffect, IEffectSystem
{
	private ChampionDamageText _championDamageText;
	private DealAdditionalDamageAoe dealAdditionalDamageAoe;
	private float damageOnEnd = 2;
	private float radius = 3;
	public void ApplyEffect(ChampionData champion, StatusData arg)
	{
		arg.type = StatusData.EffectType.Buff;
		StatusParam current_level = arg.GetCurrentParam();
		champion.currentEffect.AddEffect(ChampionEffects.Engulfing);
		foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
		{
			if (entry.Key == StatusParamKeyWord.DamageAoe)
			{
				ApplyIcon(champion, arg.level + 1, ChampionEffects.Engulfing, arg.remain_duration, arg.setting.duration);
				Debug.Log("Engulfing Level: " + arg.level + 1);
				ApplyEngulf(champion, entry.Value, arg);
				ApplyVFX(champion);
			}
		}
	}
	public void RemoveEffect(ChampionData champion, StatusData arg)
	{
		RemoveIcon(champion, ChampionEffects.Engulfing, arg.level + 1);
		ClearVFX();
		champion.currentEffect.RemoveEffect(ChampionEffects.Engulfing);
		if (champion.handles.OnHitEnemies.Contains(this.dealAdditionalDamageAoe))
			champion.handles.OnHitEnemies.Remove(dealAdditionalDamageAoe);
		dealAdditionalDamageAoe = null;
		if (arg.MaxLevel)
		{
			// Deal damage to all allie
			foreach (var enemy in champion.enemies)
			{
				if (!enemy.IsDeath)
				{
					champion.attacker.AttackEnemy(enemy, champion.attackData.damage * damageOnEnd, DamageSources.Effect, DamageTypes.Magic);
				}
			}
		}
		RemoveEffect();
		//Destroy(gameObject);
	}
	public void ReApplyEffect(ChampionData champion, StatusData arg)
	{
		arg.remain_duration = arg.setting.duration;
		arg.remain_duration_unscaled = arg.setting.duration;
		Debug.Log("Remain duration " + arg.remain_duration);
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
	public void ApplyEngulf(ChampionData championData, float value, StatusData statusData)
	{
		if (dealAdditionalDamageAoe == null)
		{
			dealAdditionalDamageAoe = new DealAdditionalDamageAoe(championData, radius);
			championData.handles.OnHitEnemies.Add(dealAdditionalDamageAoe);
		}
		dealAdditionalDamageAoe.damageBonusPercent += value;
	}
}