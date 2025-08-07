// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace ROI
// {
//     interface IEffectModifier2
//     {
//         ///<summary>
//         /// On First Apply the effect on champion
//         ///</summary>
//         void OnApply();
//
//         /// <summary>
//         /// Update
//         /// </summary>
//         /// <returns></returns>
//         bool OnUpdate();
//
//         ///<summary>
//         /// On Remove the effect on champion
//         ///</summary>
//         void OnRemove();
//     }
//
//     /// <summary>
//     /// Apply A damage to champion on time 
//     /// </summary>
//     class DamageOverTimeEffect : IEffectModifier2
//     {
//         private float _damagePerSecond;
//         private float _duration;
//
//         public DamageOverTimeEffect(int duration, float damagePerSecond)
//         {
//             _duration = duration;
//             _damagePerSecond = damagePerSecond;
//         }
//
//         public ChampionEffects EffectType => ChampionEffects.Blessed;
//
//         public void OnApply()
//         {
//         }
//
//         public bool OnUpdate()
//         {
//             return _duration <= 0;
//         }
//
//         public void OnRemove()
//         {
//         }
//     }
//
//     /// <summary>
//     /// Contain all current effects of champion
//     /// </summary>
//     class ChampionEffectData
//     {
//         public readonly List<IEffectModifier2> effectModifiers = new List<IEffectModifier2>();
//     }
//
//     /// <summary>
//     /// Champion Effect System
//     /// </summary>
//     class ChampionEffectSystem2 : MonoBehaviour
//     {
//         private readonly ChampionEffectData _effectData = new ChampionEffectData();
//
//         public void AddEffect(IEffectModifier2 effectModifier)
//         {
//             _effectData.effectModifiers.Add(effectModifier);
//             effectModifier.OnApply();
//         }
//
//         private void Update()
//         {
//             int count = _effectData.effectModifiers.Count;
//             for (int i = count - 1; i > -1; i--)
//             {
//                 if (_effectData.effectModifiers[i].OnUpdate())
//                     continue;
//
//                 OnRemove(i);
//             }
//         }
//
//         private void OnRemove(int index)
//         {
//             _effectData.effectModifiers[index].OnRemove();
//             _effectData.effectModifiers.RemoveAt(index);
//         }
//     }
// }