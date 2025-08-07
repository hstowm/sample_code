using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace ROI
{
    public class RoughUp : BaseActiveAbilityCard
    {
        public StatusSetting statusRoughUp;
        MMF_Looper loop;
        int countLoop = 0;
        bool isServer;
        List<ChampionData> listEnemy = new List<ChampionData>();
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            this.isServer = isServer;
            loop = (MMF_Looper)skillsPlayer.FeedbacksList.Find(feedback =>
      feedback.GetType() == typeof(MMF_Looper));
            loop.NumberOfLoops = _championData.enemies.Count;
            skillsPlayer.PlayFeedbacks();
            countLoop = 0;
            SoundManager.PlaySfxPrioritize(skillSound);
            listEnemy.Clear();
            for (int i = 0; i < _championData.enemies.Count; i++)
            {
                listEnemy.Add(_championData.enemies[i]);
            }
            Sort();
            _championData.gameObject.SetActive(false);
        }

        public void Sort()
        {
            for (int i = 0; i < listEnemy.Count-1; i++)
            {
                for (int j = i+1; j < listEnemy.Count; j++)
                {
                    float distance1 = Vector3.Distance(_championData.transform.position, listEnemy[i].transform.position);
                    float distance2 = Vector3.Distance(_championData.transform.position, listEnemy[j].transform.position);
                    if (distance1 > distance2)
                    {
                        ChampionData swap = listEnemy[i];
                        listEnemy[i] = listEnemy[j];
                        listEnemy[j] = swap;
                    }
                }
            }
        }

        public void GetDynamicObjectDash(GameObject dash)
        {
            if (listEnemy[countLoop] != null && !listEnemy[countLoop].IsDeath)
            {
                dash.transform.LookAt(listEnemy[countLoop].transform);
            }
        }

        public void OnInitDash(ROI_Insaniatate instantiateObject)
        {
         //   instantiateObject.TargetTransform = _championData.transform;
            //instantiateObject.ParentTransform = _championData.transform;
            instantiateObject.TargetPosition = _championData.transform.position;
        }

        public void OnInitImpact(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position;
        }

        public void GetDynamicParamester(ROI_OverridePositionFeedback _positionFeedback)
        {
            _positionFeedback.AnimatePositionTarget = _championData.gameObject;
            _positionFeedback.InitialPosition = _championData.transform.position;
            if (listEnemy[countLoop] != null && !listEnemy[countLoop].IsDeath)
            {
                _positionFeedback.DestinationPosition = listEnemy[countLoop].transform.position;
                DealDame(listEnemy[countLoop]);
            }
            countLoop++;
        }

        public void DealDame(ChampionData enemyData)
        {
            if(isServer)
            GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData(statusRoughUp.name, _championData, Vector3.zero));
        }

        protected override void CancelSkill()
        {
            base.CancelSkill();
            _championData.gameObject.SetActive(true);
        }
    }
}
