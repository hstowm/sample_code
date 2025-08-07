using System.Runtime.CompilerServices;
using UnityEngine;
using Mirror;

namespace ROI
{
    class ChampionAttackAnimEvents : NetworkBehaviour
    {
        private Transform _transform;

        private ChampionData _championData;

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }

        /// <summary>
        /// OnAttack Animation Event
        /// </summary>
        public void OnAttack()
        {
            if (isServer == false)
                return;

            RpcOnAttack();
        }

        [ClientRpc, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RpcOnAttack()
        {
            var onAttackEvts = _championData.handles.OnAttackEvents.ToArray();
            foreach (var attackEvent in onAttackEvts)
            {
                attackEvent.OnAttackEvent();
            }
        }


        /// <summary>
        /// On Start Animation Of Attack Animation. call event in anim sever, all client execute create vfx
        /// </summary>
        public void OnStartAttack()
        {
            if (false == isServer)
                return;

            RpcOnStartAttack();
        }

        [ClientRpc, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RpcOnStartAttack()
        {
            foreach (var attackEvent in _championData.handles.OnStartAttackEvents)
            {
                attackEvent.OnStartAttack();
            }
        }
        
        // /// <summary>
        // /// On End Animation Of Attack Animation
        // /// </summary>
        // public void OnEndAttack()
        // {
        //     if (!isServer)
        //         return;
        //
        //     RpcOnEndAttack();
        // }
        //
        // [ClientRpc, MethodImpl(MethodImplOptions.AggressiveInlining)]
        // private void RpcOnEndAttack()
        // {
        //     foreach (var attackEvent in _championData.handles.OnEndAttackEvents)
        //     {
        //         attackEvent.OnEndAttack();
        //     }
        // }
    }
}