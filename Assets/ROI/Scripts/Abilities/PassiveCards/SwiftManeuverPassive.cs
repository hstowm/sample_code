using System.Collections.Generic;
using UnityEngine;
using ROI.DataEntity;
using System;

namespace ROI
{
    [CreateAssetMenu(fileName = "SwiftManeuverPassive",
        menuName = "ROI/Data/AbilityPassiveCards/SwiftManeuver")]
    public class SwiftManeuverPassive : BasePassiveAbilityCard, IAbilityCard
    {
        //[Server]
        public override void OnInit(ChampionData champion)
        {
            SwiftManeuverActiveAbilitiesInject swiftManeuverActiveAbilitiesInject = new SwiftManeuverActiveAbilitiesInject(champion);
            champion.handles.OnUseCards.Add(swiftManeuverActiveAbilitiesInject);
        }

        class SwiftManeuverActiveAbilitiesInject : IOnUseCard
        {
            ChampionData championData;
            public SwiftManeuverActiveAbilitiesInject(ChampionData championData)
            {
                this.championData = championData;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets,bool isServerSide)
            {
                Debug.Log("OnUseActiveCard");
                SwiftManeuverDodgeInject swiftManeuverDodgeInject = new SwiftManeuverDodgeInject(championData);
                if (!championData.handles.OnAttacked.Contains(swiftManeuverDodgeInject))
                    championData.handles.OnAttacked.Add(swiftManeuverDodgeInject);
            }
        }

            class SwiftManeuverDodgeInject : IOnAttacked, IEquatable<SwiftManeuverDodgeInject>
        {

            ChampionData championData;
            public SwiftManeuverDodgeInject(ChampionData championData)
            {
                Debug.Log("championData: " + championData.name);
                this.championData = championData;
            }

            public bool Equals(SwiftManeuverDodgeInject other)
            {
                return other != null && other.championData.Equals(championData);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SwiftManeuverDodgeInject);
            }

            public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
            {
                if (damageDealtData.damageSource.IsBasicAttack())
                {
                    championData.handles.OnAttacked.Remove(this);
                    damageDealtData.dodgeChancePercent = 1;
                }
            }
        }
    }
}
