using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class DamageOvertimeEffect : DisplayableEffect, IEffectSystem
    {
        private float percentDamagePerSec;
        private float flatDamagePerSec;
        private bool isDealDamage = true;
        private ChampionData _championData;
        private StatusData _statusData;
        private DamageTypes _damageTypes;
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            StatusParam current_level = arg.GetCurrentParam();
            _championData = champion;
            _statusData = arg;
            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.DOT:
                        percentDamagePerSec = entry.Value;
                        break;
                    case StatusParamKeyWord.DOTPercent:
                        flatDamagePerSec = entry.Value;
                        break;
                    case StatusParamKeyWord.DOTType:
                        _damageTypes = (DamageTypes)entry.Value;
                        break;
                }
            }
            StartCoroutine(DealDamageOT());
        }

        private IEnumerator DealDamageOT()
        {
            while (isDealDamage)
            {
                float totalDamage = flatDamagePerSec + _championData.healthData.maxHealth*percentDamagePerSec;
                _statusData.creator.attacker.AttackEnemy(_statusData.target, totalDamage, DamageSources.Effect, _damageTypes);
                yield return new WaitForSeconds(1);
            }
        }
        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            isDealDamage = false;
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        }
    }
}