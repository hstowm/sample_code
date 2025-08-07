using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class Gust : BaseActiveAbilityCard
    {
        [SerializeField] private GameObject buffEffect;
        [SerializeField] private GameObject deBuffEffect;
        [SerializeField] public StatusSetting pushback;
        [SerializeField] private float distance = 2;
        [SerializeField] private int bonusUltimateEnergy = 2;
        [SerializeField] private AudioClip _audioClip;
        private  Animator _championAnimator;
        private Vector3 target;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            target = inputPosition;
            _championAnimator = _championData.GetComponent<Animator>();
            base.StartSkill(inputPosition, targets, isServer);
            
            //TODO Add 2 ultimate energy to champion data
            skillsPlayer.PlayFeedbacks();
            if(!isServer) return;
            _championData.AddBonusUltimateEnergy(bonusUltimateEnergy);
            SoundManager.PlaySfxPrioritize(_audioClip);

        }
        

        public void StartCastAniamtion()
        {

            _championAnimator.SetTrigger("ActiveCardSkill");
            _championData.transform.LookAt(target);
        }

        public void EndCast()
        {

            _championAnimator.SetTrigger("ActiveCardSkillEnd");

            _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "Gust");
            _championData.ApplyEffectToChampionsBySkill(championsEffectBySkill, "GustKnockBack", _championData.transform.position);
        }

        public void InstantiatePushBackEffect(GameObject FxPushbackFrefab)
        {
            FxPushbackFrefab.transform.position = _championData.transform.position;
            FxPushbackFrefab.transform.LookAt(target);
        }

        public void InstantiatemanipulatorEffect(GameObject FxChargeFrefab)
        {
            FxChargeFrefab.transform.position = _championData.transform.position;
        }
        public void InstantiatemanipulatorReleaseEffect(GameObject FxChargeFrefab)
        {
            FxChargeFrefab.transform.position = _championData.transform.position;
        }

        public void InstantiateChargeEffect(GameObject FxChargeFrefab)
        {
            FxChargeFrefab.transform.position = _championData.transform.position;
        }
    }
}