using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ROI
{

    public class GroundSlam : BaseActiveAbilityCard
    {
        [SerializeField] private float damageDeal = 150;
        [SerializeField] private int numAttackDecreaseAttackSpeed = 3;
        private GameObject AOEFx;
        [SerializeField] private AudioClip skillSound;
        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
            instantiateObject.TargetPosition = _championData.transform.position + _championData.transform.forward;
            instantiateObject.GetDynamicObject.RemoveListener(GetDynamicObject);
            instantiateObject.GetDynamicObject.AddListener(GetDynamicObject);
            foreach (var championData in championsEffectBySkill)
            {
                if (!championData.IsDeath && _championData.enemies.Contains(championData))
                {
                    championData.handles.OnAttackEvents.Add(new ApplyAttackSpeedByAttackNumber(championData, numAttackDecreaseAttackSpeed, "GroundSlamDecreaseAtKSpeed"));
                }
            }
        }

        public void GetDynamicObject(GameObject obj)
        {
            this.AOEFx = obj;
            obj.transform.position = _championData.transform.position;
            this.AOEFx.transform.forward = targetPosition - AOEFx.transform.position;
        }
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            
            _championData.transform.DOLookAt(targetPosition, .1f, AxisConstraint.X | AxisConstraint.Z).OnComplete(() =>
            {
                skillsPlayer.PlayFeedbacks();
                SoundManager.PlaySfxPrioritize(skillSound);
            });
           
            _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "GroundSlam");
        }
    }
}