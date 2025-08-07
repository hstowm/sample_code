using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROI
{


    public class WarChhant : BaseActiveAbilityCard, ISkillCardTrigger
    {

        [SerializeField] private float attackSpeedBonus;
        [SerializeField] private float healPercentage;
        [SerializeField] private DamageSources sources;
        [SerializeField] private float duration;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            skillsPlayer.PlayFeedbacks();
            StartCoroutine(StartSkill(attackSpeedBonus, healPercentage, duration, targets));
        }

        IEnumerator StartSkill(float attackSpeedBonus, float healPercentage, float duration, List<ChampionData> targets)
        {

            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath && targets.Contains(ally))
                {
                    ally.statModifier.AddModify(SourceTypes.Ability, new StatTypeData(StatTypes.AttackSpeed, attackSpeedBonus, StatValueTypes.Percent));
                    ally.handles.OnHitEnemies.Add(new HealOnDealDamage(ally, healPercentage, sources));
                }
            }
            
            yield return new WaitForSeconds(duration);

            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath && targets.Contains(ally))
                {
                    ally.statModifier.RemoveModify(SourceTypes.Ability, new StatTypeData(StatTypes.AttackSpeed, attackSpeedBonus, StatValueTypes.Percent));
                    ally.handles.OnHitEnemies.Remove(new HealOnDealDamage(ally, healPercentage, sources));
                }
            }

        }

    }
}