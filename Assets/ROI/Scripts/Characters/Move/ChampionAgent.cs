using System;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace ROI
{
    public class ChampionAgent : NetworkBehaviour, IOnStartMove, IOnStopMove, IEquatable<ChampionAgent>
    {
        [HideInInspector] public float3 currentDestination;
        [HideInInspector] public float moveSpeed = 1;
        [HideInInspector] public float bodyRadius = 0.4f;
        [HideInInspector] public float3 currentVelocity;
        [HideInInspector] public bool inAttackRange;

        public bool avoid = true;
        public bool IsStopped { get; private set; }

        private Transform _transform;
        private ChampionData _championData;

        private int _instanceId;

        private void Awake()
        {
            _transform = transform;
            _championData = GetComponent<ChampionData>();
            _instanceId = GetInstanceID();
        }

        public void OnStartMove()
        {
            moveSpeed = _championData.moveData.moveSpeed;
            currentVelocity = Vector3.zero;
            IsStopped = false;
        }

        public void OnStopMove()
        {
            moveSpeed = 0;
            currentVelocity = Vector3.zero;
            // TurnSpeed = 0;
            IsStopped = true;
        }

        private void OnEnable()
        {
            OnStopMove();
        }

        public bool HasTarget => IsStopped == false && _championData.HaveTarget;

        public float3 Position
        {
            get
            {
                var centerPoint = _championData.CenterPosition;
                centerPoint.y = _transform.position.y;

                return centerPoint;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
                return;

            Gizmos.DrawWireSphere(Position, bodyRadius);
        }
#endif
        public bool Equals(ChampionAgent other)
        {
            return other && other._instanceId == _instanceId;
        }

        public static bool operator==(ChampionAgent a, ChampionAgent b)
        {
            return a && a.Equals(b);
        }
        public static bool operator !=(ChampionAgent a, ChampionAgent b)
        {
            return !(a == b);
        }
        
        
    }
}