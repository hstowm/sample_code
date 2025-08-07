using ROI;
using System.Collections.Generic;
using UnityEngine;

public class BaseDealDamageEffect : DisplayableEffect, IEffectSystem
{
	[SerializeField] private ChampionDamageText _championDamageText;
	public void ApplyEffect(ChampionData champion, StatusData arg)
	{
		if (_championDamageText == null)
		{
			_championDamageText = ChampionDamageText.instance;
		}

		StatusParam current_level = arg.GetCurrentParam();

		foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
		{
			switch (entry.Key)
			{
				case StatusParamKeyWord.NormalDamage:
					DealDamageToEnemy(arg.creator, champion, entry.Value, DamageSources.Ability, DamageTypes.Physic);
					break;
				case StatusParamKeyWord.MagicDamage:
					DealDamageToEnemy(arg.creator, champion, entry.Value, DamageSources.Ability, DamageTypes.Magic);
					break;
				case StatusParamKeyWord.DamagePercent:
					DealDamageToEnemy(arg.creator, champion, entry.Value, DamageSources.Ability, DamageTypes.True);
					break;
			}
		}
	}

	public void DealDamageToEnemy(ChampionData attacker, ChampionData attacked, float damage, DamageSources damageSources,DamageTypes damageTypes)
	{
		//TODO show deal damage effect
		ApplyVFX(attacked);
		// Logs.Info($"{attacker.netId} Attack Enemy {attacked.netId} With Damage: {damage}");
		attacker.attacker.AttackEnemy(attacked, damage, damageSources, damageTypes);
	}
	

	//Simply destroy or recyle this effect system
	public void RemoveEffect(ChampionData champion, StatusData arg)
	{
		ClearVFX();
		RemoveEffect();
	}

	public void ReApplyEffect(ChampionData champion, StatusData arg)
	{

	}

	// Start is called before the first frame update

}