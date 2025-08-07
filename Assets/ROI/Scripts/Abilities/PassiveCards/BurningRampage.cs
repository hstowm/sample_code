using ROI.DataEntity;
using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// My attacks and abilities deal 10% more damage when I'm Engulfed
    /// </summary>
    /// 
    [CreateAssetMenu(fileName = "BurningRampage", menuName = "ROI/Data/AbilityPassiveCards/BurningRampage")]
    public class BurningRampage : BasePassiveAbilityCard
    {

        private ChampionData _championData;
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            champion.handles.OnHitEnemies.Add(new BurningRampageInject(champion));
        }
    }

    class BurningRampageInject : IOnHitEnemy
    {
        private ChampionData _championData;


        public BurningRampageInject(ChampionData championData)
        {
            this._championData = championData;
        }
        
        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if (_championData.currentEffect.HasEffect(ChampionEffects.Engulfing))
            {
                damageDealtData.AddBonusDamage(0.1f,StatValueType.Percent);
            }
        }
    }
}

