using System.Collections.Generic;
using UnityEngine;

namespace ROI
{


    public class Concotion : BaseActiveAbilityCard, ISkillCardTrigger
    {
        [SerializeField] private float damageAndHeal;
        [SerializeField] private StatusSetting status;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            if (targets.Count <= 0) return;
            Debug.Log(targets.Count);
            Debug.Log(targets[0].name);
            if (!isServer) return;
            if (_championData.enemies.Contains(targets[0]))
            {
                _championData.attacker.AttackEnemy(targets[0], damageAndHeal, DamageSources.ActiveCardSkill, DamageTypes.Magic);
                GeneralEffectSystem.Instance.ApplyEffect(targets[0], new StatusData(status.name, targets[0], Vector3.zero));
            }
            else if (_championData.allies.Contains(targets[0]))
            {
                targets[0].statModifier.ApplyModify(new StatTypeData(StatTypes.Health, damageAndHeal));
                ChampionDamageText.instance.ShowHealDamage(targets[0], (int)damageAndHeal);
                GeneralEffectSystem.Instance.RemoveEffectOnChampion(_championData.netId, StatusData.EffectType.DeBuff);
            }
        }

    }
}