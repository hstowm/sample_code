using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class Scoped : BaseActiveAbilityCard
    {
        [SerializeField] private StatusSetting skillDamage;
        private GameObject arrow;
        private ReticleSystem _reticleSystem;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            _reticleSystem = FindObjectOfType<ReticleSystem>(true);
            base.StartSkill(inputPosition, targets, isServer);
            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                skillsPlayer.PlayFeedbacks();
                StartCoroutine(ChanelToDealDamage(isServer));
            });
        }
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(InstantiateArrow);
            instantiateObject.GetDynamicObject.AddListener(InstantiateArrow);
        }

        public void InstantiateArrow(GameObject arrow)
        {
            this.arrow = arrow;
            arrow.transform.forward = targetPosition - arrow.transform.position;
            objectsDestroyOnSkillDone.Add(arrow);
        }
        public void MoveFireBall(ROI_OverridePositionFeedback positionFeedback)
        {
            positionFeedback.DestinationPosition =
                _championData.transform.position + _championData.transform.forward * cardSkillData.maxCastRange;
            positionFeedback.AnimatePositionTarget = arrow;
        }

        public IEnumerator ChanelToDealDamage(bool isServer)
        {
            yield return new WaitForSeconds(cardSkillData.chanelTime);
            SoundManager.PlaySfxPrioritize(skillSound);
            if(isServer)
                DealDamageToEnemy();
        }
        public void DealDamageToEnemy()
        {
            List<ChampionData> enemiesHitSkill = new List<ChampionData>();
            var indicator = _reticleSystem.InitReticle(_championData, cardSkillData);
            indicator.UpdateTarget(targetPosition);

            if (indicator.IsValid == false)
                return;

            enemiesHitSkill = indicator.ListTargets;
            if (enemiesHitSkill == null)
                enemiesHitSkill = new List<ChampionData>();
            _championData.ApplyEffectToChampionsBySkill(enemiesHitSkill, skillDamage.name);
        }

        protected override void HandleOnApplyCC()
        {
            StopAllCoroutines();
            CancelSkill();
        }
    }

}
