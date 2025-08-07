using System;
using ROI;
using UnityEngine;

public class DealAdditionalDamageAoe : IOnHitEnemy, IOnFinalDamage, IEquatable<DealAdditionalDamageAoe>
{
	public string uuid = Guid.NewGuid().ToString();
	private ChampionData _championData;
	private ChampionData _enemy;
	private float aoERadius = 3;
	public float damageBonusPercent;
	public DealAdditionalDamageAoe(ChampionData championData, float radius)
	{
		_championData = championData;
		aoERadius = radius;
	}
	public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
	{
		if (damageDealtData.damageSource != DamageSources.BasicAttack) return;
		this._enemy = enemy;
		foreach (var champion in enemy.allies)
		{
			if ( !champion.IsDeath && champion.netId != enemy.netId && Vector3.Distance(champion.transform.position, enemy.transform.position) <= aoERadius)
			{
				_championData.attacker.AttackEnemy(champion, damageDealtData.attackDamage * damageBonusPercent, DamageSources.Effect, DamageTypes.Magic);
			}
		}
		damageDealtData.hooks.OnFinalDamages.Add(this);
	}

	public bool Equals(DealAdditionalDamageAoe other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return uuid == other.uuid;
	}

	public void OnDamageCalculated(DamageDealtData damageDealtData)
	{
		if (damageDealtData.damageSource != DamageSources.BasicAttack) return;
		foreach (var champion in _enemy.allies)
		{
			if ( !champion.IsDeath && champion.netId != _enemy.netId && Vector3.Distance(champion.transform.position, _enemy.transform.position) <= aoERadius)
			{
				_championData.attacker.AttackEnemy(champion, damageDealtData.finalDamage * damageBonusPercent, DamageSources.Effect, DamageTypes.Magic);
			}
		}
	}
}