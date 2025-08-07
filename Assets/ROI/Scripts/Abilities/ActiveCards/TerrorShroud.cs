using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class TerrorShroud : BaseActiveAbilityCard
    {
        bool isServer;
        [SerializeField] private AudioClip skillSound;
        List<ChampionData> surroundingTargets = new List<ChampionData>();
        ChampionData mainTarget = null;
        ChampionData champion;
        public float radius = 3.5f;
        
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
            this.isServer = isServer;
            SoundManager.PlaySfxPrioritize(skillSound);
            this.champion = _championData;
            this.mainTarget = championsEffectBySkill[0] ?? null;
        }
        public void OnInitImpact(ROI_Insaniatate instantiateObject)
        {
            if (mainTarget)
            {
                var mainTargetPosition = mainTarget.transform.position;
                // Colliders with a capsule with height 5f and radius 5f
                var colliders = Physics.OverlapCapsule(mainTargetPosition, mainTargetPosition + new Vector3(0, 100f, 0), radius);
                surroundingTargets.Clear();
                foreach (var collider in colliders)
                {
                    var championData = collider.GetComponent<ChampionData>();
                    // Adds the champion to the surroundingTargets only if it's an enemy of the caster
                    if (championData != null && championData.IsPlayer != champion.IsPlayer)
                    {
                        surroundingTargets.Add(championData);
                    }
                    surroundingTargets.Remove(mainTarget);
                }
                Logs.Info($"TerrorShroud: surroundingTargets => {surroundingTargets.Count} {surroundingTargets}");

                instantiateObject.TargetPosition = mainTarget.transform.position;
                if (isServer)
                {
                    this.ApplyEffectOnMainTarget(mainTarget);
                    this.ApplyEffectOnSurroundingTargets(surroundingTargets);
                }
            }
        }

        public void ApplyEffectOnMainTarget(ChampionData enemyData)
        {
            GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData("Vulnerable", _championData, Vector3.zero));
        }
        
        public void ApplyEffectOnSurroundingTargets(List<ChampionData> enemiesData)
        {
            // Cycles through all enemies in the list and applies the effect to them
            foreach (var enemyData in enemiesData)
            {
                GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData("TerrorShroud_ApplyFear", _championData, Vector3.zero));
            }
        }
    }
}
