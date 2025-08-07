using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using ROI;
using UnityEngine;

public class ProtectiveVeil : BaseActiveAbilityCard
{
    [SerializeField] private StatusSetting _statusSetting;
    public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
    {
        base.StartSkill(inputPosition, targets, isServer);
        MMF_Pause pause = (MMF_Pause)skillsPlayer.FeedbacksList[skillsPlayer.FeedbacksList.Count -2];
        pause.PauseDuration = cardSkillData.chanelTime;
        skillsPlayer.PlayFeedbacks();
        StartCoroutine(StartChanel(isServer));
    }
    
    private IEnumerator StartChanel(bool isServer)
    {
        yield return new WaitForSeconds(cardSkillData.chanelTime);
        List<ChampionData> championEffectBySkill = new List<ChampionData>();
        foreach (var ally in _championData.allies)
        {
            if (!ally.IsDeath )
            {
                championEffectBySkill.Add(ally);
            }
        }

        if (isServer)
        {
            _championData.ApplyEffectToChampionsBySkill(championEffectBySkill, _statusSetting.name);
        }
        
    }

    protected override void HandleOnApplyCC()
    {
        StopAllCoroutines();
        CancelSkill();
    }
}
