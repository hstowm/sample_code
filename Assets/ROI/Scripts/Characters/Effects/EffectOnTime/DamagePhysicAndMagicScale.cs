using System.Collections.Generic;

namespace ROI
{


    public class DamagePhysicAndMagicScale : DisplayableEffect, IEffectSystem, IOnAttacked
    {
        private ChampionData _championData;
        private float damageReduce;
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            StatusParam current_level = arg.GetCurrentParam();

            foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
            {
                switch (entry.Key)
                {
                    case StatusParamKeyWord.ReflectFlat:
                        break;
                    case StatusParamKeyWord.DamageNegatePercent:
                        damageReduce = entry.Value;
                        break;
                }
            }
            champion.handles.OnAttacked.Add(this);
        }

        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            champion.handles.OnAttacked.Remove(this);
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        }

        public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
        {
            if (damageDealtData.damageType is DamageTypes.Magic or DamageTypes.Physic)
            {
                // negate this damage
                damageDealtData.blockPercent = damageReduce;
            }
        }
    }
}