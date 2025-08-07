using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROI
{


    public class HealOverTimeEffect : DisplayableEffect, IEffectSystem
    {
        private float percentHeal;
        private float flatHealPerSec;
        private bool isHeal = true;
        private ChampionData _championData;
        private ChampionDamageText _championDamageText;
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            StatusParam current_level = arg.GetCurrentParam();
            _championDamageText = ChampionDamageText.instance;
            _championData = champion;
            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.HealFlat:
                        flatHealPerSec = entry.Value;
                        break;
                    case StatusParamKeyWord.HealPercent:
                        percentHeal = entry.Value;
                        break;
                }
            }
            StartCoroutine(HealOT());
        }

        private IEnumerator HealOT()
        {
            isHeal = true;
            while (isHeal)
            {
                float totalHeal = flatHealPerSec + _championData.healthData.maxHealth*percentHeal;
                _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, totalHeal));
                _championDamageText.ShowHealDamage(_championData, (int)totalHeal);
                ApplyVFX(_championData);
                yield return new WaitForSeconds(1);
            }
        }
        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            isHeal = false;
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        }
    }
}