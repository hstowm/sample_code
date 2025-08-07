// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ROI
// {
//     interface IChampionModifier
//     {
//         ChampionData Champion { set; }
//     }
//
//
// // check effect condition
//     interface IEffectCondition : IChampionModifier
//     {
//         bool IsValid();
//     }
//
// // Applying the effect
//     interface IEffectModifier : IChampionModifier
//     {
//         void OnApply();
//         bool OnUpdate();
//         void OnRemoved();
//     }
//
//     interface IEffect
//     {
//         void OnApply();
//         bool OnUpdate();
//
//         void OnRemoved();
//     }
//
//
// // Effect Controller
//
//     /// <summary>
//     /// Effect controller
//     /// </summary>
//     [RequireComponent(typeof(ChampionData))]
//     class EffectController : MonoBehaviour, IEffect
//     {
//         private IEffectCondition[] _effectConditions;
//         private IEffectModifier[] _effectModifiers;
//         private ChampionData _championData;
//
//         void Awake()
//         {
//             _championData = GetComponent<ChampionData>();
//             _effectConditions = GetComponentsInChildren<IEffectCondition>() ?? System.Array.Empty<IEffectCondition>();
//             _effectModifiers = GetComponentsInChildren<IEffectModifier>() ?? System.Array.Empty<IEffectModifier>();
//
//             foreach (var effect in _effectConditions)
//                 effect.Champion = _championData;
//             foreach (var effect in _effectModifiers)
//                 effect.Champion = _championData;
//         }
//
//         public void OnApply()
//         {
//             foreach (var modifier in _effectModifiers)
//                 modifier.OnApply();
//         }
//
//         public void OnRemoved()
//         {
//             foreach (var modifier in _effectModifiers)
//                 modifier.OnRemoved();
//         }
//
//         public bool OnUpdate()
//         {
//             foreach (var conds in _effectConditions)
//                 if (conds.IsValid())
//                     return true;
//
//             var needUpdate = false; 
//             foreach (var modifier in _effectModifiers)
//             {
//                 needUpdate |= modifier.OnUpdate();
//             }
//
//             return needUpdate;
//         }
//     }
//
//   
// }