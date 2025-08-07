using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class GoldenOpportunity : BaseActiveAbilityCard
    {
        
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
            if (!isServer) return;
            if (_championData.healthData.health > _championData.healthData.maxHealth / 2)
            {
                var listEnemyHitSkill = new List<ChampionData>();
                foreach (var championData in championsEffectBySkill)
                {
                    if (championData != null && !championData.IsDeath)
                    {
                        if (_championData.enemies.Contains(championData))
                        {
                            listEnemyHitSkill.Add(championData);
                        }
                    }
                }
                _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, "Chilled");
            }
            else
            {
                _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>(){_championData}, "Bless");
                //TODO Apply blessed itself
            }

        }
        
    }
    
}

