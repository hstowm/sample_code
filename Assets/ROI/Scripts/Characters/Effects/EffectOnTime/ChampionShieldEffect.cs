using System;
using System.Collections.Generic;

namespace ROI
{


	public class ChampionShieldEffect : DisplayableEffect, IEffectSystem
	{
		public float shield_hp;
		public bool isStack = false;
		private ChampionData target;
		public StatusData _data;
		public Action onShieldDestroy;
		
		public void ApplyEffect(ChampionData champion, StatusData arg)
		{
			target = champion;
			_data = arg;
			ApplyShield(champion, arg);
			ApplyVFX(champion);
		}

		private void ApplyShield(ChampionData champion, StatusData arg)
		{
			StatusParam current_param = arg.GetCurrentParam();

			if (current_param != null)
			{
				ApplyVFX(champion);
				float shieldBonus = 0;
				foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_param.param_list)
				{
					switch (entry.Key)
					{
						case StatusParamKeyWord.ShieldPercent:
							shieldBonus = (int)(entry.Value * champion.healthData.maxHealth);
							break;
						case StatusParamKeyWord.ShieldFlat:
							shieldBonus = entry.Value;
							break;
						default: break;
					}

					shieldBonus *= (1 + champion.specialStatData.abilityPower / 100);
					if (isStack)
					{
						shield_hp += shieldBonus;
					} else
					{
						shield_hp = shieldBonus;
					}
				}
				champion.shieldManager.AddNewShield(this);
				
			}
		}
		public void ReApplyEffect(ChampionData champion, StatusData arg)
		{
			arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
			if (!arg.MaxLevel)
			{
				arg.level++;
			}
			ApplyEffect(champion, arg);

		}
		public void RemoveEffect(ChampionData champion, StatusData arg)
		{
			shield_hp = 0;
			champion.shieldManager.shieldsOnChampion.Remove(this);
			ClearVFX();
			//Destroy(gameObject);
			onShieldDestroy?.Invoke();
			RemoveEffect();
		}


	}

}