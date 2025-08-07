using System.Runtime.CompilerServices;
using EPOOutline;
using UnityEngine;
using Mirror;
using UnityFx.Outline;

namespace ROI
{
	/// <summary>
	/// Composite Component. The Logic between champion's components
	/// </summary>
	[RequireComponent(typeof(ChampionMovement))]
	[RequireComponent(typeof(ChampionData))]
	[RequireComponent(typeof(ChampionAutoAttackSpeed))]
	class ChampionController : NetworkBehaviour, IController
	{
		private ChampionData _championData;
		private ChampionMovement _championMovement;
		private ChampionMoveSpeed _championMoveSpeed;
		private ChampionAutoAttackSpeed _championAutoAttackSpeed;
		private Outlinable _outlinable;
		private ChampionPauseHandle _championPauseHandle;

		private float _timeRemainingUpdate;
		private float _healthRegenTiming;
		private ChampionTargetSwitcher _championTargetSwitcher;

		// private MapSystem _mapSystem;

		private void Awake()
		{
			// _mapSystem = FindObjectOfType<MapSystem>(true);
			_championData = GetComponent<ChampionData>();

			_championMovement = GetComponent<ChampionMovement>();

			_championAutoAttackSpeed = GetComponent<ChampionAutoAttackSpeed>();
			_championMoveSpeed = GetComponent<ChampionMoveSpeed>();

			_championTargetSwitcher = new ChampionTargetSwitcher(_championData);
			_outlinable = GetComponent<Outlinable>();

			_championPauseHandle = new ChampionPauseHandle(_championData, this);

			_championData.statModifier = new ChampionStatModifier(_championData, _championMoveSpeed, _championAutoAttackSpeed);
		}

		public void SetOutline(bool active)
		{
			//if (outlineBehaviour != null && outlineBehaviour.enabled != active)
			//outlineBehaviour.SetActive(_championData.IsDeath == false && active);

			_outlinable.OutlineParameters.Enabled = _championData.IsDeath == false && active;
		}

		public void SetOutlineColor(Color color)
		{
			// if (outlineBehaviour != null)
			// 	outlineBehaviour.OutlineColor = color;
			_outlinable.OutlineParameters.Color = color;
		}
		public void ResetHexPosition()
		{
			_championMovement.InitHexPosition();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining), Server]
		private bool IsUpdateRate()
		{
			if (_timeRemainingUpdate > 0)
			{
				_timeRemainingUpdate -= Time.deltaTime;
				return true;
			}

			_timeRemainingUpdate += _championData.updateRate;
			return false;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining), Server]
		private void UpdateHealthRegen()
		{
			if (_championData.specialStatData.healthRegen <= 0)
				return;

			_healthRegenTiming += Time.deltaTime;
			if (_healthRegenTiming >= 1)
			{
				_championData.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, _championData.specialStatData.healthRegen));
				_healthRegenTiming -= 1;
			}
		}

		/// <summary>
		/// Update/Check states (attack and moving) in a frame
		/// </summary>
		private void Update()
		{
			// current champion is Death Or Pause Or Not IsServer
			if (_championData.IsDeath || _championData.state == ChampionStates.Stopped || isServer == false)
				return;

			UpdateHealthRegen();

			if (IsUpdateRate())
				return;

			if (_championPauseHandle.UpdatePauseState())
			{
				// On Handle Paused
				_championData.state = ChampionStates.Paused;
				return;
			}

			// check has target
			var state = _championData.state;

			// dont have target
			if (_championData.targetFinder.FindTarget(out var target) == false)
			{
				// stop current state
				_championData.target = null;
				if (state == ChampionStates.Moving)
				{
					//StartMove();
					//_championMovement.MoveToTarget();
					return;
				}

				if (state != ChampionStates.None)
				{
					Logs.Info($"Stop ALL with Current State: {state}");
					StopAll();
					return;
				}

				return;
			}

			// set target for champion
			_championTargetSwitcher.SetTarget(target);

			// Is In Attack Range target in range
			if (_championMovement.InAttackRange == false)
			{
				// stop attack when target not in range
				if (state == ChampionStates.Attacking)
				{
					StopAutoAttack();
					return;
				}

				// continue move to target
				if (state == ChampionStates.Moving)
				{
					_championMovement.MoveToTarget();
					return;
				}

				// current is not moving
				StartMove();
				return;
			}

			// var angle = _championMovement.RotationToTarget();

			// have target and current is attacking
			if (state == ChampionStates.Attacking)
				return;

			// stop move when current is moving
			if (state == ChampionStates.Moving)
			{
				StopMove();
				return;
			}

			// start attack when current is in attack angle
			// if (_championData.attackData.startAttackAngle >= angle)
			_championMovement.RotationToTarget();
			StartAutoAttack();
		}

		/// <summary>
		/// Stop Current States and change current state it to none
		/// </summary>
		public void StopAll()
		{
			StopMove();
			StopAutoAttack();
			_championData.state = ChampionStates.None;
			_championData.target = null;
		}

		[ClientRpc]
		private void StopMove()
		{
			//Logs.Info("Stop MOVE");
			var stopMoves = _championData.handles.OnStopMoves.ToArray();
			foreach (var onStopMove in stopMoves)
			{
				onStopMove.OnStopMove();
			}

			if (isServer)
				_championData.state = ChampionStates.None;
		}

		[ClientRpc]
		public void StartMove()
		{
			var startMoves = _championData.handles.OnStartMoves.ToArray();
			foreach (var onStartMove in startMoves)
			{
				onStartMove.OnStartMove();
			}

			if (isServer)
				_championData.state = ChampionStates.Moving;
		}

		[ClientRpc]
		private void StopAutoAttack()
		{
			//Logs.Info("Stop Auto Attack");
			var stopAutoAttacks = _championData.handles.OnStopAttacks.ToArray();
			foreach (var stopAttack in stopAutoAttacks)
			{
				stopAttack.OnStopAutoAttack();
			}

			if (isServer)
				_championData.state = ChampionStates.None;
		}

		[ClientRpc]
		private void StartAutoAttack()
		{
			//Logs.Info("Start Auto Attack");

			_championData.animatorNetwork.animator.fireEvents = true;
			var attackHandles = _championData.handles.OnStartAutoAttacks.ToArray();
			foreach (var attackHandle in attackHandles)
			{
				attackHandle.OnStartAutoAttack();
			}

			if (isServer)
				_championData.state = ChampionStates.Attacking;
		}


		[Server]
		public void Pause(IPauseHandle pauseHandle)
		{
			if (null == pauseHandle)
			{
				Logs.Error("Cant Pause when Pause handle is null");
				return;
			}

			_championPauseHandle.AddPauseHandle(pauseHandle);
			_championData.agent.moveSpeed = 0;
			_championAutoAttackSpeed.SetAutoAttack(false);
			_championMoveSpeed.SetMoveSpeed(0);

		}

		/// <summary>
		/// Set Champion is death
		/// </summary>
		[Server]
		public void SetDeath()
		{
			_championData.state = ChampionStates.Stopped;
			_championData.imperishable = false;
			_championData.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, -_championData.healthData.maxHealth));
			RpcOnDead();
		}

		[ClientRpc]
		private void RpcOnDead()
		{
			var onDeaths = _championData.handles.OnDeads.ToArray();

			foreach (var attackHandle in onDeaths)
			{
				attackHandle.OnDead();
			}
		}

		[Server]
		public void StartAlive()
		{
			_championPauseHandle.Clear();
			_championData.state = ChampionStates.None;
			_timeRemainingUpdate = _championData.updateRate;
			_healthRegenTiming = 0;
			RpcStartAlive();
		}

		[ClientRpc]
		private void RpcStartAlive()
		{
			var startLivings = _championData.handles.OnStartAlive.ToArray();
			foreach (var alive in startLivings)
			{
				alive.OnStartAlive();
			}
		}

		[Server]
		public void SetTrigger(int animHashID)
		{
			//  Pause();
			_championData.animatorNetwork.SetTrigger(animHashID);
		}

	}
}