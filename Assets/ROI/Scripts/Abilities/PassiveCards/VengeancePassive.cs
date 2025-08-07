using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "VengeancePassive", menuName = "ROI/Data/AbilityPassiveCards/Vengeance")]
    public class VengeancePassive : BasePassiveAbilityCard
    {

        private ChampionData _championData;
        
        public override void OnInit(ChampionData champion)
        {
            //VengeanceInject vengeanceInject = new VengeanceInject(champion);
            _championData = champion;
            champion.handles.OnStartAlive.Add(new VengeanceInjectAlive(champion));
        }
        
        class VengeanceInjectAlive : IOnStartAlive
        {
            
            ChampionData championData;
            public VengeanceInjectAlive(ChampionData championData)
            {
                this.championData = championData;
            }
            
            public void OnStartAlive()
            {
                for (int i = 0; i < championData.allies.Count; i++)
                {
                    if (!championData.allies[i].Equals(championData))
                    {
                        Debug.Log("init on dead: " + championData.allies[i].name);
                        championData.allies[i].handles.OnDeads.Add(new VengeanceInject(championData));
                    }
                }
            }
        }

        class VengeanceInject : IOnDead
        {
            ChampionData championData;
            public VengeanceInject(ChampionData championData)
            {
                this.championData = championData;
            }

          
            public void OnDead()
            {
                Debug.Log("Engulfing");
                if (championData.IsDeath) return;
                GeneralEffectSystem.Instance.ApplyEffect(championData, new StatusData("Engulfing", championData, new Vector3()));
            }
        }

     
    }
}
