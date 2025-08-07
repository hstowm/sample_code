using System.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace ROI
{
    readonly struct ChampionInitializer
    {
        private readonly ChampionData _championData;
        private readonly ChampionAnimInitializer _animInitializer;

        public ChampionInitializer(ChampionData championData)
        {
            _championData = championData;
            _animInitializer = new ChampionAnimInitializer();
        }

        public void Init()
        {
            // init all component first
            InitComponent();
            
            // init anim
            _animInitializer.OnInitialized(_championData);
        }

        /// <summary>
        /// Initialize component
        /// </summary>
        private void InitComponent()
        {
            var obj = _championData.gameObject;
            
            _championData.col = obj.GetComponent<Collider>();
            _championData.col.enabled = true;

            _championData.rigid = obj.GetComponent<Rigidbody>();
            _championData.animatorNetwork = obj.GetComponent<NetworkAnimator>();
            // _championData.animatorNetwork.animator = obj.GetComponentInChildren<Animator>();
            
            _championData.bodyRadius = obj.GetComponent<CapsuleCollider>().radius;
            _championData.agent = obj.GetComponent<ChampionAgent>();
            _championData.agent.bodyRadius = _championData.bodyRadius;
            
            _championData.handles = new ChampionHandles(obj);

            _championData.targetFinder = obj.GetComponent<ITargetFinder>();
            _championData.attacker = obj.GetComponent<IAttacker>();
           
            _championData.shieldManager = obj.GetComponent<ChampionShieldManager>();

            _championData.controller = obj.GetComponent<IController>();
        }
    }
}