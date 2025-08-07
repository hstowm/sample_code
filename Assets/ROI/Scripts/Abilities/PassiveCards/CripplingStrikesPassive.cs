using System;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    //When I hit an enemy, reduce its next auto-attack damage by 20%
    [CreateAssetMenu(fileName = "CripplingStrikesPassive", menuName = "ROI/Data/AbilityPassiveCards/CripplingStrikes")]
    public class CripplingStrikesPassive : BasePassiveAbilityCard, IAbilityCard
    {
        public float percentReduce;
        //[Server]
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new CripplingStrikesAttacker(champion, percentReduce));
        }
    }

    class CripplingStrikesAttacker : IOnHitEnemy
    {
        ChampionData champion;
        float percentDamageReduce;
        public CripplingStrikesAttacker(ChampionData champion, float percentReduce)
        {
           this.champion = champion;
           this.percentDamageReduce = percentReduce;
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
         //    float dameReduce = enemy.attackData.damage * percentReduce;
            CripplingStrikesEnemy cripplingStrikesEnemy = new CripplingStrikesEnemy(enemy, percentDamageReduce);
            
            if (!enemy.handles.OnHitEnemies.Contains(cripplingStrikesEnemy))
                enemy.handles.OnHitEnemies.Add(cripplingStrikesEnemy);
        }
    }

    class CripplingStrikesEnemy : IOnHitEnemy, IEquatable<CripplingStrikesEnemy>
    {
        ChampionData champion;
        float percentDamageReduce;
        public CripplingStrikesEnemy(ChampionData champion, float percentDamageReduce)
        {
            this.champion = champion;
            this.percentDamageReduce = percentDamageReduce;
        }

        public bool Equals(CripplingStrikesEnemy other)
        {
            return other != null && other.champion.Equals(champion);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CripplingStrikesEnemy);
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
                 // dang bi danh lien tuc se cong don lam am damage
                 champion.handles.OnHitEnemies.Remove(this);

                 damageDealtData.AddBonusDamage(-percentDamageReduce, StatValueType.Percent);
                 
            //     var attackData = champion.attackData;
            //     attackData.damage -= dameReduce;
            //     
            // damageDealtData.healthReduction = DamageDealtCalculator.CalcHeathReduction(enemy, attackData,
            //     out damageDealtData.attackDamage,
            //     out damageDealtData.critDamagePercent,
            //     out damageDealtData.isCit);
        }
    }
}
