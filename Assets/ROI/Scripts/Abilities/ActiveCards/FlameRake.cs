using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class FlameRake : BaseActiveAbilityCard
    {
        private GameObject fireChain;
        [SerializeField] private AudioClip skillAudio;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);

            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                skillsPlayer.PlayFeedbacks();
            });
            SoundManager.PlaySfxPrioritize(skillAudio);
            if(isServer)
                DealDamageToEnemy();
        }

        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(GetShieldObject);
            instantiateObject.GetDynamicObject.AddListener(GetShieldObject);
        }
        public void GetShieldObject(GameObject shieldObj)
        {
            this.fireChain = shieldObj;
            this.fireChain.transform.forward = targetPosition - fireChain.transform.position;
        }
        
        public void DealDamageToEnemy()
        {
            _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "FlameRake");
            _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "FlameRakePull", _championData.transform.position);
        }
    }
}