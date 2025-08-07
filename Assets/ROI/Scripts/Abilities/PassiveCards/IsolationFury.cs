using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    /// <summary>
    /// Deal 25% more damage when there are no nearby allies around me (within range 2)
    /// </summary>
    
    [CreateAssetMenu(fileName = "IsolationFury", menuName = "ROI/Data/AbilityPassiveCards/IsolationFury", order = 1)]
    public class IsolationFury : BasePassiveAbilityCard
    {
        private ChampionData _championData;
        private MapSystem _mapSystem;
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            if (_mapSystem == null) 
                _mapSystem = FindObjectOfType<MapSystem>(true);
            
            champion.handles.OnHitEnemies.Add(new IsolationFuryInject(champion,_mapSystem));
        }



        class IsolationFuryInject : IOnHitEnemy
        {
            private ChampionData _championData;
            private MapSystem _mapSystem;

            public IsolationFuryInject(ChampionData championData, MapSystem mapSystem)
            {
                _championData = championData;
                _mapSystem = mapSystem;
            }
            
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                if (CheckConditional())
                {
                    damageDealtData.AddBonusDamage(0.25f,StatValueType.Percent);
                }
            }
            
            
            private bool CheckConditional()
            {
                if (_championData.allies.Count == 1) return true;
         
                float range = _mapSystem.ConvertToUnit(2);

                for (int i = 0; i < _championData.allies.Count; i++)
                {
                    var ally = _championData.allies[i];
                    if (!ally.Equals(_championData))
                    {
                        if (Vector3.Distance(_championData.transform.position, ally.transform.position) <= range)
                            return false;
                    }
                
                }

                return true;

            }

        }
        
        

      
        
    }
    
}

