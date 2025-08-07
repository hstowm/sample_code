using System;
using ROI;
using UnityEngine;
using Guid = Pathfinding.Util.Guid;

public class DealAdditionalDamageToAlliesAround : IOnAttacked, IEquatable<DealAdditionalDamageToAlliesAround>
{
    public string uuid = Guid.NewGuid().ToString();
    private ChampionData _championData;
    private float aoERadius = 3;
    private float damageBonusPercent;
    public DealAdditionalDamageToAlliesAround(ChampionData championData, float damageBonusPercent = 0.2f)
    {
        _championData = championData;
        this.damageBonusPercent = damageBonusPercent;
    }

    public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
    {
        if (damageDealtData.damageSource == DamageSources.Effect) return;
        foreach (var champion in _championData.allies)
        {
            if (!champion.IsDeath && champion.netId != _championData.netId && Vector3.Distance(champion.transform.position, _championData.transform.position) <= aoERadius)
            {
                _championData.attacker.AttackEnemy(champion, damageDealtData.attackDamage*damageBonusPercent, DamageSources.Effect, DamageTypes.Magic
                );
            }
        }
    }

    public bool Equals(DealAdditionalDamageToAlliesAround other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return uuid == other.uuid;
    }
}
