using System;
using UnityEngine;

namespace ROI
{
    
    /// <summary>
    /// Recycle Game Object On Dead handle
    /// </summary>
    class RecycleGameObjectOnDead : IOnDead, IEquatable<RecycleGameObjectOnDead>
    {
        private readonly GameObject _gameObject;

        public void OnDead()
        {
            if (_gameObject)
                _gameObject.Recycle();
        }

        public RecycleGameObjectOnDead(GameObject gameObject)
        {
            _gameObject = gameObject;
        }


        public bool Equals(RecycleGameObjectOnDead other)
        {
            return other != null && _gameObject == other._gameObject;
        }
    }
}