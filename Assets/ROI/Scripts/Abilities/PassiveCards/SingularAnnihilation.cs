using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// Deal 30% more damage to isolated enemy units (no allies within range 2)
    /// </summary>
    
    [CreateAssetMenu(fileName = "SingularAnnihilation", menuName = "ROI/Data/AbilityPassiveCards/SingularAnnihilation", order = 1)]
    public class SingularAnnihilation : BasePassiveAbilityCard, IOnHitEnemy
    {
        private MapSystem _mapSystem;
        public override void OnInit(ChampionData champion)
        {
            if (_mapSystem == null) _mapSystem = FindObjectOfType<MapSystem>();
            champion.handles.OnHitEnemies.Add(this);
        }

        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            Debug.Log("check : " + CheckConditional(enemy));
            if (CheckConditional(enemy))
            {
                damageDealtData.AddBonusDamage(0.3f,StatValueType.Percent);
            }
        }
        
        private bool CheckConditional(ChampionData enemy)
        {
            if (enemy.allies.Count == 1) return true;
            float range = _mapSystem.ConvertToUnit(2);
            
            for (int i = 0; i < enemy.allies.Count; i++)
            {
                var ally = enemy.allies[i];
                if (!ally.Equals(enemy))
                {
                    if (Vector3.Distance(enemy.transform.position, ally.transform.position) <= range)
                        return false;
                }
                
            }

            return true;

        }
        
    }

}

