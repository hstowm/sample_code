using UnityEngine;
namespace ROI
{
	/// <summary>
	/// Champion Life Steal Special Stat
	/// </summary>
	class ChampionLifeSteal : IOnHitEnemy, IOnFinalDamage
	{
		private readonly ChampionData _championData;
		private readonly ChampionDamageText _championDamageText;

		public ChampionLifeSteal(ChampionData championData, ChampionDamageText championDamageText)
		{
			_championData = championData;
			_championDamageText = championDamageText;

			_championData.handles.OnHitEnemies.Add(this);
		}

		public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
		{
			if (_championData.specialStatData.lifeSteal > 0)
			{
				damageDealtData.hooks.OnFinalDamages.Add(this);
			}
		}
		
		public void OnDamageCalculated(DamageDealtData damageDealtData)
		{
			if(damageDealtData.isDodge)
				return;
			
			var health = Mathf.FloorToInt(_championData.specialStatData.lifeSteal * damageDealtData.finalDamage);
			
			// Logs.Info($"Health: {health} On Life Steal: {_championData.specialStatData.lifeSteal}. Final Damage: {damageDealtData.finalDamage}.");
			
			_championData.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, health));
			
			_championDamageText.ShowHealDamage(_championData, health);
		}
	}
}