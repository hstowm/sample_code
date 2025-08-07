using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "RelentlessAssaultPassive",menuName = "ROI/Data/AbilityPassiveCards/RelentlessAssault")]
    public class RelentlessAssaultPassive : BasePassiveAbilityCard, IAbilityCard
    {
        public float critChanceAddPerAtk;

       //  [Server]
        public override void OnInit(ChampionData champion)
        {
            ChangeCritChance data = new ChangeCritChance(champion, critChanceAddPerAtk);
            champion.handles.OnHitEnemies.Add(data);
        }

        class ChangeCritChance : IOnHitEnemy,IOnFinalDamage
        {
            ChampionData championData;
            float critChance;
            float totalCritChance;

            public ChangeCritChance(ChampionData champion, float critChance)
            {
                championData = champion;
                this.critChance = critChance;
                totalCritChance = 0;
            }

            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                if(damageDealtData.damageSource.IsBasicAttack() == false)
                    return;

                Logs.Info($"{championData.name} Current Total Crit Chance: {totalCritChance}");
                damageDealtData.critDamageChance += totalCritChance;
                damageDealtData.hooks.OnFinalDamages.Add(this);
            }
            
            public void OnDamageCalculated(DamageDealtData damageDealtData)
            {
                if (damageDealtData.isCrit)
                {
                    Logs.Info($"{championData.name} have Crit. Reset Total Crit Chance to zero.");
                    totalCritChance = 0;
                    return;
                }
                
                totalCritChance += critChance;
                Logs.Info($"{championData.name} Dont Crit. Increase Total Crit Chance to {totalCritChance}");
            }
        }
    }
}
