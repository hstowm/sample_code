// using System;
// using System.Collections;
// using System.Collections.Generic;
// using ROI;
// using UnityEngine;
//
// namespace ROI
// {
//     [Obsolete]
//     class ChampionBodyBlock2 : MonoBehaviour
//     {
//         public ChampionData player;
//         public ChampionData target;
//
//         public GameObject blockerGroups;
//
//         public List<ChampionData> blockers = new List<ChampionData>(16);
//
//         private void Awake()
//         {
//             var blocks = System.Array.Empty<ChampionData>();
//             if (blockerGroups)
//                 blocks = blockerGroups.GetComponentsInChildren<ChampionData>();
//
//             blockers.AddRange(blocks);
//         }
//
//
//         private void OnDrawGizmos()
//         {
//             if (Application.isPlaying == false || player == null || target == null || blockers.Count < 1)
//                 return;
//
//             Gizmos.color = Color.red;
//             Gizmos.DrawLine(player.CenterPosition, target.CenterPosition);
//
//             Gizmos.color = Color.yellow;
//
//             foreach (var block in blockers)
//             {
//                 if (IsBlocked(player, block, target) == false)
//                     continue;
//
//                 Gizmos.DrawLine(player.CenterPosition, block.CenterPosition);
//                 Gizmos.DrawLine(target.CenterPosition, block.CenterPosition);
//             }
//         }
//
//
//         /// <summary>
//         /// Moving to target and Check Is Blocked
//         /// </summary>
//         /// <returns></returns>
//         private bool IsBlocked(ChampionData champion, ChampionData blocker, ChampionData targetChampion)
//         {
//             var pos = champion.CenterPosition;
//             var target = targetChampion.CenterPosition;
//             var block = blocker.CenterPosition;
//
//             block.y = target.y = pos.y;
//
//             Logs.Info($"Check Block With  Agent({pos}), block({block}), Target({target}).. ");
//
//             var angle = Vector3.Angle(pos - block, target - block);
//
//             Logs.Info($"--Ange (ange -- block -- target) Is: {angle}. Next Check: {angle >= 95}");
//
//             if (angle < 95)
//                 return false;
//
//
//             var height = CalcHeight(pos, block, target);
//
//             Logs.Info(
//                 $"--Height: {height}, Radius: {champion.bodyRadius}: IsBlocked: {height < champion.bodyRadius}");
//
//             return height < champion.bodyRadius + blocker.bodyRadius;
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