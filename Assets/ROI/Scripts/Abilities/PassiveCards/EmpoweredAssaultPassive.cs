using ROI.DataEntity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    
    /// <summary>
    /// Every target hit from my abilities increases my attack damage by 5% for 10s (max 5 stacks).
    /// </summary>
    
    [CreateAssetMenu(fileName = "EmpoweredAssaultPassive", menuName = "ROI/Data/AbilityPassiveCards/EmpoweredAssault")]
    public class EmpoweredAssaultPassive : BasePassiveAbilityCard
    {
        public float percentReduce = 0.5f;
        
        public override void OnInit(ChampionData champion)
        {
            EmpoweredAssaultInject empoweredAssaultInject = new EmpoweredAssaultInject(champion, percentReduce);
            champion.handles.OnHitEnemies.Add(empoweredAssaultInject);

            EmpoweredAssaultUsecardInject empoweredAssaultUsecardInject = new EmpoweredAssaultUsecardInject(champion, empoweredAssaultInject);
            champion.handles.OnUseCards.Add(empoweredAssaultUsecardInject);
        }

        class EmpoweredAssaultUsecardInject : IOnUseCard
        {
            ChampionData championData;
            int countStackAddDame;
            DateTime timeActivePassive = DateTime.UtcNow;
            EmpoweredAssaultInject empoweredAssaultInject;
            public EmpoweredAssaultUsecardInject(ChampionData championData, EmpoweredAssaultInject empoweredAssaultInject)
            {
                this.championData = championData;
                this.empoweredAssaultInject = empoweredAssaultInject;
            }

            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                TimeSpan elapsedSpan = new TimeSpan(DateTime.UtcNow.Ticks - timeActivePassive.Ticks);
                if (elapsedSpan.Seconds > 10 && countStackAddDame > 0)
                {
                    countStackAddDame = 0;
                }

                timeActivePassive = DateTime.UtcNow;
                if (countStackAddDame < 5)
                {
                    countStackAddDame += listTargets.Count;// so nay thay bang tong so enemy bi trung
                    if(countStackAddDame > 5)
                    {
                        countStackAddDame = 5;
                    }
                }
                empoweredAssaultInject.UseSkill(countStackAddDame);
            }
        }

        class EmpoweredAssaultInject : IOnHitEnemy
        {
            ChampionData championData;
            float percentReduce;
            DateTime timeActivePassive = DateTime.UtcNow;
            public int countStackAddDame;
            public EmpoweredAssaultInject(ChampionData championData, float percentReduce)
            {
                this.championData = championData;
                this.percentReduce = percentReduce;
            }

            public void UseSkill(int countStackAddDame)
            {
                this.countStackAddDame = countStackAddDame;
                timeActivePassive = DateTime.UtcNow;
            }

            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
    
                TimeSpan elapsedSpan = new TimeSpan(DateTime.UtcNow.Ticks - timeActivePassive.Ticks);
                if (elapsedSpan.Seconds > 10 && countStackAddDame > 0)
                {
                    countStackAddDame = 0;
                }


                if (countStackAddDame > 0)
                {
                    damageDealtData.AddBonusDamage(countStackAddDame * percentReduce, StatValueType.Percent);
                }
            }
        }
    }
}
