// using UnityEngine;
//
// namespace ROI
// {
//
//     /// <summary>
//     /// Apply a knock with direction and strength in time
//     /// </summary>
//     class ChampionKnock : MonoBehaviour, IKnockApplier, IInjectInstance<ChampionData>
//     {
//         private readonly KnockData2 _knockData = new KnockData2();
// #if UNITY_EDITOR
//         [SerializeField] private float _testKnockTime = 0.5f;
//         [SerializeField] private float _testKnockStrength = 3;
// #endif
//         public void OnInject(ChampionData instance)
//         {
//             _knockData.championData = instance;
//             _knockData.trans = instance.transform;
//             _knockData.controller = instance.controller; //<ChampionController>();
//         }
// #if UNITY_EDITOR
//         [ContextMenu("Test knock Back")]
//         public void TestKnockBack()
//         {
//             var championData = gameObject.GetComponent<ChampionData>();
//
//             OnInject(championData);
//             ApplyKnock(-_knockData.trans.forward, _testKnockStrength, _testKnockTime);
//         }
//
//         [ContextMenu("Test knock Up")]
//         public void TestKnockUp()
//         {
//             var championData = gameObject.GetComponent<ChampionData>();
//
//             OnInject(championData);
//             ApplyKnock(Vector3.up, _testKnockStrength, _testKnockTime);
//         }
// #endif
//
//         public void ApplyKnock(Vector3 direction, float strength, float time)
//         {
//             // dont apply when die
//             if (_knockData.championData == null
//                 || _knockData.championData.IsDeath
//                 || strength <= 0 || time <= 0
//                 || _knockData.isKnocking)
//                 return;
//
//             var pos = _knockData.trans .position;
//
//             // offset center point
//             // var offset = _knockData.trans.InverseTransformPoint(pos);
//             _knockData.endPoint = direction.normalized * strength + pos;
//
//             var distance = Vector3.Distance(pos, _knockData.endPoint);
//
//             if (Mathf.Approximately(distance, strength) == false)
//                 Logs.Error("Calc end point offset Fail!!!");
//
//             // _knockData.remainTime = time;
//             _knockData.speed = strength / time;
//             _knockData.isKnocking = true;
//
//             _knockData.controller.Pause();
//         }
//
//         public void StopKnock()
//         {
//             _knockData.controller.Resume();
//             _knockData.isKnocking = false;
//         }
//
//         private void Update()
//         {
//             if (_knockData.isKnocking == false)
//                 return;
//
//             var movement = _knockData.speed * Time.deltaTime;
//             var pos = Vector3.MoveTowards(_knockData.trans.position, _knockData.endPoint, movement);
//
//             _knockData.trans.position = pos;
//
//             if (Vector3.Distance(pos, _knockData.endPoint) < 0.1f)
//             {
//                 _knockData.trans.position = _knockData.endPoint;
//                 StopKnock();
//             }
//             
//         }
//     }
// }