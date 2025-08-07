using UnityEngine;

namespace ROI
{
    //Gain 5% ability power for each beneficial status I have
    [CreateAssetMenu(fileName = "AmplifiedPotency", menuName = "ROI/Data/AbilityPassiveCards/AmplifiedPotency",
        order = 1)]
    public class AmplifiedPotency : BasePassiveAbilityCard
    {
        [SerializeField] private float abilityPowerBonusForEachBuff = 0.05f;

        public override void OnInit(ChampionData champion)
        {
            if(PlayerNetwork.Instance.isServer)
                champion.handles.OnStartAlive.Add(new BonusAbilityPowerOnBuff(champion, abilityPowerBonusForEachBuff));
        }

        private class BonusAbilityPowerOnBuff : IOnStartAlive
        {
            private ChampionData _championData;
            private float _bonusForEachBuff;
            private float abilityPowerBonus = 0;

            public BonusAbilityPowerOnBuff(ChampionData data, float bonusForEachBuff)
            {
                _championData = data;
                _bonusForEachBuff = bonusForEachBuff;
            }

            public void OnStartAlive()
            {
                GeneralEffectSystem.Instance.applyEffectActions[_championData.netId] -= AddAbilityPower;
                GeneralEffectSystem.Instance.applyEffectActions[_championData.netId] += AddAbilityPower;
                GeneralEffectSystem.Instance.removeEffectActions[_championData.netId] -= AddAbilityPower;
                GeneralEffectSystem.Instance.removeEffectActions[_championData.netId] += AddAbilityPower;
            }

            private void AddAbilityPower(StatusData obj)
            {
                // Reset ability power on champion;
                _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.AbilityPower, -abilityPowerBonus));
                abilityPowerBonus = 0;
                if (GeneralEffectSystem.ListEffectData.TryGetValue(_championData.netId, out _) == false)
                {
                    return;
                }
                foreach (var statusData in GeneralEffectSystem.ListEffectData[_championData.netId])
                {
                    if (statusData.type == StatusData.EffectType.Buff)
                    {
                        abilityPowerBonus += _bonusForEachBuff;
                    }
                }
                _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.AbilityPower, abilityPowerBonus));
            }




        }
    }
}