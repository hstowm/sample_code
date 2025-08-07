using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class ReflectiveBarrier : BaseActiveAbilityCard
    {
        private GameObject shield;
        [SerializeField] private float damageReflect = 0.5f;
        [SerializeField] private StatusSetting shieldSetting, reflectDamageSetting;

        private StatusData _reflectSetting;
        // [SerializeField] private ChampionData attackerData;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();

            if (isServer)
            {
                _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>() { _championData },
                    shieldSetting.name);
                _reflectSetting = new StatusData(reflectDamageSetting, _championData, new Vector3());
                GeneralEffectSystem.Instance.ApplyEffect(_championData,_reflectSetting);
                if (GeneralEffectSystem.ListEffectData.TryGetValue(_championData.netId, out _) == false)
                {
                    return;
                }

                foreach (var shield in GeneralEffectSystem.ListEffectData[_championData.netId])
                {
                    if (shield.effect_system is ReflectShield)
                    {
                        ReflectShield protectiveShield = (ReflectShield)shield.effect_system;
                        protectiveShield.onShieldDestroy = () => {GeneralEffectSystem.Instance.RemoveEffect(_championData, _reflectSetting); };
                    }
                }
            }
        }

    }
}
