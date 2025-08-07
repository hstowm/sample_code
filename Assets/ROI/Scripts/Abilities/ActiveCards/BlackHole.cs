using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class BlackHole : BaseActiveAbilityCard
    {
        [SerializeField] private float timeFear = 2;
        private GameObject AOEFx;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            if (isServer)
            {
                _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "BlackHole");
                _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "Vulnerable");
            }
        }

        public void SpawnExplosion(GameObject AoeFx)
        {
            AoeFx.transform.position = targetPosition;
            this.AOEFx = AoeFx;
        }
    }
}