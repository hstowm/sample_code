using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace ROI
{
	class ChampionAttacker : NetworkBehaviour, IAttacker
	{
		private ChampionData _championData;

		private void Awake()
		{
			_championData = GetComponent<ChampionData>();
		}

		/// <summary>
		/// Hit an enemy by auto attack (basic attack)
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="damageDealtData"></param>
		[Server]
		public void AttackEnemy(ChampionData enemy, out DamageDealtData damageDealtData)
		{
			var attackData = _championData.attackData;
			damageDealtData = new DamageDealtData(DamageSources.BasicAttack, attackData, enemy.defenseData);
			if(_championData.HasUltimateEnergy())
			_championData.AddBonusUltimateEnergy(0.67f);
			if (isServer == false)
				return;

			damageDealtData.dodgeChancePercent = enemy.specialStatData.dodgeChance;
			damageDealtData.damageTaken = enemy.specialStatData.damageTaken;
			damageDealtData.critDamageChance = attackData.critDamageChance;
			damageDealtData.critDamageBonus = attackData.critDamage;

			RpcAttackEnemy(enemy, damageDealtData);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="abilityDamage"></param>
		/// <param name="damageSource"></param>
		/// <param name="damageType"></param>
		[Server]
		public void AttackEnemy(ChampionData enemy, float abilityDamage, DamageSources damageSource, DamageTypes damageType)
		{
			if (isServer == false)
				return;

			if (damageSource.IsBasicAttack())
			{
				Logs.Error("Cant Attack Enemy with damage source is basic");
				return;
			}

			var damageDealtData = new DamageDealtData(damageSource, damageType, _championData.attackData.damage, enemy.defenseData);
			damageDealtData.critDamageBonus = _championData.attackData.critDamage;

			damageDealtData.abilityDamage = abilityDamage;
			damageDealtData.damageTaken = enemy.specialStatData.damageTaken;
			damageDealtData.dodgeChancePercent = enemy.specialStatData.dodgeChance;
			damageDealtData.abilityPower = _championData.specialStatData.abilityPower;

			if (damageDealtData.isAbility)
			{
				damageDealtData.critDamageChance = _championData.specialStatData.critChanceOnAbility;
			}

			RpcAttackEnemy(enemy, damageDealtData);
		}

		/// <summary>
		/// Dealt a damaged which have been calculated
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="damageDealtData"></param>
		[Server]
		public void ApplyDamageDealt(ChampionData enemy, DamageDealtData damageDealtData) => RpcAttackEnemy(enemy, damageDealtData);


		/// <summary>
		/// RPC Attack To All Client
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="damageDealtData"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining), ClientRpc]
		private void RpcAttackEnemy(ChampionData enemy, DamageDealtData damageDealtData)
		{
			var onHitEnemies = _championData.handles.OnHitEnemies.ToArray();
			// champion hit enemy
			foreach (var hitEnemy in onHitEnemies)
			{
				hitEnemy.OnHitEnemy(enemy, damageDealtData);
			}			
			// enemy have been attacked
			var onAttackeds = enemy.handles.OnAttacked.ToArray();
			foreach (var onAttacked in onAttackeds)
			{
				onAttacked.OnHit(_championData, damageDealtData);
			}

			if (isServer == false)
				return;

			// calculate last heath
			damageDealtData.CalculateFinalDamage(isServer);

			// callback when damage have been calculated
			foreach (var onFinalDamage in damageDealtData.hooks.OnFinalDamages)
			{
				onFinalDamage.OnDamageCalculated(damageDealtData);
			}

			ReduceHealth(enemy, damageDealtData.finalDamage);

			RpcOnDamaged(enemy, damageDealtData);

		}

		[ClientRpc]
		private void RpcOnDamaged(ChampionData enemy, DamageDealtData damageDealtData)
		{
			// Logs.Info("healthReduction: " + healthReduction);
			var onDamageds = enemy.handles.OnDamageds.ToArray();
			foreach (var onDamaged in onDamageds)
			{
				onDamaged.OnDamaged(_championData, damageDealtData);
			}

			
#if UNITY_EDITOR
			if (isServer == false)
				return;

			//Logs.Info($"After Health Reduction: {damageDealtData.GetHealthReduction()}. Zero Damage: {damageDealtData.zeroDamage}. Shield: {damageDealtData.shieldHealthReduction}");
			if (Math.Abs(damageDealtData.finalDamage - damageDealtData.GetFinalDamage()) > 0.001f)
			{
				Logs.Warning($"Dont Change DamageDealtData When OnDamaged: Old Health Reduction:{damageDealtData.finalDamage} - new: {damageDealtData.GetFinalDamage()}");
				// Logs.Warning("AFTER....");
				// damageDealtData.Print();
			}
			
#endif
			
		}

		/// <summary>
		/// Reduce Health enemy
		/// </summary>
		/// <param name="target"></param>
		/// <param name="healthReduction"></param>
		[Server]
		private void ReduceHealth(ChampionData target, float healthReduction)
		{
			if (target.imperishable || healthReduction == 0 || target.IsDeath)
				return;

			target.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, -healthReduction));

			if (target.IsDeath)
				target.controller.SetDeath();
		}

	}
}