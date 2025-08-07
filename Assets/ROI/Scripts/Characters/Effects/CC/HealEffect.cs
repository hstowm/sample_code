using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ROI
{
    public class HealEffect : DisplayableEffect, IEffectSystem
    {
        [SerializeField] private ChampionDamageText _championDamageText;
        public override void OnStartClient()
        {
            _championDamageText = ChampionDamageText.instance;
        }

        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            if (_championDamageText == null)
            {
                _championDamageText = ChampionDamageText.instance;
            }

            StatusParam current_level = arg.GetCurrentParam();
            float hpHeal = 0;
            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.HealFlat:
                        hpHeal += entry.Value;
                        break;
                    case StatusParamKeyWord.HealPercent:
                        hpHeal += entry.Value * champion.healthData.maxHealth;
                        break;
                }
            }
            hpHeal *= (champion.specialStatData.abilityPower / 100 + 1);
            HealChampion(champion, hpHeal);

        }
        public void HealChampion(ChampionData champion, float hpHeal)
        {
            
            champion.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, hpHeal));
            ShowHealFX(champion, (int)hpHeal);
            ApplyVFX(champion);

        }

        [ClientRpc]
        void ShowHealFX(ChampionData champion, int hpHeal)
        {
            _championDamageText.ShowHealDamage(champion, hpHeal);
        }
        
        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            ClearVFX();
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
        }
    }
}