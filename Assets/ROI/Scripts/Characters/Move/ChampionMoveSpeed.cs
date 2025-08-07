using Mirror;
namespace ROI
{
    public class ChampionMoveSpeed : NetworkBehaviour, IOnStartMove, IOnStopMove
    {
        private ChampionData _championData;
        private float _originBodySize = -1;

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }

        // public ChampionMoveSpeed(ChampionData championData, bool isServer)
        // {
        //     _championData = championData;
        //     
        //     _championData.handles.OnStartMoves.Add(this);
        //     _championData.handles.OnStopMoves.Add(this);
        // }

        /// <summary>
        ///  Start moving with move Speed
        /// </summary>
        /// <param name="moveSpeed">Unit Per Second</param>
        public void SetMoveSpeed(float moveSpeed)
        {
            if (isServer == false)
            {
                Logs.Error("cant set move speed in client side");
                return;
            }
            
            if (moveSpeed < 0)
            {
                Logs.Warning($"Cant Set Move Speed to less zero: {moveSpeed}");
                return;
            }
            
            // Logs.Warning($"Champion:{_championData.name}, Animator:{(null == _championData.animatorNetwork.animator ? "NULL": "NOT NULL")}");
            
            // Debug.Log("SetMoveSpeed");
            // _championData.moveData.moveSpeed = moveSpeed;
            var animMoveSpeed = CalcMoveAnimMultiplier(moveSpeed);
            _championData.animatorNetwork.animator.SetFloat(AnimHashIDs.MoveSpeed, animMoveSpeed);
        }


        public void OnMoveSpeedChanged()
        {
            if(_championData.state != ChampionStates.Moving)
                return;

            OnStartMove();
        }

        /// <summary>
        /// Convert champion's move speed (meter per second) to animation's speed multiplier
        /// </summary>
        /// <param name="moveSpeed">Unit per second</param>
        /// <returns>Animation Move Speed Multiplier</returns>
        private float CalcMoveAnimMultiplier(float moveSpeed)
        {
            if (moveSpeed == 0)
                return 0;

            // current move speed: meter per second( meter per frame * frame per second)
            var meterPerFrame = _championData.moveAnim.totalMeterPerAnim / _championData.moveAnim.animFrames;
            var currentMoveSpeed = meterPerFrame * _championData.moveAnim.animSpeed;

            if (currentMoveSpeed == 0)
            {
                return 0;
            }

            return moveSpeed / currentMoveSpeed; //_championData.moveAnim.animSpeed;
        }

        public void OnStartMove()
        {
            if (isServer == false)
                return;

            if (_originBodySize < 0)
                _originBodySize = _championData.agent.bodyRadius;

            _championData.agent.bodyRadius = _originBodySize * 0.8f;
            SetMoveSpeed(_championData.moveData.moveSpeed);
        }

        public void OnStopMove()
        {
            if (isServer == false)
                return;

            if (_originBodySize > 0)
                _championData.agent.bodyRadius = _originBodySize;
            SetMoveSpeed(0);
        }
    }
}