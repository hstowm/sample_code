using System.Collections.Generic;

namespace ROI
{
    public class ReflectDamageEffect : DisplayableEffect, IEffectSystem, IOnDamaged
    {
        private ChampionData _championData;
        private float reflectPercent = 0f;
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            StatusParam current_level = arg.GetCurrentParam();

            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.ReflectFlat:
                        break;
                    case StatusParamKeyWord.ReflectPercentDamageReceive:
                        reflectPercent = entry.Value;
                        break;
                }
            }
            _championData = champion;
            champion.handles.OnDamageds.Add(this);
        }

        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            champion.handles.OnDamageds.Remove(this);
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        }

        public void OnDamaged(ChampionData attacker, DamageDealtData damageDealtData)
        {
            if(damageDealtData.damageType == DamageTypes.True || damageDealtData.damageSource.IsEffect()) return;
            var totalDamage = damageDealtData.damageOnHit; 
            GeneralEffectSystem.Instance.DealDynamicDamageOnChampion(_championData,attacker, totalDamage*reflectPercent, DamageSources.Effect, DamageTypes.True);
        }
    }

}
