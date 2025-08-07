using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ROI
{
    public class ParalyzingVortex : BaseActiveAbilityCard
    {

        [SerializeField] private GameObject chanelObject;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                skillsPlayer.PlayFeedbacks();
                SoundManager.PlaySfxPrioritize(skillSound);
                if(isServer)
                    StartCoroutine(StartChanel());
            });
            MMF_Pause pause = (MMF_Pause)skillsPlayer.FeedbacksList[skillsPlayer.FeedbacksList.Count -2];
            pause.PauseDuration = cardSkillData.chanelTime;
            
        }

        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.GetDynamicObject.RemoveListener(GetShieldObject);
            instantiateObject.GetDynamicObject.AddListener(GetShieldObject);
            instantiateObject.TargetPosition = targetPosition;
        }

        public void GetShieldObject(GameObject shieldObj)
        {
            this.chanelObject = shieldObj;
            objectsDestroyOnSkillDone.Add(chanelObject);
        }
        
        private IEnumerator StartChanel()
        {
            // TODO apply 
            var timeCounting = 0f;
            while (timeCounting < cardSkillData.chanelTime)
            {
                List<ChampionData> championHitBySkill = new List<ChampionData>();
                foreach (var enemy in _championData.enemies)
                {
                    if (Vector3.Distance(enemy.transform.position,targetPosition) < cardSkillData.wide)
                    {
                        championHitBySkill.Add(enemy);
                    }
                }
                //TODO apply stunned for enemy for 0.1 sec
                _championData.ApplyEffectToChampionsBySkill(championHitBySkill, "ParalyzingVortex");

                timeCounting += 0.25f;
                yield return new WaitForSeconds(0.25f);
            }
        }

        protected override void HandleOnApplyCC()
        {
            StopAllCoroutines();
            CancelSkill();
        }
    }
}
