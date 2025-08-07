using UnityEngine;


namespace ROI
{
    /// <summary>
    /// When I gain Frenzy, remove all debuffs from me.
    /// </summary>
    
    [CreateAssetMenu(fileName = "UnstoppableMomentum", menuName = "ROI/Data/AbilityPassiveCards/UnstoppableMomentum")]

    public class UnstoppableMomentum : BasePassiveAbilityCard
    {
        
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnStartAlive.Add(new UnstoppableMomentumInject(champion));
        }

        class UnstoppableMomentumInject : IOnStartAlive
        {

            private ChampionData _championData;
            
            public UnstoppableMomentumInject(ChampionData championData)
            {
                this._championData = championData;
            }
            
            public void OnStartAlive()
            {
                GeneralEffectSystem.Instance.applyEffectActions[_championData.netId] += ApplyEffectAction;
            }
            
            private void ApplyEffectAction(StatusData statusData)
            {
                if (statusData.creator.netId != _championData.netId)
                {
                    if (statusData.setting.name == "Frenzy")
                    {
                        if (GeneralEffectSystem.ListEffectData.TryGetValue(_championData.netId, out _) == false)
                        {
                            return;
                        }
                        foreach (var effectsData in GeneralEffectSystem.ListEffectData[_championData.netId])
                            {
                                if(effectsData.type == StatusData.EffectType.DeBuff)
                                {
                                    GeneralEffectSystem.Instance.RemoveEffect(_championData, new StatusData(effectsData.key_name, _championData, effectsData.position));
                                }
                            }
                        
                    }
                }
          
            }
        }
        
       

       
    }

}
