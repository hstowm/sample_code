using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace ROI
{
    [RequireComponent(typeof(IObjectPool))]
    class ChampionVfxSystem : NetworkBehaviour
    {
        private IObjectPool _objectPool;

        private void Awake()
        {
            _objectPool = GetComponent<IObjectPool>();
        }

        public int CreateVFXPool(VfxObject vfxObject)
        {
            return _objectPool.CreatPool(vfxObject.prefab, vfxObject.poolSize);
        }
        
        /// <summary>
        /// create vfx from vfx object data
        /// </summary>
        /// <param name="championData"></param>
        /// <param name="vfxObject"></param>
        /// <param name="vfx"></param>
        /// <returns></returns>
        public bool CreateVFX(ChampionData championData, VfxObject vfxObject, out GameObject vfx)
        {
            if (false == _objectPool.Use(vfxObject.prefab, vfxObject.lifeTime, out vfx))
                return false;

            var trans = vfx.transform;
            var pos = GetVFXSpawnPoint(championData, vfxObject, out var pt);

            if (vfxObject.isChild && pt)
            {
                trans.SetParent(pt);
                trans.localRotation = quaternion.identity;
                trans.localPosition = Vector3.zero;

                return true;
            }

            if (vfxObject.rotateByOwner)
            {
                trans.rotation = Quaternion.Euler(championData.transform.rotation.eulerAngles);
            }

            if (vfxObject.removeOnOwnerDeath)
            {
                //var go = vfx;
                var recycleOnDead = new RecycleGameObjectOnDead(vfx);
                if (championData.handles.OnDeads.Contains(recycleOnDead) == false)
                    championData.handles.OnDeads.Add(recycleOnDead);
            }

            trans.position = pos;
            return true;
        }

      

        /// <summary>
        /// Get Spawn Position From Vfx Object Data
        /// </summary>
        /// <param name="champion"></param>
        /// <param name="vfxObject"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Vector3 GetVFXSpawnPoint(ChampionData champion, VfxObject vfxObject, out Transform pt)
        {
            pt = null;
            var trans = champion.transform;

            switch (vfxObject.vfxPosition)
            {
                case VfxPositions.SpawnPoint:
                    pt = vfxObject.spawnPoint;
                    return vfxObject.spawnPoint ? pt.position : Vector3.zero;
                case VfxPositions.CenterTarget:
                    pt = champion.HaveTarget ? champion.transform : null;
                    return pt
                        ? champion.target.CenterPosition
                        : Vector3.zero;
                case VfxPositions.CenterPosition:
                    pt = trans;
                    return champion.CenterPosition;
                case VfxPositions.TransformPosition:
                    pt = trans;
                    return trans.position;
            }

            return Vector3.zero;
        }
    }
}