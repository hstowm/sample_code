using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ROI
{

    public class Divinity : BaseActiveAbilityCard
    {

        [SerializeField] private GameObject chanelObject;
        [SerializeField] private GameObject lightEffect;
        private bool isServer = false;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            this.isServer = isServer;
            MMF_Pause pause = (MMF_Pause)skillsPlayer.FeedbacksList[skillsPlayer.FeedbacksList.Count -2];
            pause.PauseDuration = cardSkillData.chanelTime;
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            StartCoroutine(StartChanel(isServer));
        }

        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.GetDynamicObject.RemoveListener(GetShieldObject);
            instantiateObject.GetDynamicObject.AddListener(GetShieldObject);
            instantiateObject.ParentTransform = _championData.transform;
            instantiateObject.TargetTransform = _championData.transform;
        }

        public void GetShieldObject(GameObject shieldObj)
        {
            this.chanelObject = shieldObj;
            objectsDestroyOnSkillDone.Add(shieldObj);
        }
        
        private IEnumerator StartChanel(bool isServer)
        {
            // TODO 
            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath && ally.netId != _championData.netId)
                {
                    objectsDestroyOnSkillDone.Add(Instantiate(lightEffect, ally.transform));
                    if(isServer) //RemoveAllDebuff(ally);
                        GeneralEffectSystem.Instance.applyEffectActions[ally.netId] += RemoveDebuffStatus;
                }
            }
            yield return new WaitForSeconds(cardSkillData.chanelTime);
            if (isServer)
            {
                CancelSkill();
            }
        }

        protected override void CancelSkill()
        {
            base.CancelSkill();
            if(!isServer) return;
            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath && ally.netId != _championData.netId)
                {
                    //RemoveAllDebuff(_championData);
                    GeneralEffectSystem.Instance.applyEffectActions[ally.netId] -= RemoveDebuffStatus;
                }
            }
        }


        public void RemoveAllDebuff(ChampionData champion)
        {
            if (GeneralEffectSystem.ListEffectData.TryGetValue(champion.netId, out _) == false)
            {
                return;
            }
            foreach (var effectsData in GeneralEffectSystem.ListEffectData[champion.netId])
                {
                    if(effectsData.type == StatusData.EffectType.DeBuff)
                    {
                        GeneralEffectSystem.Instance.RemoveEffect(champion, new StatusData(effectsData.key_name, champion, effectsData.position));
                    }
                }
            
        }

        public void RemoveDebuffStatus(StatusData data)
        {
            if (data.type == StatusData.EffectType.DeBuff)
            {
                data.remain_duration = data.remain_duration_unscaled = 0;
                Debug.Log($"remove de-buff on champion: {data.key_name}");
                GeneralEffectSystem.Instance.RemoveEffect(data.target, data);
            }
        }

        protected override void HandleOnApplyCC()
        {
            StopAllCoroutines();
            CancelSkill();
        }
    }
}