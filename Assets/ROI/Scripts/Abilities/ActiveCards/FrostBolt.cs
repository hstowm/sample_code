using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class FrostBolt : BaseActiveAbilityCard
    {
        [SerializeField] private float exploreRadius = 1;
        [SerializeField] private float magicDamage = 50;
        private GameObject iceShard;
        public GameObject hitImpact;
        [SerializeField] private StatusSetting damageDealSetting;
        [SerializeField] private StatusSetting knockBackSetting;
        [SerializeField] private StatusSetting chilledSetting;
        [SerializeField] private AudioClip skillSound;
        private bool isServer;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            this.isServer = isServer;
            ROI_OverridePositionFeedback positionFeedback = (ROI_OverridePositionFeedback)skillsPlayer.FeedbacksList[2];
            ROI_Insaniatate instantiateObject = (ROI_Insaniatate)skillsPlayer.FeedbacksList[1];
            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
                skillsPlayer.PlayFeedbacks();
                SoundManager.PlaySfxPrioritize(skillSound);
            });
        }
        
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(InstantiateFireball);
            instantiateObject.GetDynamicObject.AddListener(InstantiateFireball);
        }
        
        public void InstantiateFireball(GameObject fireball)
        {
            this.iceShard = fireball;
            this.iceShard.transform.forward = targetPosition - iceShard.transform.position;
            fireball.GetComponent<ProjectileHitFirstChampion>().owner = _championData;
            fireball.GetComponent<ProjectileHitFirstChampion>().OnHit = target =>
            {
                GameObject.Instantiate(hitImpact, iceShard.transform.position + iceShard.transform.forward, iceShard.transform.rotation);
                iceShard.SetActive(false);
                iceShard.GetComponent<Collider>().isTrigger = false;
                if (isServer)
                    DealDamageToEnemy(target);
            };
            objectsDestroyOnSkillDone.Add(iceShard);
        }
        public void MoveFireBall(ROI_OverridePositionFeedback positionFeedback)
        {
            positionFeedback.DestinationPosition =
                _championData.transform.position + _championData.transform.forward * cardSkillData.maxCastRange;
            positionFeedback.AnimatePositionTarget = iceShard;
        }
        
        public void DealDamageToEnemy(ChampionData target)
        {
            List<ChampionData> championsHitBySkill = new List<ChampionData>(){target};
          
            _championData.ApplyEffectToChampionsBySkill(championsHitBySkill, damageDealSetting.name, Vector3.back);
            _championData.ApplyEffectToChampionsBySkill(championsHitBySkill, knockBackSetting.name, _championData.transform.position);
            _championData.ApplyEffectToChampionsBySkill(championsHitBySkill, chilledSetting.name, Vector3.back);
        }
    }
}