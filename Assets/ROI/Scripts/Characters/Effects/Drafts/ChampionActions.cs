// namespace ROI
// {
//     using UnityEngine;
//     
//     interface IChampionOnAction {
//         void OnAction();
//     }
//
//
//     class ChampionActions : MonoBehaviour
//     {
//         
//     }
//     
//     class ChampionActionConditions : MonoBehaviour, IInjectInstance<ChampionData>
//     {
//         private ChampionData _championData;
//
//         [SerializeField]private ChampionActionStates _triggerActionStates = ChampionActionStates.None;
//
//         private IChampionOnAction[] _championOnActions;
//         
//         private void Awake()
//         {
//             _championOnActions =
//                 GetComponentsInChildren<IChampionOnAction>() ?? System.Array.Empty<IChampionOnAction>();
//         }
//
//         public void OnInject(ChampionData instance)
//         {
//             _championData = instance;
//             _championData.onAction -= OnAction;
//             _championData.onAction += OnAction;
//         }
//         
//         private void OnAction(ChampionActionStates actionState)
//         {
//             if(actionState != _triggerActionStates)
//                 return;
//
//             foreach (var action in _championOnActions)
//             {
//                 action.OnAction();
//             }
//         }
//         
//     }
// }