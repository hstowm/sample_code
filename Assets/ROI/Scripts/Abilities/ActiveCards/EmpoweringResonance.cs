using System;
using System.Collections;
using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    public class EmpoweringResonance : BaseActiveAbilityCard, ISkillCardTrigger
    {
        [SerializeField] private float damageIncreasePercentage;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            // _championData.GetComponent<ChampionController>().Pause();

            championsEffectBySkill = targets;
            Debug.Log("Start");
            foreach (var cham in _championData.allies)
            {
                Debug.Log(cham.name);
                cham.handles.OnHitEnemies.Add(new IncreaseDamage(damageIncreasePercentage));
            }
            StartCoroutine(EndSkill(cardSkillData.chanelTime));
        }

        IEnumerator EndSkill(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("End");
            foreach (var cham in _championData.allies)
            {
                cham.handles.OnHitEnemies.Remove(new IncreaseDamage(damageIncreasePercentage));
            }

            // _championData.GetComponent<ChampionController>().Resume();
            skillsPlayer.StopFeedbacks();
        }

    }

    public class IncreaseDamage : IOnHitEnemy
    {
        private float percentage;

        public IncreaseDamage(float percentage)
        {
            this.percentage = percentage;
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            damageDealtData.AddBonusDamage(percentage, StatValueType.Percent);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return obj is IncreaseDamage other && this.percentage == other.percentage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(percentage);
        }
    }
}