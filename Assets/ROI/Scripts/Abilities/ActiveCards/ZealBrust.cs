using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class ZealBrust : BaseActiveAbilityCard
    {
        
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            // TODO apply frenzy to champion
            var listChampionEffected = new List<ChampionData>();
            listChampionEffected.Add(_championData);
            skillsPlayer.PlayFeedbacks();
            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath && ally.netId != _championData.netId)
                {
                    // TODO apply frenzy to ally
                    listChampionEffected.Add(ally);
                }
            }
            if(isServer)
                _championData.ApplyEffectToChampionsBySkill(listChampionEffected, "Frenzy");
        }
    }
}