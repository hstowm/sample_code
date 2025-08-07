using System;

namespace ROI
{
	public class DealAdditionalDamageOnCrit : IOnHitEnemy, IEquatable<DealAdditionalDamageOnCrit>
	{
		private string id = Guid.NewGuid().ToString();
		private ChampionData _championData;

		public DealAdditionalDamageOnCrit(ChampionData championData)
		{
			_championData = championData;
		}
		public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
		{
			if (damageDealtData.damageSource == DamageSources.BasicAttack && damageDealtData.isCrit)
			{
				foreach (var champion in enemy.allies)
				{
					if (!champion.IsDeath)
					{
						// Deal the same damage to additional damage
						_championData.attacker.AttackEnemy(enemy, damageDealtData.attackDamage, DamageSources.Effect, DamageTypes.Physic);
						break;
					}
				}
			}
		}
		public bool Equals(DealAdditionalDamageOnCrit other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return id == other.id;
		}
		public override int GetHashCode()
		{
			return (id != null ? id.GetHashCode() : 0);
		}
	}
}