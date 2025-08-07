using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// Playing an ability increases my next auto-attacks damage by 25%
    /// </summary>
    
    [CreateAssetMenu(fileName = "EternalPyre", menuName = "ROI/Data/AbilityPassiveCards/EternalPyre")]
    public class EternalPyre : BasePassiveAbilityCard
    {
        
        private ChampionData _championData;
       
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            var exter = new EternalPyreInject(champion);
            champion.handles.OnUseCards.Add(exter);
            champion.handles.OnHitEnemies.Add(exter);
        }


        class EternalPyreInject : IOnHitEnemy, IOnUseCard
        {
            private ChampionData _championData;
            private bool _increaseDamage = false;

            public EternalPyreInject(ChampionData championData)
            {
                this._championData = championData;
            }
            
            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                _increaseDamage = true;
            }

            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                if (damageDealtData.damageSource.IsBasicAttack())
                {
                    if (_increaseDamage)
                    {
                        damageDealtData.AddBonusDamage(0.25f,StatValueType.Percent);
                        _increaseDamage = false;
                    }
                }
           
            }
        }
       
    }

}

