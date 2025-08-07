using UnityEngine;
using ROI.DataEntity;

namespace ROI
{
    [CreateAssetMenu(fileName = "IronWillPassive",
        menuName = "ROI/Data/AbilityPassiveCards/IronWill")]
    public class IronWillPassive : BasePassiveAbilityCard, IAbilityCard
    {
        public float lessDamePercent;

        //[Server]
        public override void OnInit(ChampionData champion)
        {
            IronWillInject ironWillInject = new IronWillInject(champion, lessDamePercent);
            champion.handles.OnAttacked.Add(ironWillInject);
        }

        class IronWillInject : IOnAttacked
        {
            ChampionData championData;
            float lessDamePercent;
            public IronWillInject(ChampionData championData, float lessDamePercent)
            {
                this.championData = championData;
                this.lessDamePercent = lessDamePercent;
            }

            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                if ((damageDealtData.damageSource & DamageSources.ActiveAbility) == 0) return;
                damageDealtData.AddBonusDamage(-lessDamePercent, StatValueType.Percent);
            }
        }
    }
}
