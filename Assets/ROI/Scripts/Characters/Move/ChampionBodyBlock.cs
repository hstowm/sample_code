// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ROI
// {
//     [Obsolete]
//     [RequireComponent(typeof(ChampionData))]
//     class ChampionBodyBlock : MonoBehaviour
//     {
//         private ChampionData _championData;
//         private Collider[] _colliders;
//
//         private int _instanceID;
//         private Transform _transform;
//
//         //[SerializeField] private float _blockDelayTime = 0.5f;
//
//         private ChampionData _blocker;
//         private float _blockRemainTime;
//         private ChampionMovement _championMovement;
//
//         // private RaycastHit[] _raycastHits;
//
//         private void Awake()
//         {
//             _championData = GetComponent<ChampionData>();
//             _championMovement = GetComponent<ChampionMovement>();
//             _transform = transform;
//
//             _colliders = new Collider[8];
//             //  _raycastHits = new RaycastHit[10];
//             _instanceID = gameObject.GetInstanceID();
//         }
//
//         private List<Vector3> _targetPositions = new List<Vector3>(10);
//
//         /// <summary>
//         /// Blocked by champion others
//         /// </summary>
//         /// <returns></returns>
//         public bool IsBlocked(out Vector3 avoidPos)
//         {
//             // if (_blockRemainTime > 0)
//             // {
//             //     _blockRemainTime -= Time.deltaTime;
//             //     return true;
//             // }
//
//             var range = CalcRadiusVisible(out var center);
//
//             var resultSize =
//                 Physics.OverlapSphereNonAlloc(center, range, _colliders, GameLayers.Champion);
//
//             _targetPositions.Clear();
//             var y = _transform.position.y;
//
//             for (int i = 0; i < resultSize; i++)
//             {
//                 var go = _colliders[i].gameObject;
//
//                 // is not ally or self
//                 if (IsAllyChampion(go) == false || go.GetInstanceID() == _instanceID)
//                     continue;
//
//               
//                 var blocker = go.GetComponent<ChampionData>();
//
//                 if (IsBlocked(blocker) == false)
//                     continue;
//
//
//                 var side = GetSidePosition(blocker);
//                 side.y = y;
//                 _targetPositions.Add(side);
//
//                 side = GetSidePosition(blocker, true);
//                 side.y = y;
//                 _targetPositions.Add(side);
//             }
//
//             avoidPos = GetBestPosition();
//             avoidPos.y = y;
//
//             return _targetPositions.Count > 0;
//         }
//
//         Vector3 GetBestPosition()
//         {
//             var centerPos = _championData.CenterPosition;
//
//             if (_targetPositions.Count == 0)
//             {
//                 return centerPos;
//             }
//
//             var index = 0;
//             var minDistance = Vector3.Distance(centerPos, _targetPositions[index]);
//
//             for (var i = 0; i < _targetPositions.Count; i++)
//             {
//                 var distance = Vector3.Distance(centerPos, _targetPositions[i]);
//                 if (minDistance > distance)
//                 {
//                     minDistance = distance;
//                     index = i;
//                 }
//             }
//
//             return _targetPositions[index];
//         }
//
//         float CalcRadiusVisible(out Vector3 center)
//         {
//             // default range is max x;
//             center = _championData.CenterPosition;
//             if (_championData.HaveTarget == false)
//                 return 5 + _championData.bodyRadius;
//
//             var pos = _transform.position;
//             var targetPos = _championData.target.transform.position;
//             targetPos.y = pos.y;
//
//             center = new Vector3(pos.x + targetPos.x, 0, pos.z + targetPos.z) * 0.5f;
//
//             return Vector3.Distance(pos, targetPos) * 0.5f + _championData.bodyRadius;
//         }
//
//         /// <summary>
//         /// same tag
//         /// </summary>
//         /// <param name="go"></param>
//         /// <returns></returns>
//         private bool IsAllyChampion(GameObject go)
//         {
//             var tg = _championData.gameObject.tag;
//
//             return go.CompareTag(tg);
//         }
//
//         private Vector3 GetSidePosition(ChampionData target, bool isLeftSide = false)
//         {
//             var centerPos = target.CenterPosition;
//             centerPos.x += (isLeftSide ? -1 : 1) * (target.bodyRadius + _championData.bodyRadius) * 1.5f;
//
//             return centerPos; //championData.transform.TransformPoint(sideOffset);
//         }
//
//         private void OnDrawGizmosSelected()
//         {
//             if (Application.isPlaying == false || _championData.IsDeath || _championData.HaveTarget == false)
//                 return;
//
//             Gizmos.color = Color.green;
//             var radius = CalcRadiusVisible(out var center);
//             Gizmos.DrawWireSphere(center, radius);
//
//             Gizmos.color = Color.yellow;
//             var side = GetSidePosition(_championData.target);
//             Gizmos.DrawWireSphere(side, 0.02f);
//
//             Gizmos.color = Color.red;
//             side = GetSidePosition(_championData.target, true);
//             Gizmos.DrawWireSphere(side, 0.02f);
//         }
//
//
//         /// <summary>
//         /// Moving to target and Check Is Blocked
//         /// </summary>
//         /// <param name="blocker"></param>
//         /// <returns></returns>
//         private bool IsBlocked(ChampionData blocker)
//         {
//             var pos = _championData.CenterPosition;
//             var target = _championData.target.CenterPosition;
//             var block = blocker.CenterPosition;
//
//             block.y = target.y = pos.y;
//
//             //Logs.Info($"Check Block With  Agent({pos}), block({block}), Target({target}).. ");
//
//             var angle = Vector3.Angle(pos - block, target - block);
//
//             //Logs.Info($"--Ange (ange -- block -- target) Is: {angle}. Next Check: {angle >= 95}");
//
//             if (angle < 95)
//                 return false;
//
//
//             var height = CalcHeight(pos, block, target);
//
//             //Logs.Info(
//             //    $"--Height: {height}, Radius: {_championData.radius}: IsBlocked: {height < _championData.radius}");
//
//             return height < _championData.bodyRadius;
//         }
//
//         public float CalcHeight(Vector3 agent, Vector3 blocker, Vector3 target)
//         {
//             return 2f * S(agent, blocker, target) / (agent - target).magnitude;
//         }
//
//         public float S(Vector3 a, Vector3 b, Vector3 c)
//         {
//             return S((b - c).magnitude, (a - c).magnitude, (a - b).magnitude);
//         }
//
//         public float S(float a, float b, float c)
//         {
//             var p = 0.5f * (a + b + c);
//             return Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
//         }
//     }
// }