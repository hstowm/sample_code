using System.Collections.Generic;
using UnityEngine;

namespace ROI
{

    public class Empowerment : BaseActiveAbilityCard
    {
        private GameObject AOEFx;
        [SerializeField] private AudioClip skillSound;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            if (isServer)
            {
                ApplySkillEffect();
            }

        }
        public void SpawnExplosion(GameObject AoeFx)
        {
            AoeFx.transform.position = targetPosition;
            this.AOEFx = AoeFx;
        }

        public void ApplySkillEffect()
        {
            List<ChampionData> alliesHitBySkill = new List<ChampionData>();
            List<ChampionData> enemiesHitBySkill = new List<ChampionData>();
            foreach (var vChampionData in championsEffectBySkill)
            {
                if (_championData.allies.Contains(vChampionData))
                {
                    alliesHitBySkill.Add(vChampionData);
                }
                else
                {
                    // TODO Knock back enemy to border of radius 
                    enemiesHitBySkill.Add(vChampionData);
                }
            }
            _championData.ApplyEffectToChampionsBySkill(alliesHitBySkill, "EmpowermentHealth");
            _championData.ApplyEffectToChampionsBySkill(enemiesHitBySkill, "EmpowermentKnockBack");
        }
        
    }
}