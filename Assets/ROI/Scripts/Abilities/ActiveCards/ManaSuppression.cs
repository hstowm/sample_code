using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{

/// <summary>
/// Channel for 5 seconds. Upon completion, the next card you play costs 5 mana less
/// </summary>
    public class ManaSuppression : BaseActiveAbilityCard, IOnUseCard
    {
        Dictionary<SkillCard, float> listManaReduceEachCard = new Dictionary<SkillCard, float>();
        public int manaReduce = 5;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            MMF_Pause pause = (MMF_Pause)skillsPlayer.FeedbacksList[skillsPlayer.FeedbacksList.Count -2];
            pause.PauseDuration = cardSkillData.chanelTime;
            skillsPlayer.PlayFeedbacks();
            if(isServer)
                StartCoroutine(StartChanel(isServer));
        }
    
        private IEnumerator StartChanel(bool isServer)
        {
            yield return new WaitForSeconds(cardSkillData.chanelTime);
            foreach (var champion in _championData.allies)
            {
                if (!champion.IsDeath)
                {
                    champion.AddManaCostBonus(-manaReduce);
                    champion.handles.OnUseCards.Add(this);
                }
            }
        }

        public void ReSetCardSkillMana(Dictionary<SkillCard, float> cardAddMana)
        {
            foreach (var ally in _championData.allies)
            {
                if (!ally.IsDeath)
                {
                    Debug.Log($"Reset mana for champion {ally.name}");
                    ally.AddManaCostBonus(manaReduce);

                    ally.handles.OnUseCards.Remove(this);
                }
            }
        }
        public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
        {
            if(cardSkillType.cardSkillType != CardSkillType.ChampionUltimate)
                ReSetCardSkillMana(listManaReduceEachCard);
        }

        protected override void HandleOnApplyCC()
        {
            StopAllCoroutines();
            CancelSkill();
        }
    }
}