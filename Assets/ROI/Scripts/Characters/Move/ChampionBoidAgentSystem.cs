using System.Collections.Generic;
using ProjectDawn.LocalAvoidance;
using Unity.Mathematics;
using UnityEngine;

namespace ROI
{
	/// <summary>
	/// System that executs local avoidance logic for <see cref="Agent"/>.
	/// </summary>
	[DefaultExecutionOrder(50)]
	[RequireComponent(typeof(ChampionManager))]
	class ChampionBoidAgentSystem : MonoBehaviour
	{
		public float SeperationWeight = 2.2f;
		public float AlignmentWeight = 0.3f;
		public float CohesionWeight = 0.05f;
		public float AvoidanceWeight = 2.8f;
		public bool PushStanding = false;
		public bool ChangeTransform = true;
		public bool TightFormation = true;

		private List<ChampionBoidAgent> _agents = new List<ChampionBoidAgent>(2);

		private ChampionManager _championManager;

		private void Awake()
		{
			_championManager = GetComponent<ChampionManager>();
		}

		void FixedUpdate()
		{
			//  var agents = Query<BoidsAgent>();

			foreach (var agent in _agents)
			{
				float2 impulse = float2.zero;

				// Notice: In scalable game this would been optimized. For example using spatial partitioning for querying only nearby agents
				// Check JobifiedAgentSystem it has spatial partitioning done with multi hash map
				var nearbyAgents = _agents;

				if (agent.CanMove())
				{
					impulse += GetSeek(agent, agent.Destination);
					impulse += GetAlignment(nearbyAgents, agent) * AlignmentWeight;
					impulse += GetCohesion(nearbyAgents, agent) * CohesionWeight;
					impulse += GetSeparation(nearbyAgents, agent) * SeperationWeight;
					impulse += GetAvoid(nearbyAgents, agent, math.normalizesafe(agent.Destination - agent.Position)) * AvoidanceWeight;

					Debug.DrawLine(agent.transform.position, new Vector3(agent.Destination.x, 0, agent.Destination.y), Color.green);
				} else
				{
					if (!agent.Pushable || !PushStanding)
						continue;

					impulse += GetSeparation(nearbyAgents, agent) * SeperationWeight;

					// TODO: This should account of time
					agent.Velocity *= 0.9f;
				}

				if (ChangeTransform)
				{
					agent.ApplyImpulse(impulse);
					Distinguish(nearbyAgents, agent);
				}

				Arrival(agent);
			}
		}

		void Arrival(ChampionBoidAgent boidAgent)
		{
			float distance = math.distance(boidAgent.Position, boidAgent.Destination);

			if (distance <= boidAgent.StopingDistance)
				boidAgent.Stop();
		}

		float2 GetSeek(ChampionBoidAgent boidAgent, float2 destination)
		{
			float2 position = boidAgent.Position;

			if (math.all(position == destination))
				return float2.zero;

			float2 desired = destination - position;

			desired *= boidAgent.MaxSpeed / math.length(desired);

			float2 velocityChange = desired - boidAgent.Velocity;

			velocityChange *= boidAgent.ForceDivSpeed;

			return velocityChange;
		}

		float2 GetSeparation(IList<ChampionBoidAgent> agents, ChampionBoidAgent boidAgent)
		{
			float2 totalForce = float2.zero;
			int neighboursCount = 0;
			int count = agents.Count;

			for (var i = 0; i < count; i++)
			{
				ChampionBoidAgent targetBoidAgent = agents[i];

				if (boidAgent.CanSeperate(targetBoidAgent))
				{
					float2 pushForce = boidAgent.Position - targetBoidAgent.Position;
					float distance = math.length(pushForce);

					if (distance < boidAgent.Separation && distance > 0)
					{
						float r = (boidAgent.Radius + targetBoidAgent.Radius);

						totalForce += pushForce * (1f - ((distance - r) / (boidAgent.Separation - r)));

						neighboursCount++;
					}
				}
			}

			if (neighboursCount == 0)
				return float2.zero;

			totalForce *= boidAgent.MaxForce / neighboursCount;

			return totalForce;
		}

		float2 GetCohesion(IList<ChampionBoidAgent> agents, ChampionBoidAgent boidAgent)
		{
			float2 centerOfMass = float2.zero;
			int neighboursCount = 0;
			int count = agents.Count;

			for (var i = 0; i < count; i++)
			{
				ChampionBoidAgent targetBoidAgent = agents[i];

				if (boidAgent.CanFlock(targetBoidAgent))
				{
					float distance = math.distance(boidAgent.Position, targetBoidAgent.Position);

					if (distance < boidAgent.Cohesion)
					{
						centerOfMass += targetBoidAgent.Position;

						neighboursCount++;
					}
				}
			}

			if (neighboursCount == 0)
				return float2.zero;

			centerOfMass /= neighboursCount;

			return GetSeek(boidAgent, centerOfMass);
		}

		float2 GetAlignment(IList<ChampionBoidAgent> agents, ChampionBoidAgent boidAgent)
		{
			float2 averageHeading = float2.zero;
			int neighboursCount = 0;
			int count = agents.Count;

			for (var i = 0; i < count; i++)
			{
				ChampionBoidAgent targetBoidAgent = agents[i];
				float distance = math.distance(boidAgent.Position, targetBoidAgent.Position);
				float speed = math.length(targetBoidAgent.Velocity);

				if (distance < boidAgent.Cohesion && speed > 0 && boidAgent.CanFlock(targetBoidAgent))
				{
					float2 head = targetBoidAgent.Velocity / speed;

					averageHeading += head;
					neighboursCount++;
				}
			}

			if (neighboursCount == 0)
				return averageHeading;

			averageHeading /= neighboursCount;

			return GetSteerTowards(boidAgent, averageHeading);
		}

		float2 GetAvoid(IList<ChampionBoidAgent> agents, ChampionBoidAgent boidAgent,
			float2 desiredDirection)
		{
			if (math.lengthsq(desiredDirection) == 0)
				return float2.zero;

			float maxDistance = math.min(6, math.distance(boidAgent.Destination, boidAgent.Position));
			int count = agents.Count;

			if (maxDistance == 0)
				return float2.zero;

			SonarAvoidance sonar = new SonarAvoidance(
				new float3(boidAgent.Position.x, boidAgent.transform.position.y, boidAgent.Position.y),
				new float3(desiredDirection.x, 0, desiredDirection.y),
				new float3(0, 1, 0),
				boidAgent.Radius,
				maxDistance,
				math.length(boidAgent.Velocity)
			);

			var agentCircle = new Circle(boidAgent.Position + math.length(boidAgent.Velocity) * Time.deltaTime * desiredDirection, boidAgent.Radius);
			bool desiredDirectionOccluded = false;

			for (int i = 0; i < count; i++)
			{
				ChampionBoidAgent targetBoidAgent = agents[i];
				float2 direction = targetBoidAgent.Position - boidAgent.Position;
				float distance = math.length(direction);

				if (!boidAgent.CanAvoid(targetBoidAgent))
					continue;

				sonar.InsertObstacle(
					new float3(targetBoidAgent.Position.x, boidAgent.transform.position.y, targetBoidAgent.Position.y),
					new float3(targetBoidAgent.Velocity.x, 0, targetBoidAgent.Velocity.y),
					targetBoidAgent.Radius);

				// Check if taking a step to desired direction would collide with target agent
				if (TightFormation && !desiredDirectionOccluded && Circle.Collide(agentCircle, new Circle(targetBoidAgent.Position, targetBoidAgent.Radius)))
				{
					desiredDirectionOccluded = true;
				}
			}

			sonar.InsertObstacle(math.normalizesafe(-new float3(boidAgent.Velocity.x, 0, boidAgent.Velocity.y)), math.radians(135));

			bool success = sonar.FindClosestDirection(out float3 newDirection);

			// If all directions obstructed, try to at least move agent as close as possible to destination without colliding
			// This is not necessary step, but it yields much better formation movement
			if (TightFormation && !success && !desiredDirectionOccluded)
			{
				newDirection = new float3(desiredDirection.x, 0, desiredDirection.y);
			}

			// if (boidAgent.gameObject.TryGetComponent(out AgentDebug agentDebug))
			// {
			//     if (agentDebug.Vision.IsCreated)
			//         agentDebug.Vision.Dispose();
			//     agentDebug.Vision = new SonarAvoidance(sonar, Unity.Collections.Allocator.Persistent);
			// }

			sonar.Dispose();

			return GetSteerTowards(boidAgent, new float2(newDirection.x, newDirection.z));
		}

		void Distinguish(IList<ChampionBoidAgent> agents, ChampionBoidAgent boidAgent)
		{
			int count = agents.Count;

			for (int i = 0; i < count; i++)
			{
				ChampionBoidAgent targetBoidAgent = agents[i];

				if (boidAgent.CanDistinguish(targetBoidAgent))
				{
					float2 direction = boidAgent.Position - targetBoidAgent.Position;
					float directionMagnitude = math.length(direction);
					float distance = boidAgent.Radius + targetBoidAgent.Radius - directionMagnitude;
					float2 seperate = direction / directionMagnitude * distance;

					if (distance > 0 && directionMagnitude > 0)
					{
						boidAgent.transform.position += new Vector3(seperate.x, 0, seperate.y) *
						                                boidAgent.GetDistinguishWeigth(targetBoidAgent);
					}
				}
			}
		}

		float2 GetSteerTowards(ChampionBoidAgent boidAgent, float2 desiredDirection)
		{
			float2 desiredVelocity = desiredDirection * boidAgent.MaxSpeed;
			float2 velocityChange = desiredVelocity - boidAgent.Velocity;

			velocityChange *= boidAgent.ForceDivSpeed;

			return velocityChange;
		}


	}
}