using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{
    public class VenomousAffliction : BaseActiveAbilityCard
    {
        [SerializeField] private int explosionExistTime = 10;
        private GameObject posionAOE;
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
                _championData.ApplyEffectToChampionsBySkill(targets, "Poisoned");
        }

        public IEnumerator IncreaseManaOnChampionSkill()
        {
            yield return new WaitForSeconds(explosionExistTime);
        }
        
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(GetDynamicObject);
            instantiateObject.GetDynamicObject.AddListener(GetDynamicObject);
        }

        public void GetDynamicObject(GameObject obj)
        {
            posionAOE = obj;
            posionAOE.transform.position = targetPosition;
            
            StartCoroutine(DestroyExplosion(posionAOE));
        }
        public IEnumerator DestroyExplosion(GameObject explosion)
        {
            foreach (var championData in championsEffectBySkill)
            {
                championData.AddManaCostBonus(1);
            }
            
            yield return new WaitForSeconds(explosionExistTime);
            Destroy(explosion);
            foreach (var championData in championsEffectBySkill)
            {
                championData.AddManaCostBonus(-1);
            }
        }
    }
}