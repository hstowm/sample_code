using UnityEngine;


namespace ROI
{
    /// <summary>
    /// Start the game with a 10% max health shield.
    /// </summary>
    
    [CreateAssetMenu(fileName = "ShadowyPresence", menuName = "ROI/Data/AbilityPassiveCards/ShadowyPresence")]

    public class ShadowyPresence : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnStartAlive.Add(new ShadowyPresenceInject(champion));
        }

        class ShadowyPresenceInject : IOnStartAlive
        {
            
            private ChampionData _championData;

            public ShadowyPresenceInject(ChampionData championData)
            {
                _championData = championData;
            }
            
            public void OnStartAlive()
            {
                //StartPassiveShields_status
                GeneralEffectSystem.Instance.ApplyEffect(
                    (_championData), new StatusData("StartPassiveShields_status", _championData, Vector3.zero));
            }
        }

      
    }
}


