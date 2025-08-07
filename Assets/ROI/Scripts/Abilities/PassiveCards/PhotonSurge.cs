using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    /// <summary>
    /// When I'm targeted by an ally, reduce that cards cost by 1 for the rest of the game (minimum 1)
    /// </summary>
    
    [CreateAssetMenu(fileName = "PhotonSurge", menuName = "ROI/Data/AbilityPassiveCards/PhotonSurge", order = 1)]
    public class PhotonSurge : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnStartAlive.Add(new PhotonSurgeStartAlive(champion));
        }

        class PhotonSurgeInject : IOnUseCard
        {
            
            private ChampionData _championData;
            private ChampionData _championBuff;

            public PhotonSurgeInject(ChampionData championData, ChampionData championBuff)
            {
                _championData = championData;
                _championBuff = championBuff;
            }
           
            public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
            {
                if (!_championData.IsDeath && !_championBuff.IsDeath)
                {
                    if (listTargets.Contains(_championData))
                    {
                        Debug.Log("add passive");
                        _championBuff.AddManaCostBonus(cardSkillType.KeyName,-1);
                    }
                }
            }
        }

        class PhotonSurgeStartAlive : IOnStartAlive
        {
            
            private ChampionData _championData;

            public PhotonSurgeStartAlive(ChampionData championData)
            {
                _championData = championData;
            }
            
            public void OnStartAlive()
            {
                for (int i = 0; i < _championData.allies.Count; i++)
                {
                    var champion = _championData.allies[i];
                    champion.handles.OnUseCards.Add(new PhotonSurgeInject(_championData, champion));
                }
            }
        }

        

       
    }

}

