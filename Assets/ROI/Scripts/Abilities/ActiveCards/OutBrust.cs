using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class OutBrust : BaseActiveAbilityCard
    {
        [SerializeField] private float knockBackDistance = 2;
        private GameObject AOEFx;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            if (!isServer) return;

            var listEnemyHitSkill = new List<ChampionData>();
            foreach (var championData in championsEffectBySkill)
            {
                if (championData != null && !championData.IsDeath)
                {
                    listEnemyHitSkill.Add(championData);
                }
            }

            _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, "Gust");
            _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, "GustKnockBack", targetPosition);
        }

        public void SpawnExplosion(GameObject AoeFx)
        {
            AoeFx.transform.position = targetPosition;
            this.AOEFx = AoeFx;
            //objectsDestroyOnSkillDone.Add(AOEFx);
        }
    }

}
