// using System;
// using UnityEngine;
//
// namespace ROI
// {
//     interface IOnApply
//     {
//         void OnApply();
//     }
//
//     interface IOnTimeout
//     {
//         void OnTimeout();
//     }
//
//     /// <summary>
//     /// Start update time from Apply time then call ITimeout when remaining time is zero.
//     /// </summary>
//     class TickOnTime : MonoBehaviour, IOnApply
//     {
//         [SerializeField] private float _duration = 10;
//         [SerializeField] private float _timePerTick = 1;
//
//         private float _remaining = 0;
//         private float _timeTemp;
//
//         private int _tickCount;
//
//         private IOnTimeout[] _onTimeouts;
//
//         private void Awake()
//         {
//             _onTimeouts = GetComponents<IOnTimeout>();
//             if (null == _onTimeouts)
//                 _onTimeouts = System.Array.Empty<IOnTimeout>();
//         }
//
//
//         [ContextMenu("Apply Tick On Time")]
//         public void OnApply()
//         {
//             _remaining = _duration;
//             _timeTemp = 0;
//             _tickCount = 0;
//         }
//
//         void Update()
//         {
//             if (_remaining <= 0 && _tickCount <= 0)
//                 return;
//
//             if (_remaining <= 0)
//             {
//                 if (_tickCount != Mathf.FloorToInt(_duration / _timePerTick))
//                     Logs.Error("Test tick on time fail");
//                 else
//                     Logs.Info("Test tick on time pass");
//
//                 _tickCount = 0;
//                 OnTimeout();
//                 return;
//             }
//
//             _timeTemp += Time.deltaTime;
//             if (_timeTemp >= _timePerTick)
//             {
//                 OnTick();
//                 _timeTemp -= _timePerTick;
//             }
//
//             _remaining -= Time.deltaTime;
//         }
//
//         void OnTick()
//         {
//             _tickCount++;
//
//             Logs.Info($"OnTick: {_tickCount}");
//         }
//
//         void OnTimeout()
//         {
//             foreach (var timeoutHandle in _onTimeouts)
//             {
//                 timeoutHandle.OnTimeout();
//             }
//         }
//     }
// }