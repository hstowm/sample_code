using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ROI
{


    public class Amplify : BaseActiveAbilityCard, ISkillCardTrigger
    {

        [SerializeField] private int levelBonus;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            //skillsPlayer.PlayFeedbacks();
            if (isServer)
            {
                GeneralEffectSystem.Instance.ApplyEffect(_championData, new StatusData("Engulfing", _championData, new Vector3()));
                ApplyEffectOnChampions(_championData.allies, StatusData.EffectType.Buff);
                ApplyEffectOnChampions(_championData.enemies, StatusData.EffectType.DeBuff);
            }
         
        }

        public void ApplyEffectOnChampions(SyncList<ChampionData> listChampion, StatusData.EffectType type)
        {
            foreach (var champion in listChampion)
            {
                if (!champion.IsDeath)
                {
                    if (GeneralEffectSystem.ListEffectData.TryGetValue(champion.netId, out _) == false)
                    {
                        continue;
                    }
                    foreach (var effectsData in GeneralEffectSystem.ListEffectData[champion.netId])
                    {
                        if(effectsData.type == type)
                        {
                            GeneralEffectSystem.Instance.ApplyEffect(champion, new StatusData(effectsData.key_name, _championData, effectsData.position));
                        }
                    }
                    
                }
            }
        }

    }
}