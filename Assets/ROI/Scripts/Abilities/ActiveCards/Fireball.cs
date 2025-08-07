using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class Fireball : BaseActiveAbilityCard
    {
        [SerializeField] private float exploreRadius = 3;
        private GameObject fireBall;
        public GameObject hitImpact;
        [SerializeField] private StatusSetting engulfSetting;
        [SerializeField] private StatusSetting damageDealSetting;
        private bool isServer;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            this.isServer = isServer;   
            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                skillsPlayer.PlayFeedbacks();
                _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>(){_championData}, engulfSetting.name);
            });
        }
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(InstantiateFireball);
            instantiateObject.GetDynamicObject.AddListener(InstantiateFireball);
            
            Logs.Warning($" instantiateObject.GetDynamicObject.AddListener(InstantiateFireball) count: {instantiateObject.GetDynamicObject.GetPersistentEventCount()}");
        }

        public void InstantiateFireball(GameObject fireball)
        {
            
            this.fireBall = fireball;
            this.fireBall.transform.forward = targetPosition - fireBall.transform.position;
            fireball.GetComponent<ProjectileHitFirstChampion>().owner = _championData;
            fireball.GetComponent<ProjectileHitFirstChampion>().OnHit = target =>
            {
                SoundManager.PlaySfxPrioritize(skillSound);
                Instantiate(hitImpact, target.transform.position, target.transform.rotation);
                fireBall.SetActive(false);
                fireBall.GetComponent<Collider>().isTrigger = false;
                if(isServer)
                    DealDamageToEnemy(target);
            };
            objectsDestroyOnSkillDone.Add(fireBall);
        }
        public void MoveFireBall(ROI_OverridePositionFeedback positionFeedback)
        {
            positionFeedback.DestinationPosition =
                _championData.transform.position + _championData.transform.forward * cardSkillData.maxCastRange;
            positionFeedback.AnimatePositionTarget = fireBall;
        }

        public void DealDamageToEnemy(ChampionData target)
        {
            List<ChampionData> enemiesHitSkill = new List<ChampionData>();
            enemiesHitSkill.Add(target);
            //Deals damage to all enemy 's allies in skill impact range
            foreach (var vaChampionData in target.allies)
            {
                if (!vaChampionData.IsDeath && vaChampionData.netId != target.netId && Vector3.Distance(vaChampionData.transform.position, target.transform.position)<= exploreRadius)
                {
                    enemiesHitSkill.Add(vaChampionData);
                }
            }
            Debug.Log($"Deal damage to enemy count: {enemiesHitSkill.Count}" );
            _championData.ApplyEffectToChampionsBySkill(enemiesHitSkill, damageDealSetting.name);
        }
        
    }
}