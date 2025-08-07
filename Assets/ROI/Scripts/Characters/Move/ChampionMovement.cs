using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ROI
{
	[RequireComponent(typeof(ChampionData))]
	class ChampionMovement : NetworkBehaviour, IOnStartAlive, IOnDead //, IOnStartMove
	{
		private ChampionData _championData;
		private MapSystem _mapSystem;

		private bool _needMoveToAttackPosition;
		private uint _championNetId;

		private bool _isSnap;
		private bool _isPaused;
		private float movementDistanceApproximate = 0.5f;
		private bool _needUpdate;

		private void Awake()
		{
			// _transform = transform;

			_championData = GetComponent<ChampionData>();
			_mapSystem = FindObjectOfType<MapSystem>(true);

			// _championManager = FindObjectOfType<ChampionManager>(true);
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			MapData.targetCoordinates.Clear();
		}

		private bool IsApproximately(float v) => v < movementDistanceApproximate || Mathf.Approximately(movementDistanceApproximate, v);

		[Server]
		public void MoveToPosition(Vector3 targetPos)
		{
			// _targetPos = targetPos;
			//targetPos.y = _championData.transform.position.y;
			_championData.agent.currentDestination = targetPos;
		}

		private void MoveToPosition()
		{
			var pos = _mapSystem.ConvertCoordinateToWorldPosition(MapData.targetCoordinates[_championNetId]);
			MoveToPosition(pos);
		}


		public bool InAttackRange {
			get {

				if (_needUpdate)
				{
					UpdateHexPos();
					_needUpdate = false;
					return false;
				}

				// 1. check in range of attack data
				var championPos = _championData.transform.position;
				var enemyPos = _championData.target.transform.position;

				enemyPos.y = championPos.y = 0;

				var dir = enemyPos - championPos;
				var range = _mapSystem.ConvertToUnit(_championData.attackData.range);

				var offset = Mathf.Abs(dir.magnitude - range);

				_championData.agent.inAttackRange = dir.magnitude <= range || IsApproximately(offset);

				if (_championData.agent.inAttackRange == false || _isPaused)
				{
					_isPaused = FindAttackPosition() == false;
					return false;
				}

				//2. check snap in
				var targetPosition = _mapSystem.ConvertCoordinateToWorldPosition(MapData.targetCoordinates[_championNetId]);
				targetPosition.y = championPos.y;

				_isSnap = IsApproximately(Vector3.Distance(targetPosition, championPos));

				if (_isSnap == false && Vector3.Distance(enemyPos, championPos) <= _mapSystem.ConvertToUnit(0.5f))
				{
					MoveToNeighbor(championPos, enemyPos);
					return false;
				}

				if (_isSnap)
					RotationToTarget();
				else
					RotateToCoordinate(MapData.targetCoordinates[_championNetId]);

				return _isSnap;
			}
		}

		/// <summary>
		/// Move To Neighbor of enemies of position
		/// </summary>
		/// <param name="championPos"></param>
		/// <param name="enemyPos"></param>
		private void MoveToNeighbor(Vector3 championPos, Vector3 enemyPos)
		{
			var enemyCoord = _mapSystem.ConvertWorldPositionToCoordinate(enemyPos);
			var neighbors = _mapSystem.GetNeighborCoordinate(enemyCoord);

			var distance = float.MaxValue;
			var min = -1;
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (_mapSystem.IsBlockedByOther(_championNetId, neighbors[i]))
					continue;

				var pos = _mapSystem.ConvertCoordinateToWorldPosition(neighbors[i]);
				pos.y = championPos.y;

				var d = Vector3.Distance(pos, championPos);
				if (d < distance)
				{
					distance = d;
					min = i;
				}
			}

			if (min > -1)
			{
				MapData.targetCoordinates[_championNetId] = neighbors[min];
				RotateToCoordinate(MapData.targetCoordinates[_championNetId]);
			}
		}

		/// <summary>
		/// Move To The Target
		/// </summary>
		public void MoveToTarget()
		{
			if (_championData.agent.inAttackRange)
			{
				if (_isSnap == false)
					MoveToPosition();
				return;
			}

			var enemyPos = _championData.target.transform.position;
			MoveToPosition(enemyPos);
		}

		private void UpdateHexPos()
		{
			var championPos = _championData.transform.position;
			var enemyPosition = _championData.target.transform.position;
			var targetCoordinate = MapData.targetCoordinates[_championNetId];
			var targetPosition = _mapSystem.ConvertCoordinateToWorldPosition(targetCoordinate);

			var championToEnemyDistance = Vector3.Distance(championPos, enemyPosition);
			var targetToEnemyDistance = Vector3.Distance(targetPosition, enemyPosition);

			if (targetToEnemyDistance > championToEnemyDistance
			    && IsApproximately(Mathf.Abs(targetToEnemyDistance - championToEnemyDistance)) == false)
			{
				var listNeighbors = _mapSystem.GetNeighborCoordinate(targetCoordinate);

				if (listNeighbors.Count < 1)
					return;
				var minIndex = 0;
				var pos = _mapSystem.ConvertCoordinateToWorldPosition(listNeighbors[minIndex]);
				pos.y = enemyPosition.y;

				//var distance = Vector3.Distance(pos, enemyPosition);
				var distance = float.MaxValue;
				
				for (int i = 1; i < listNeighbors.Count; i++)
				{
					if (_mapSystem.IsValidCoordinate(listNeighbors[i]) == false)
						continue;

					pos = _mapSystem.ConvertCoordinateToWorldPosition(listNeighbors[i]);
					pos.y = enemyPosition.y;
					var d = Vector3.Distance(pos, enemyPosition);
					if (d < distance)
					{
						minIndex = i;
						distance = d;
						MapData.targetCoordinates[_championNetId] = listNeighbors[minIndex];
					}
				}
				// UpdatePosition();
			}
		}


		private bool FindAttackPosition()
		{
			// clamp attack range range
			var championPos = _championData.transform.position;
			var attackRange = Mathf.Max(1, (int)_championData.attackData.range);
			// var targetCoordinate = _targetCoordinates[_championNetId];
			var enemyPosition = _championData.target.transform.position;

			championPos.y = enemyPosition.y = 0;

			var enemyCoordinate = _mapSystem.ConvertWorldPositionToCoordinate(enemyPosition);
			var championCoordinate = _mapSystem.ConvertWorldPositionToCoordinate(championPos);

			var range = _mapSystem.OffsetDistance(championCoordinate, enemyCoordinate);
			/*if (attackRange <= range)
			{
				return true;
			}*/
			if (attackRange > range)
			{
				attackRange = range;
				//return true;
			}

			var distance = float.MaxValue;

			for (int i = attackRange; i > 0; i--)
			{
				var ring = _mapSystem.GetSingleRing(enemyCoordinate, i);
#if UNITY_EDITOR
				_testRings.Clear();
				_testRings.AddRange(ring);
#endif
				var minIndex = -1;

				for (int index = 0; index < ring.Count; index++)
				{
					if (_mapSystem.IsBlockedByOther(_championNetId, ring[index]) || _mapSystem.IsValidCoordinate(ring[index]) == false)
						continue;

					var ringPosition = _mapSystem.ConvertCoordinateToWorldPosition(ring[index]);
					ringPosition.y = championPos.y;
					var ringToChampion = Vector3.Distance(ringPosition, championPos);

					if (ringToChampion > distance)
						continue;

					minIndex = index;
					distance = ringToChampion;
				}
				if (minIndex > -1)
				{
					MapData.targetCoordinates[_championNetId] = ring[minIndex];
					return true;
				}

			}

			return false;
		}




#if UNITY_EDITOR

		private readonly List<Coordinate> _testRings = new List<Coordinate>();


		private void OnDrawGizmos()
		{
			if (Application.isPlaying == false)
				return;

			if (MapData.targetCoordinates.Count > 0)
			{
				Gizmos.color = Color.red;

				foreach (var c in MapData.targetCoordinates)
				{
					Gizmos.DrawWireSphere(_mapSystem.ConvertCoordinateToWorldPosition(c.Value), 0.3f);
				}
			}

			if (_testRings.Count > 0)
			{
				Gizmos.color = Color.green;

				foreach (var c in _testRings)
				{
					Gizmos.DrawWireSphere(_mapSystem.ConvertCoordinateToWorldPosition(c), 0.2f);
				}
			}

		}
#endif

		[Server]
		public float RotationToTarget()
		{
			return RotateTo(_championData.target.transform.position);
		}

		public void RotateToCoordinate(Coordinate coordinate)
		{
			var targetPosition = _mapSystem.ConvertCoordinateToWorldPosition(coordinate);
			RotateTo(targetPosition);
		}

		public float RotateTo(Vector3 targetPos)
		{
			var ts = _championData.transform;

			var pos = ts.position;

			var targetDirection = targetPos - pos;
			targetDirection.y = 0;

			// Calculate a rotation a step closer to the target and applies rotation to this object
			ts.rotation = Quaternion.LookRotation(targetDirection);

			var forward = ts.forward;
			forward.y = 0;

			return Vector3.Angle(forward, targetDirection);
		}

		public void OnStartAlive()
		{
			// Logs.Info($"Start Alive: {_championData.name}, Is Server: {isServer}");

			if (isServer == false)
				return;

			_championNetId = _championData.netId;
			InitHexPosition();

			// _inRange = false;
			_isSnap = false;

			_championData.agent.inAttackRange = false;

			// UpdateTargetPosition();

			if (GeneralEffectSystem.Instance.removeEffectActions.ContainsKey(_championNetId))
			{
				GeneralEffectSystem.Instance.removeEffectActions[_championNetId] -= OnRemoveEffect;
				GeneralEffectSystem.Instance.removeEffectActions[_championNetId] += OnRemoveEffect;
			}

			_needUpdate = true;
			_isPaused = false;
		}

		public void InitHexPosition()
		{
			MapData.targetCoordinates[_championNetId] =
				_mapSystem.ConvertWorldPositionToCoordinate(_championData.transform.position);
		}

		private void OnRemoveEffect(StatusData statusData)
		{
			// var pauseHandles = new List<IPauseHandle>();
			if (statusData.effect_system is FearStayAwayLocationEffectCC ||
			    statusData.effect_system is BaseForceMoveEffect)
			{
				// Logs.Info($"{statusData.effect_system.GetType().FullName} is out of date");
				// var coordinate = _mapSystem.ConvertWorldPositionToCoordinate(_championData.transform.position);
				// MapData.targetCoordinates[_championNetId] = coordinate;

				// _needUpdate = true;
				UpdateNonBlockCoordinate();
			}
		}

		public void OnDead()
		{
			if (isServer == false)
				return;

			MapData.targetCoordinates[_championNetId] = new Coordinate(-1, -1);
		}

		private void UpdateNonBlockCoordinate()
		{
			var coordinate = _mapSystem.ConvertWorldPositionToCoordinate(_championData.transform.position);
			if (_mapSystem.IsValidCoordinate(coordinate) == false)
			{
				FindAttackPosition();
				return;
			}
			var currentCoordinate = MapData.targetCoordinates[_championNetId];
			if (currentCoordinate == coordinate)
				return;
			for (int r = 1; r < 8; r++)
			{
				var neighbors = _mapSystem.GetSingleRing(coordinate, r);
				for (int i = 0; i < neighbors.Count; i++)
				{
					if (_mapSystem.IsBlockedByOther(_championNetId, neighbors[i]) || _mapSystem.IsValidCoordinate(neighbors[i]) == false)
						continue;

					MapData.targetCoordinates[_championNetId] = neighbors[i];
					return;

				}
			}
		}
	}
}