using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class WormHole : BaseActiveAbilityCard, IOnDead
    {
        private GameObject AOEFx;
        private bool isServer;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            _championData.handles.OnDeads.Add(this);
            base.StartSkill(inputPosition, targets, isServer);
            this.isServer = isServer;
            ROI_Insaniatate portalTransformInstantiate = (ROI_Insaniatate)skillsPlayer.FeedbacksList[1];
            ROI_Insaniatate portalTargetInstantiate = (ROI_Insaniatate)skillsPlayer.FeedbacksList[2];
            portalTransformInstantiate.TargetPosition = _championData.transform.position;
            portalTargetInstantiate.TargetPosition = targetPosition;
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
        }

        public void ChangePosition()
        {
            Debug.Log("changePositionEvent");
            if(!isServer) return;
            foreach (var vChampionData in championsEffectBySkill)
            {
                if (!vChampionData.IsDeath && _championData.allies.Contains(vChampionData) && vChampionData.netId != _championData.netId)
                {
                    // TODO Swap position between two champion
                    var oldTranform = _championData.transform.position;
                    _championData.transform.position = vChampionData.transform.position;
                    vChampionData.transform.position = oldTranform;
                    vChampionData.controller.ResetHexPosition();
                    _championData.controller.ResetHexPosition();
                    break;
                }
                
            }
        }

        public void KnockBackEnemy()
        {
            if(!isServer) return;
            List<ChampionData> listEnemyEffectedBySkill = new List<ChampionData>();
            foreach (var vChampionData in championsEffectBySkill)
            {
                if (_championData.enemies.Contains(vChampionData))
                {
                    listEnemyEffectedBySkill.Add(vChampionData);
                }
            }
            
            _championData.ApplyEffectToChampionsBySkill(listEnemyEffectedBySkill, "WormHole", targetPosition);
        }

    }
}