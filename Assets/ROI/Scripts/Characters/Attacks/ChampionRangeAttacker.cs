using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ROI
{
    class ChampionRangeAttacker : NetworkBehaviour, IOnAttackEvent, IInjectInstance<BulletSystem>
    {
        [SerializeField] private Object bullet;
        [SerializeField] private uint poolSize = 2;
        [SerializeField] private Transform _spawnPoint;

        private ChampionData _championData;
        private int _poolID;
        private BulletSystem _bulletSystem;

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }

        public void OnInject(BulletSystem bulletSystem)
        {
            _bulletSystem = bulletSystem;
           // if (isServer)
            CreatePool();
        }

        public void OnAttackEvent()
        {
            if (_championData.HaveTarget == false
                || _championData.state == ChampionStates.Stopped
                || _bulletSystem == null)
                return;

            // rpc client
            // if (isServer)
            CreateBullet();
        }

        //[ClientRpc]
        void CreateBullet()
        {
            var bulletData = _bulletSystem.CreateBullet(_poolID, _championData);
            bulletData.transform.position = _spawnPoint.position;
            bulletData.GetComponent<IVfxBullet>().InitBullet(_championData, _bulletSystem);
        }

        // [ClientRpc]
        void CreatePool()
        {
            _poolID = _bulletSystem.CreatePool(bullet, poolSize);
        }
    }
}