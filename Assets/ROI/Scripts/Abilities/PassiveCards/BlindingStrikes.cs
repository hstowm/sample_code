using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// My attacks have a 5% chance to blind enemies for 5 seconds.
    /// </summary>
    
    [CreateAssetMenu(fileName = "BlindingStrikes", menuName = "ROI/Data/AbilityPassiveCards/BlindingStrikes", order = 1)]
    public class BlindingStrikes : BasePassiveAbilityCard
    {
        public float percentActive = 0.05f;
        public StatusSetting BlindStrike;
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnHitEnemies.Add(new BlindingStrikesInject(percentActive,champion,BlindStrike.name));
        }


        class BlindingStrikesInject : IOnHitEnemy
        {
            private ChampionData _championData;
            private float percentActive;
            private string nameStatus;
            
            public BlindingStrikesInject(float percentActive, ChampionData championData, string nameStatus)
            {
                this.percentActive = percentActive;
                this._championData = championData;
                this.nameStatus = nameStatus;
            }
            public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
            {
                var ran = Random.RandomRange(0, 100);
                Debug.Log(ran + " " + (percentActive * 100));
                var checkActive = ran < percentActive * 100;
                if (checkActive)
                {
                    if (!enemy.currentEffect.HasEffect(ChampionEffects.Blind))
                    {
                        // TODO : effect blind for enemy 
                        Debug.Log("blind");
                        GeneralEffectSystem.Instance.ApplyEffect(enemy, new StatusData(nameStatus,_championData,Vector3.zero));
                    }
              
                }
            }

        }
        
        

     
    }
    
}

