using System.Collections.Generic;
using UnityEngine;

namespace ROI
{

    public class EchoingAffinity : BaseActiveAbilityCard
    {
        
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            //TODO give champion status Engulfed 
            _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>{_championData}, "Engulfing");
            skillsPlayer.PlayFeedbacks();
            if(!isServer) return;
            foreach (var ally in _championData.allies)
            {
                if (ally.netId != _championData.netId && !ally.IsDeath)
                {
                    if (GeneralEffectSystem.ListEffectData.TryGetValue(_championData.netId, out _) == false)
                    {
                        return;
                    }
                    foreach (var effectData in GeneralEffectSystem.ListEffectData[_championData.netId])
                        {
                            if (effectData.type == StatusData.EffectType.Buff)
                            {
                                GeneralEffectSystem.Instance.ApplyEffect(ally, new StatusData(effectData.key_name, _championData, new Vector3()));
                            }
                        }   
                    
                }
            }
        }
    }
}