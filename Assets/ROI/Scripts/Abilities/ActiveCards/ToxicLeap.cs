using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class ToxicLeap : BaseActiveAbilityCard
    {
        [SerializeField] private float timeKnockUp = 0.5f;
        private GameObject AOEFx;
        [SerializeField] private StatusSetting skillDamage;
        [SerializeField] private StatusSetting skillKnockUp;
        [SerializeField] private StatusSetting skillPoison;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            _championData.transform.DOJump(targetPosition, 8, 1, 0.5f).onComplete = () =>
            {
                if (isServer)
                {
                    var listEnemyHitSkill = new List<ChampionData>();
                    foreach (var championData in championsEffectBySkill)
                    {
                        if (championData != null && !championData.IsDeath)
                        {
                            listEnemyHitSkill.Add(championData);
                        }
                    }
                    _championData.controller.ResetHexPosition();
                    _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, skillDamage.name);
                    _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, skillKnockUp.name, targetPosition);
                    _championData.ApplyEffectToChampionsBySkill(listEnemyHitSkill, skillPoison.name, targetPosition);
                }
            };
        }
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = targetPosition;
            instantiateObject.GetDynamicObject.RemoveListener(GetDynamicObject);
            instantiateObject.GetDynamicObject.AddListener(GetDynamicObject);
        }

        public void GetDynamicObject(GameObject shieldObj)
        {
            shieldObj.transform.position = targetPosition;
            this.AOEFx = shieldObj;
        }
        
    
    }

}
