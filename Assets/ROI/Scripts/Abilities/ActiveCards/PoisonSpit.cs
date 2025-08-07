using System.Collections.Generic;
using UnityEngine;

namespace ROI
{


    public class PoisonSpit : BaseActiveAbilityCard, ISkillCardTrigger
    {

        [SerializeField] private StatusSetting poisonStatus;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            skillsPlayer.PlayFeedbacks();
            foreach (var target in targets)
            {
                if (!target.currentEffect.HasEffect(ChampionEffects.Poisoned))
                {
                    GeneralEffectSystem.Instance.ApplyEffect(target, new StatusData(poisonStatus.name, _championData, Vector3.zero));
                }
                GeneralEffectSystem.Instance.ApplyEffect(target, new StatusData(poisonStatus.name, _championData, Vector3.zero));

            }
        }
    }
}