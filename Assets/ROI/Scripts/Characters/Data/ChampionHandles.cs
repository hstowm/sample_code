using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
	public class ChampionHandles
	{
		const int ComponentCapacity = 16;

		public readonly List<IOnStartAlive> OnStartAlive = new(ComponentCapacity);

		/// <summary>
		/// 1. IOnStartAutoAttack(one time when start auto attack)
		/// 2. --> IStartAttackEvent
		/// 3. --> On Attack Event
		/// 4. --> IOnHitEnemy
		/// 5. --> IOnAttacked
		/// 6. --> IEndAttackEvent
		/// </summary>
		public readonly List<IOnStartAutoAttack> OnStartAutoAttacks = new(ComponentCapacity);
		public readonly List<IStartAttackEvent> OnStartAttackEvents = new(ComponentCapacity);
		public readonly List<IOnAttackEvent> OnAttackEvents = new(ComponentCapacity);
		public readonly List<IOnHitEnemy> OnHitEnemies = new(ComponentCapacity);
		public readonly List<IOnAttacked> OnAttacked = new(ComponentCapacity);

		public readonly List<IOnDead> OnDeads = new(ComponentCapacity);

		// on target changed
		public readonly List<IOnTargetChanged> OnTargetChangeds = new(ComponentCapacity);

		public readonly List<IOnStartMove> OnStartMoves = new(ComponentCapacity);
		public readonly List<IOnStopMove> OnStopMoves = new(ComponentCapacity);
		public readonly List<IOnStopAutoAttack> OnStopAttacks = new(ComponentCapacity);
		
		public readonly List<IOnUseCard> OnUseCards = new(ComponentCapacity);
		
		public readonly List<IOnDamaged> OnDamageds = new(ComponentCapacity);


		public ChampionHandles()
		{
			OnStartAutoAttacks.Clear();
			OnStartAttackEvents.Clear();
			OnAttackEvents.Clear();
			OnHitEnemies.Clear();
			OnAttacked.Clear();
			OnDeads.Clear();

			// on target changed
			OnTargetChangeds.Clear();

			OnStartMoves.Clear();
			OnStopMoves.Clear();
			OnStopAttacks.Clear();
			
			OnUseCards.Clear();
			OnDamageds.Clear();
		}
		
		public ChampionHandles(GameObject gameObject, bool inChildren = true) : this()
		{
			gameObject.Gets(ref OnStartAutoAttacks, inChildren);
			gameObject.Gets(ref OnAttackEvents, inChildren);
			gameObject.Gets(ref OnAttacked, inChildren);

			gameObject.Gets(ref OnStopAttacks, inChildren);
			gameObject.Gets(ref OnHitEnemies, inChildren);

			gameObject.Gets(ref OnStartAttackEvents, inChildren);
			gameObject.Gets(ref OnDeads, inChildren);
			gameObject.Gets(ref OnStartAlive, inChildren);

			gameObject.Gets(ref OnStopMoves);
			gameObject.Gets(ref OnStartMoves);

			gameObject.Gets(ref OnTargetChangeds);
			
			gameObject.Gets(ref OnUseCards);
			
			gameObject.Gets(ref OnDamageds);
		}
	}
}