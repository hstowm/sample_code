using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ROI
{
    [Serializable]
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "ROI/Data/AbilityPassiveCards/PassiveAbility", order = 1)]
    public class PassiveAbility : Trait
    {
        [JsonProperty("Set")] public string setType;

        public override void Apply(ChampionData data)
        {
            // if (data.setData.Civ.ToString() == setType || data.setData.Zset.ToString() == setType || data.setData.classType.ToString() == setType)
            {
                base.Apply(data);
            }
        }

        public override void Remove(ChampionData data)
        {
            // if (data.setData.Civ.ToString() == setType || data.setData.Zset.ToString() == setType || data.setData.classType.ToString() == setType)
            {
                base.Remove(data);
            }
        }
    }
}