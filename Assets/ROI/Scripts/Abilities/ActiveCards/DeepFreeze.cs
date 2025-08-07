using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class DeepFreeze : BaseActiveAbilityCard, IOnHitEnemy
    {

        private GameObject shield;
        private float _shieldRemain = 0;
        private float _shieldPercentHealth = 0.2f;
        [SerializeField] private StatusSetting shieldSetting;
        [SerializeField] private StatusSetting chillSetting;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
            SoundManager.PlaySfxPrioritize(skillSound);
            if (isServer)
            {
                _championData.handles.OnHitEnemies.Add(this);
                _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>(){_championData}, shieldSetting.name);
                if (GeneralEffectSystem.ListEffectData.TryGetValue(_championData.netId, out _) == false)
                {
                    return;
                }

                foreach (var shield in GeneralEffectSystem.ListEffectData[_championData.netId])
                    {
                        if (shield.effect_system is DeepFreezeShield)
                        {
                            DeepFreezeShield protectiveShield = (DeepFreezeShield)shield.effect_system;
                            protectiveShield.onShieldDestroy = () =>
                            {
                                _championData.handles.OnHitEnemies.Remove(this);
                            };
                        }
                    }
                
            }

        }
        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if ((damageDealtData.damageSource & DamageSources.ActiveAbility )!= 0)
            {   
                _championData.ApplyEffectToChampionsBySkill(new List<ChampionData>{enemy},chillSetting.name);
            }
        }
    }
}