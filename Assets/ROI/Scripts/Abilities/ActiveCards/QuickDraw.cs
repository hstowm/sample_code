using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class QuickDraw : BaseActiveAbilityCard
    {
        private ApplyAttackSpeedByAttackNumber _attackSpeedApplier;
        [SerializeField] private int numAttackApply = 3; 
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
            if(!isServer) return;
            if (_attackSpeedApplier != null && _championData.handles.OnAttackEvents.Contains(_attackSpeedApplier))
            {
                _attackSpeedApplier.numAttack = numAttackApply;
            }
            else
            {
                _attackSpeedApplier =
                        new ApplyAttackSpeedByAttackNumber(_championData, numAttackApply, "QuickDraw");
                _championData.handles.OnAttackEvents.Add(_attackSpeedApplier);
            }


        }
    }

    public class ApplyAttackSpeedByAttackNumber:IOnAttackEvent
    {
        private ChampionData _championData;
        public int numAttack;
        private StatusData _statusData;
        public ApplyAttackSpeedByAttackNumber(ChampionData championData, int numAttack, string keyName)
        {
            _championData = championData;
            this.numAttack = numAttack;
            /*this._championData.statModifier.ApplyModify(new StatTypeData(StatTypes.AttackSpeed,
                this.attackSpeedIncrease, StatValueTypes.Percent));*/
            _statusData = new StatusData(keyName, _championData, new Vector3());
            GeneralEffectSystem.Instance.ApplyEffect(_championData, _statusData);
        }

        public void OnAttackEvent()
        {
            if (numAttack-- == 0)
            {
                GeneralEffectSystem.Instance.RemoveEffect(_championData, _statusData);
                _championData.handles.OnAttackEvents.Remove(this);
            }
        }
    }
}

