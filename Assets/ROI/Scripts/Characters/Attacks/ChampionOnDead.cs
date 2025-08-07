using System.Collections;
using UnityEngine;
using Mirror;

namespace ROI
{
    class ChampionOnDead : NetworkBehaviour, IOnDead
    {
        private ChampionData _championData;
        // private ChampionAutoAttackSpeed _championAutoAttackSpeed;

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }

        public void OnDead()
        {
            if (isServer)
            {
                _championData.state = ChampionStates.Stopped;
                _championData.animatorNetwork.SetTrigger(AnimHashIDs.Death);
            }

            _championData.col.enabled = false;
            _championData.rigid.isKinematic = true;
            _championData.target = null;
            _championData.ShowUltimateCard(false);

            StartCoroutine(OnDeathAnim());
        }
        
        private IEnumerator OnDeathAnim()
        {
            var trans = transform;
            yield return new WaitForSeconds(3);

            var t = 3f;
            while (t > 0)
            {
                var pos = trans.position;
                trans.position = new Vector3(pos.x, pos.y - Time.deltaTime, pos.z);
                t -= Time.deltaTime;
                yield return null;
            }

            // StopAllCoroutines();
            gameObject.Recycle();
        }
    }
}