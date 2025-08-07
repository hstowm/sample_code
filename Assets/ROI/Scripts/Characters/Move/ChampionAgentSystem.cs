using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using ProjectDawn.LocalAvoidance;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace ROI
{
	[RequireComponent(typeof(ChampionManager))]
	class ChampionAgentSystem : NetworkBehaviour
	{
		[Range(1, 10)] public float SonarRadius = 6;
		[Range(0, 1)] public float SonarForwardVelocityScaler = 1;
		[Range(0, 1)] public float SonarBackVelocityScaler = 1;
		[Range(0, 180)] public float SonarCutBackVisionAngle = 135f;
		public bool ChangeTransform = true;
		public bool TightFormation = false;
		public bool Is3D = true;
		public float AgentAcceleration = 8;

		[Range(0, 1)] public float stopDistance = 0.1f;

		private ChampionManager _championManager;

		private MapSystem _mapSystem = null;


		private void Awake()
		{
			_championManager = GetComponent<ChampionManager>();
			
		}

		private readonly Vector3[] _barrierlines = new Vector3[4];

		private void Start()
		{
			StartCoroutine(GetMapSystem());		
		}

		private IEnumerator GetMapSystem()
		{
			while( _mapSystem == null || _mapSystem.mapData == null || _mapSystem.mapData.bgCollider == null )
			{
                _mapSystem = FindObjectOfType<MapSystem>(true);

                yield return new WaitForEndOfFrame();

            }
            var collier = _mapSystem.mapData.bgCollider;
            var center = collier.center;
            var size = collier.size;
            var trs = collier.transform;
            _barrierlines[0] = trs.TransformPoint(center + new Vector3(-size.x, 0, -size.z) * 0.5f);
            _barrierlines[1] = trs.TransformPoint(center + new Vector3(size.x, 0, -size.z) * 0.5f);
            _barrierlines[2] = trs.TransformPoint(center + new Vector3(size.x, 0, size.z) * 0.5f);
            _barrierlines[3] = trs.TransformPoint(center + new Vector3(-size.x, 0, size.z) * 0.5f);
        }

		void FixedUpdate()
		{
			if (_championManager.agents.Count == 0 || isServer == false)
				return;

			foreach (var agent in _championManager.agents)
			{
				if (agent.HasTarget == false)
					continue;

				float3 impulse = 0;
				if (!agent.IsStopped && agent.moveSpeed > 0)
				{
					float3 desiredDirection = math.normalizesafe(agent.currentDestination - agent.Position);
					if (agent.avoid)
						impulse += GetAvoid(agent, desiredDirection);
					else
						impulse += desiredDirection * agent.moveSpeed;

					var velocity = math.lerp(agent.currentVelocity, impulse, Time.deltaTime * AgentAcceleration);
					agent.currentVelocity = velocity;
				} else
				{
					agent.currentVelocity = 0;
				}

				if (ChangeTransform && math.lengthsq(agent.currentVelocity) != 0)
				{
					var offset = agent.currentVelocity * Time.deltaTime;
					offset.y = 0;

					// Avoid over-steping destination
					var distance = math.distance(agent.currentDestination, agent.Position);
					offset = ForceLength(offset, distance);

					var transf = agent.transform;

					var pos = transf.position;
					pos += (Vector3)offset;
					
					transf.position = pos;

					if (agent.inAttackRange == false)
					{
						var v = agent.currentVelocity;
						v.y = 0;
						//agent.currentVelocity.y = 0;
						//if (NeedRotation(tr.position, v))
						var r = transf.rotation;
						transf.rotation = Quaternion.Lerp(r, Quaternion.LookRotation(v, Vector3.up), Time.deltaTime);
					}
				}
			}
		}


		//	private float3 _rotateVelocity;

		// private Vector3 _rotationPos;
		//
		// bool NeedRotation(Vector3 pos, float3 v)
		// {
		// 	if (Vector3.Angle(pos, v) < 1f)
		// 		return false;
		//
		// 	if (Mathf.Approximately(Vector3.Distance(pos, _rotationPos), 0.2f))
		// 		return false;
		//
		// 	_rotationPos = pos;
		// 	return true;
		// }


		float3 GetAvoid(ChampionAgent agent, float3 desiredDirection)
		{
			if (math.lengthsq(desiredDirection) == 0)
				return float3.zero;

			// Destination should not be farther than the vision
			var sonarRadius = math.min(SonarRadius, math.distance(agent.currentDestination, agent.Position));

			var up = Is3D ? new float3(0, 1, 0) : new float3(0, 0, -1);
			var sonar = new SonarAvoidance(
				agent.Position,
				desiredDirection,
				up,
				agent.bodyRadius,
				sonarRadius,
				math.length(agent.currentVelocity));

			for (int i = 0; i < _barrierlines.Length - 1; i++)
			{
				sonar.InsertObstacle(_barrierlines[i], _barrierlines[i + 1]);
			}

			// Logs.Info($"Node Length: {sonar.m_Nodes.Length}");

			var agentCircle =
				new Circle(agent.Position.xz + math.length(agent.currentVelocity) * Time.deltaTime * desiredDirection.xz,
					agent.bodyRadius);
			bool desiredDirectionOccluded = false;

			// foreach (Circle c in _listBarriers)
			// {
			// 	sonar.InsertObstacle(new float3(c.Point.x, 0, c.Point.y), 0, c.Radius);
			// }


			foreach (var nearbyAgent in _championManager.agents)
			{
				// Skip itself
				if (nearbyAgent.netId == agent.netId)
					continue;

				sonar.InsertObstacle(nearbyAgent.Position, nearbyAgent.currentVelocity, nearbyAgent.bodyRadius);

				// Check if taking a step to desired direction would collide with target agent
				if (TightFormation && !desiredDirectionOccluded && Circle.Collide(agentCircle,
					    new Circle(nearbyAgent.Position.xz, nearbyAgent.bodyRadius)))
				{
					desiredDirectionOccluded = true;
				}
			}

			// Add blocker behind the velocity
			// This will prevent situations where agent has on right and left equally good paths
			sonar.InsertObstacle(math.normalizesafe(-agent.currentVelocity), math.radians(SonarCutBackVisionAngle));

			bool success = sonar.FindClosestDirection(out float3 newDirection);

			// If all directions obstructed, try to at least move agent as close as possible to destination without colliding
			// This is not necessary step, but it yields much better formation movement
			if (TightFormation && !success && !desiredDirectionOccluded)
			{
				newDirection = new float3(desiredDirection.x, 0, desiredDirection.y);
			}

			sonar.Dispose();

			return newDirection * agent.moveSpeed;
		}

		float3 ForceLength(float3 value, float length)
		{
			var valueLength = math.length(value);
			return valueLength > length ? value / valueLength * length : value;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{

			for (int i = 0; i < _barrierlines.Length - 1; i++)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(_barrierlines[i], _barrierlines[i + 1]);
			}

			Gizmos.DrawLine(_barrierlines[0], _barrierlines[^1]);
		}
#endif

	}
}