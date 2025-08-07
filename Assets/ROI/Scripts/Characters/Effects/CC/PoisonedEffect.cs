using ROI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonedEffect : DisplayableEffect, IEffectSystem
{
	// Start is called before the first frame update
	private ChampionDamageText _championDamageText;
	private Coroutine handle;
	private float _remain_missing_heath = 0;
	public int tick;
	private float poisonValue;
	public void ApplyEffect(ChampionData champion, StatusData arg)
	{
		arg.type = StatusData.EffectType.DeBuff;
		StatusParam current_level = arg.GetCurrentParam();
		if (_championDamageText == null)
		{
			_championDamageText = ChampionDamageText.instance;
		}

		foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
		{
			if (entry.Key == StatusParamKeyWord.Poison)
			{
				_remain_missing_heath += entry.Value;
				poisonValue = _remain_missing_heath;
				if (handle == null)
				{
					handle = StartCoroutine(ApplyPoisoned(champion, _remain_missing_heath, arg, tick));
				}
				ApplyIcon(champion, arg.level + 1, ChampionEffects.Poisoned, arg.remain_duration, arg.setting.duration);
				ApplyVFX(champion);
			}

			if (entry.Key == StatusParamKeyWord.Poison_Instant)
			{
				ApplyInstantPoison(entry.Value + _remain_missing_heath, champion, arg);
			}
		}
	}

	private void ApplyInstantPoison(float value, ChampionData champion, StatusData arg)
	{
		arg.remain_duration = arg.remain_duration_unscaled = 0;
		_remain_missing_heath = 0;
		int actual_dmg = (int)(value * champion.healthData.maxHealth);
		arg.creator.attacker.AttackEnemy(champion, actual_dmg, DamageSources.Effect, DamageTypes.True);
		RemoveIcon(champion, ChampionEffects.Poisoned, arg.level + 1);
		ClearVFX();
	}

	private IEnumerator ApplyPoisoned(ChampionData champion, float _value, StatusData arg, int tick)
	{
		Debug.Log(_value);
		float tick_percent = tick / arg.setting.duration;
		while (arg.remain_duration > 0)
		{
			float reduce_percent = poisonValue * tick_percent;
			// Debug.Log("value reduce" + reduce_percent);
			_remain_missing_heath -= reduce_percent;

			int actual_dmg = (int)(reduce_percent * champion.healthData.maxHealth);

			arg.creator.attacker.AttackEnemy(champion, actual_dmg, DamageSources.Effect, DamageTypes.True);

			yield return new WaitForSeconds(tick);
		}
		handle = null;
		yield return null;
	}

	public void ReApplyEffect(ChampionData champion, StatusData arg)
	{

		arg.remain_duration = arg.setting.duration;
		if (arg.MaxLevel)
		{
			if (effectSound)
			{
				SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
			}
			return;
		}
		arg.level++;
		//StopCoroutine(handle);

		ApplyEffect(champion, arg);
	}

	public void RemoveEffect(ChampionData champion, StatusData arg)
	{
		RemoveIcon(champion, ChampionEffects.Poisoned, arg.level + 1);
		if (handle != null)
			StopCoroutine(handle);
		handle = null;
		ClearVFX();
		RemoveEffect();
		//Destroy(gameObject);
	}
	// Start is called before the first frame update

}