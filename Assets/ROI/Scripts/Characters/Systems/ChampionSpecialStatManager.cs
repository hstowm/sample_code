using UnityEngine;

namespace ROI
{
    public class ChampionSpecialStatManager : MonoBehaviour
    {
        public StatusSetting poisonedSetting, blessSetting, frenzySetting, engulfSetting, stunSetting,vulnerableSetting, chillSetting, reflectDamageSetting, shieldOnStartSetting, attackSpeedReduceSetting;
        public void InitSpecialStat(ChampionData championData)
        {
            if (championData.specialStatData.reflectDamage > 0)
            {
                championData.handles.OnStartAlive.Add(new ApplyReflectDamage(championData, reflectDamageSetting, championData.specialStatData.reflectDamage));
            }
            if (championData.specialStatData.reduceAttackerAtkSpeed > 0)
            {
                championData.handles.OnAttacked.Add(new ApplyAttackSpeedReduceOnAttacked(championData, attackSpeedReduceSetting, championData.specialStatData.reduceAttackerAtkSpeed));
            }
            if (championData.specialStatData.chanceToApplyBlessOnSpell > 0)
            {
                championData.handles.OnUseCards.Add(new ApplyEffectSelfOnUsingSkill(championData, blessSetting, championData.specialStatData.chanceToApplyBlessOnSpell));
            }
            if (championData.specialStatData.shieldOnStartCombat > 0)
            {
                championData.handles.OnStartAlive.Add(new ApplyShieldOnStartCombat(championData, shieldOnStartSetting, championData.specialStatData.shieldOnStartCombat));
            }
            if (championData.specialStatData.chanceToChillOnHit> 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, chillSetting, championData.specialStatData.chanceToChillOnHit, false, false));
            }
            if (championData.specialStatData.chanceToPoisonedOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, poisonedSetting, championData.specialStatData.chanceToPoisonedOnHit, false, false));
            }
            if (championData.specialStatData.chanceToFrenzyOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, frenzySetting, championData.specialStatData.chanceToFrenzyOnHit, false, true));
            }
            if (championData.specialStatData.chanceToEngulfOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, engulfSetting, championData.specialStatData.chanceToEngulfOnHit, false, true));
            }
            if (championData.specialStatData.chanceToBlessOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, blessSetting, championData.specialStatData.chanceToBlessOnHit, false, true));
            }
            if (championData.specialStatData.chanceToVulnerableOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, vulnerableSetting, championData.specialStatData.chanceToVulnerableOnHit, false, false));
            }
            if (championData.specialStatData.chanceToStunOnHit > 0)
            {
                championData.handles.OnHitEnemies.Add(new ApplyEffectOnNormalAttackEnemy(championData, stunSetting, championData.specialStatData.chanceToStunOnHit, false, false));
            }
        }
    }
}