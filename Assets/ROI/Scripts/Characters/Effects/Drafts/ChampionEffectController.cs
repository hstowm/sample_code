// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ROI
// {
//     /// <summary>
//     /// On Apply An effect
//     /// </summary>
//     interface IEffectController
//     {
//         void ApplyEffect();
//     }
//
//     /// <summary>
//     /// Add/Remove Apply all effects
//     /// </summary>
//     class ChampionEffectController : MonoBehaviour, IEffectController
//     {
//         private readonly List<IEffect> _effects = new List<IEffect>();
//
//         void Awake()
//         {
//             var passiveEffects = GetComponentsInChildren<IEffect>() ?? System.Array.Empty<IEffect>();
//             _effects.AddRange(passiveEffects);
//         }
//
//         public void ApplyEffect()
//         {
//             foreach (var effect in _effects)
//             {
//                 effect.OnApply();
//             }
//         }
//     }
// }