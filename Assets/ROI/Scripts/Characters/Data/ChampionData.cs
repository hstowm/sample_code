using System;
using UnityEngine;
using Mirror;

namespace ROI
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(ChampionAgent))]
	[RequireComponent(typeof(IAttacker))]
	[RequireComponent(typeof(ChampionAttackAnimEvents))]
	public class ChampionData : NetworkBehaviour, IEquatable<ChampionData>
	{

		[SyncVar]
		public bool imperishable;

		// delay time for an update
		[SyncVar]
		public float updateRate = 1 / 25f;

		private int _instanceID;

        #region INSPECTOR SERIALIZE DATA

		public AnimData attackAnim = new();
		public AnimData moveAnim = new();

		[SyncVar] public AttackData attackData = new();
		[SyncVar] public DefenseData defenseData = new();
		[SyncVar] public MoveData moveData = new();
		[SyncVar] public HealthData healthData = new();
		[SyncVar] public SpecialStatData specialStatData = new();

		/// <summary>
		/// UID of User Champion
		/// </summary>
		[SyncVar] public string userChampionUID;
		[SyncVar] public uint creatorNetId = uint.MaxValue;
		[SyncVar] public bool ownedByHost;
		[SyncVar] public bool isIllusion;
        [SyncVar] public bool isNPC = false;
        [SyncVar] public uint maxUltimateEnergy;
		[SyncVar] public bool canUseEnergy = false;
		// [SyncVar] public string championTag;

        #endregion

        #region ASSIGN ON CREATED

		[HideInInspector] public NetworkAnimator animatorNetwork;

		[HideInInspector] public Collider col;
		[HideInInspector] public Rigidbody rigid;

		// dont edit inspector. Change in runtime
		[SyncVar] public ChampionStates state = ChampionStates.None;

		// dont assign in inspector. It's been Changed in runtime
		[SyncVar] [HideInInspector] public ChampionData target;

		[HideInInspector] public SyncList<ChampionData> enemies = new();
		[HideInInspector] public SyncList<ChampionData> allies = new();
		[HideInInspector] public SyncList<ChampionData> attackers = new();

		[HideInInspector][SyncVar] public float bodyRadius;

		// public MeshRenderer effect_render;
		public ChampionHandles handles;
		public ChampionShieldManager shieldManager;
		public ChampionStatModifier statModifier;

		[HideInInspector]
		public ChampionAgent agent;
		[SyncVar]
		public ChampionEffects currentEffect;
		
		public IAttacker attacker;
		public IController controller;
		public ITargetFinder targetFinder;

        #endregion

        #region READONLY PROPERTIES

		public bool IsPlayer => gameObject.CompareTag(GameTags.Player);
		public bool IsDeath => healthData.health <= 0;
		public bool HaveTarget => target && target.IsDeath == false;
		public Vector3 CenterPosition => col.bounds.center;
		
        #endregion

		public bool IsMine {
			[Client]
			get => NetworkClient.active && creatorNetId == NetworkClient.connection.identity.netId;
		}

		/// <summary>
		/// Init Data On Awake
		/// </summary>
		private void Awake()
		{

			_instanceID = GetInstanceID();
			new ChampionInitializer(this).Init();
		}

		public bool Equals(ChampionData other) => other && other._instanceID == _instanceID;
	}
}