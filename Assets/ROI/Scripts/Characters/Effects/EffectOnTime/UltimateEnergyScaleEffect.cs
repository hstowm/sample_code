using System.Collections.Generic;
using Mirror;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{

    public class UltimateEnergyScaleEffect : NetworkBehaviour, IEffectSystem,IOnUseCard
    {
        public float scaleUltimateEnergyPercent = 0f;
        private ChampionData _championData;
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            StatusParam current_level = arg.GetCurrentParam();
            _championData = champion;
            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.UltimateEnergyFlat:
                        scaleUltimateEnergyPercent = entry.Value;
                        break;
                    case StatusParamKeyWord.UltimateEnergyPercent:
                        break;
                }
            }  
            _championData.handles.OnUseCards.Add(this);
        }

        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            _championData.handles.OnUseCards.Remove(this);
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration_unscaled = arg.remain_duration = arg.setting.duration;
        }

        public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
        {
            _championData.AddBonusUltimateEnergy((int)scaleUltimateEnergyPercent);
        }
    }
}