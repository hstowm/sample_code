using Mirror;

namespace ROI
{
    public class ChampionTargetSwitcher
    {
        private readonly ChampionData _championData;

        public ChampionTargetSwitcher(ChampionData championData)
        {
            _championData = championData;
        }

        [Server]
        public void SetTarget(ChampionData target)
        {
            if (!target)
                return;

            if (!_championData.target)
            {
                _championData.target = target;
                RpcOnTargetChanged(null);
                return;
            }

            if (_championData.target.Equals(target))
                return;

            // new target
            var prevTarget = _championData.target;
            _championData.target = target;
            RpcOnTargetChanged(prevTarget);
        }

        [ClientRpc]
        private void RpcOnTargetChanged(ChampionData prevTarget)
        {
            foreach (var handle in   _championData.handles.OnTargetChangeds)
            {
                handle.OnTargetChanged(prevTarget);
            }
        }
    }
}