using Mirror;
using UnityEngine;

namespace ROI
{
    class ChampionMeleeAttacker : NetworkBehaviour, IOnAttackEvent
    {
        private Transform _transform;
        private ChampionData _championData;
        private readonly Collider[] _hitEnemies = new Collider[32];

        private void Awake()
        {
            _transform = transform;
            _championData = GetComponent<ChampionData>();
        }

        /// <summary>
        /// On Attack Event Fired
        /// </summary>
        public void OnAttackEvent()
        {
            if (isServer == false)
                return;
            
            if(_championData.HaveTarget)
                _championData.attacker.AttackEnemy(_championData.target, out _);

            /*
            // var enemies = new Collider[20];
            var pos = _championData.CenterPosition;
            var size = Physics.OverlapSphereNonAlloc(_championData.CenterPosition, _championData.attackData.range,
                _hitEnemies, GameLayers.Champion);
            if (size < 1)
                return;

            Transform hitEnemy;
            var enemyTag = _championData.IsPlayer ? GameTags.Enemy : GameTags.Player;
            // Debug.Log($"OnEnemiesInRange: {size}");
            //    var pos = _transform.position;
            var forward = _transform.forward;
            forward.y = 0;
            for (int i = 0; i < size; i++)
            {
                hitEnemy = _hitEnemies[i].transform;
                if (hitEnemy.CompareTag(enemyTag) == false)
                    continue;

                var ray = hitEnemy.position - pos;
                ray.y = 0;

                // Angle: {Vector3.Angle(forward, ray)}. Dot: {Vector3.Dot(forward, ray)}
                // 0 <= angle <= 90, dot >= 0
                var dot = Vector3.Dot(forward, ray);

                if (dot <= 0)
                    continue;

                var enemy = hitEnemy.GetComponent<ChampionData>();
                
               Logs.Info($"EQUAL TARGET: {enemy.Equals(_championData.target)}");
                
                // on hit
                _championData.attacker.AttackEnemy(enemy, out _);
            }*/
        }
    }
}