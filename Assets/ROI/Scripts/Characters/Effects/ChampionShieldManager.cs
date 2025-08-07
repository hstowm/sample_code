using System;
using System.Collections.Generic;
using Mirror;

namespace ROI
{
	public class ChampionShieldManager : NetworkBehaviour, IOnDamaged, IOnAttacked
	{
		private ChampionData _championData;
		public List<ChampionShieldEffect> shieldsOnChampion = new List<ChampionShieldEffect>();

		public float ShieldValue {
			get {
				float shieldVal = 0;
				foreach (var championShield in shieldsOnChampion)
				{
					shieldVal += championShield.shield_hp;
				}
				return shieldVal;
			}
		}
        private void Start()
        {
            
			_championData = GetComponent<ChampionData>();
		}

		public void AddNewShield(ChampionShieldEffect shieldEffect)
		{
			if (!shieldsOnChampion.Contains(shieldEffect))
				shieldsOnChampion.Add(shieldEffect);
			shieldsOnChampion.Sort((effect, championShieldEffect) =>
			{
				if (effect._data.remain_duration > championShieldEffect._data.remain_duration)
				{
					return 1;
				} else if (Math.Abs(effect._data.remain_duration - championShieldEffect._data.remain_duration) < 0.01f)
				{
					return 0;
				} else
				{
					return -1;
				}
			});
			DisplayShield(ShieldValue);
		}
		public void OnDamaged(ChampionData attacker, DamageDealtData damageDealtData)
		{

			var shieldTakenDamage = damageDealtData.shieldTaken;
			foreach (var championShield in shieldsOnChampion)
			{
				if ( championShield.shield_hp > shieldTakenDamage)
				{
					championShield.shield_hp -= shieldTakenDamage;
					break;
				}
				
				shieldTakenDamage -= championShield.shield_hp;
				championShield.shield_hp = 0;
				championShield._data.remain_duration = championShield._data.remain_duration_unscaled = 0;
			}

			if (isServer)
			{
				DisplayShield(ShieldValue);
			}
		}

		public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
		{
			damageDealtData.targetShield = ShieldValue;
		}


		[ClientRpc]
		private void DisplayShield(float shield_hp)
		{
			if (_championData == null) return;
			float _percent = shield_hp / _championData.healthData.maxHealth;

			ChampionHealthBar.instance.healthBars[_championData.netId].UpdatePercentShield(_percent);
		}
	}

}