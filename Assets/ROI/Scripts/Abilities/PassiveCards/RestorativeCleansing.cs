using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
    
    /// <summary>
    /// When my abilities affect an ally, 50% chance to remove all negative statuses from them.
    /// </summary>
    
    [CreateAssetMenu(fileName = "RestorativeCleansing", menuName = "ROI/Data/AbilityPassiveCards/RestorativeCleansing")]
    public class RestorativeCleansing : BasePassiveAbilityCard, IOnUseCard
    {
        private ChampionData _championData;
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            champion.handles.OnUseCards.Add(this);
        }
        

        public void OnUseActiveCard(CardSkillData cardSkillType, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
        {
            var chance = Random.RandomRange(0, 100) % 2 == 0;

            if (chance)
            {
                for (int i = 0; i < listTargets.Count; i++)
                {
                    var champion = listTargets[i];
                    if (_championData.allies.Contains( champion))
                    {
                        if (GeneralEffectSystem.ListEffectData.TryGetValue(champion.netId, out _) == false)
                        {
                            return;
                        }
                        var lstEffectsData = GeneralEffectSystem.ListEffectData[champion.netId].ToArray();
                            foreach (var effectsData in lstEffectsData)
                            {
                                if (effectsData.type == StatusData.EffectType.DeBuff)
                                {
                                    GeneralEffectSystem.Instance.RemoveEffect(champion, effectsData);
                                }
                            }
                        
                    }
                }
            }

        }
    }
}


