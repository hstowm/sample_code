using System.Collections;
using System.Collections.Generic;
using ROI;
using UnityEngine;

namespace ROI
{
    
}
public class IceField : BaseActiveAbilityCard
{
    public float iceFieldExistTime = 5;
    public StatusSetting dealSkillDamage;
    public StatusSetting dealSkillChilled;
    public List<ChampionData> enemiesHitSkill = new List<ChampionData>();
    private bool isServer;
    private GameObject iceField;
    [SerializeField] private AudioClip skillSound;
    public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
    {
        base.StartSkill(inputPosition, targets, isServer);
        this.isServer = isServer;
        enemiesHitSkill = targets;
        skillsPlayer.PlayFeedbacks();
        SoundManager.PlaySfxPrioritize(skillSound);
    }

    public void SetIceFieldPosition(GameObject iceField)
    {
        this.iceField = iceField;
        iceField.transform.position = targetPosition;
        if (isServer)
        {
            DealsSkillEffect(enemiesHitSkill);
            StartCoroutine(IceFieldActive());
        }
    }
    private IEnumerator IceFieldActive()
    {
        float timeCounting = 0f;
        while (timeCounting < iceFieldExistTime)
        {
            List<ChampionData> enemyHitShill = new List<ChampionData>();
            foreach (var enemy in _championData.enemies)
            {
                if ( !enemy.IsDeath && Vector3.Distance(enemy.transform.position, targetPosition) <= cardSkillData.wide &&  enemy.currentEffect.HasEffect(ChampionEffects.KnockBack) )
                {
                    // Deals 50 damage and chilled to enemy
                    if (!enemiesHitSkill.Contains(enemy))
                    {
                        Debug.Log($"apply skill Ice Field to {enemy.name}");
                        enemyHitShill.Add(enemy);
                        enemiesHitSkill.Add(enemy);
                    }
                }
                else
                {
                    if (enemiesHitSkill.Contains(enemy))
                    {
                        enemiesHitSkill.Remove(enemy);
                    }
                }
            }
            DealsSkillEffect(enemyHitShill);
            yield return new WaitForEndOfFrame();
            timeCounting += Time.deltaTime;
        }
        Destroy(iceField);
    }

    private void DealsSkillEffect(List<ChampionData> enemiesHitSkill)
    {
        _championData.ApplyEffectToChampionsBySkill(enemiesHitSkill, dealSkillDamage.name);
        _championData.ApplyEffectToChampionsBySkill(enemiesHitSkill, dealSkillChilled.name);
    }
}
