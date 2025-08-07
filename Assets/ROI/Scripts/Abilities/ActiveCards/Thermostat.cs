using System.Collections.Generic;
using UnityEngine;
namespace ROI
{
    public class Thermostat : BaseActiveAbilityCard
    {
        public StatusSetting engulfing;
        public StatusSetting chilled;
        bool isServer;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
            if (isServer)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] != null)
                    {
                        if (_championData.allies.Contains(targets[i]))
                        {
                            GeneralEffectSystem.Instance.ApplyEffect(targets[i], new StatusData(engulfing.name, targets[i], new Vector3()));
                        }
                        else
                        {
                            GeneralEffectSystem.Instance.ApplyEffect(targets[i], new StatusData(chilled.name, targets[i], new Vector3()));
                        }
                    }
                }
            }
        }

        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            if (championsEffectBySkill.Count > 0 && championsEffectBySkill[0] != null)
            {
                instantiateObject.TargetPosition = championsEffectBySkill[0].transform.position;
            }
            else
            {
                instantiateObject.TargetPosition = targetPosition;
            }
        }
    }


}
