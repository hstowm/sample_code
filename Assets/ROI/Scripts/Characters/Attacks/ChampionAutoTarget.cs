using Mirror;
using UnityEngine;

namespace ROI
{
    [RequireComponent(typeof(ChampionData))]
    class ChampionAutoTarget : MonoBehaviour,ITargetFinder
    {
        private ChampionData _championData;
        private int _instanceID;
        private Transform _transform;

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
            _instanceID = gameObject.GetInstanceID();
            _transform = transform;
        }

        [Server]
        public bool FindTarget(out  ChampionData target)
        {
            var enemies = _championData.enemies;
            target = null;
            
            if (enemies.Count == 0)
            {
                return false;
            }

            var pos = _transform.position;

            var min = -1f;
            var minIndex = -1;

            for (int i = enemies.Count - 1; i > -1; i--)
            {

                // dont target on dead target or yourself
                if (enemies[i].IsDeath || enemies[i].GetInstanceID() == _instanceID)
                    continue;

                var dist = Vector3.Distance(enemies[i].transform.position, pos);
                if (minIndex < 0 || dist < min)
                {
                    min = dist;
                    minIndex = i;
                }
            }

            if (minIndex < 0 || minIndex >= enemies.Count)
            {
              //  _championData.target = null;
                return false;
            }

            target = enemies[minIndex];
            return true;
        }
    }
}