using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class ManaSurge : BaseActiveAbilityCard
    {
        public StatusSetting statusDealDamage;
        bool isServer;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            skillsPlayer.PlayFeedbacks();
           this.isServer = isServer;
            SoundManager.PlaySfxPrioritize(skillSound);
   
        }

        public void OnInitImpact(ROI_Insaniatate instantiateObject)
        {
            if (championsEffectBySkill[0] != null)
            {
                instantiateObject.TargetPosition = championsEffectBySkill[0].transform.position;
                if (isServer)
                {

                    DealDame(championsEffectBySkill[0]);
                }
            }
        }

        public void DealDame(ChampionData enemyData)
        {
            GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData(statusDealDamage.name, _championData, Vector3.zero));
        }
    }
}
