using UnityEngine;

namespace ROI
{


    /// <summary>
    /// When a friendly status timer on me runs out, I have a 25% chance to reapply it at level 1.
    /// </summary>
    [CreateAssetMenu(fileName = "ManaHarvest", menuName = "ROI/Data/AbilityPassiveCards/ManaHarvest", order = 1)]
    public class ManaHarvest : BasePassiveAbilityCard
    {
        public float rateReapply = 0.25f;

        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnStartAlive.Add(new ReApplyEffectOnChampion(champion, rateReapply));
        }
        public class ReApplyEffectOnChampion:IOnStartAlive
        {
            private ChampionData _championData;
            private float reApplyChance;

            public ReApplyEffectOnChampion(ChampionData championData, float reApplyChance)
            {
                _championData = championData;
                this.reApplyChance = reApplyChance;
            }
            public void OnStartAlive()
            {
                GeneralEffectSystem.Instance.removeEffectActions[_championData.netId] += ReApplyEffect;
            }

            private void ReApplyEffect(StatusData data)
            {
                if (data.type == StatusData.EffectType.Buff)
                {
                    bool isReapply = DamageDealtCalculator.HasChance(reApplyChance);
                    if (isReapply)
                    {
                        GeneralEffectSystem.Instance.ApplyEffect(_championData,new StatusData(data.setting, _championData, data.position, data.type));
                    }
                }
            }
        }
    }
}
