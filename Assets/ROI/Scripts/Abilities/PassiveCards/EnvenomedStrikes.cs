using UnityEngine;
namespace ROI
{
    [CreateAssetMenu(fileName = "EnvenomedStrikes", menuName = "ROI/Data/AbilityPassiveCards/EnvenomedStrikes")]
    public class EnvenomedStrikes : BasePassiveAbilityCard
    {
        public StatusSetting poisonedSetting;
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnStartAlive.Add(new ApplyPoisonedOnEnemy(champion, poisonedSetting));
        }

        private class ApplyPoisonedOnEnemy : IOnStartAlive
        {
            private ChampionData _champion;
            public StatusSetting poisonedSetting;

            public ApplyPoisonedOnEnemy(ChampionData championData, StatusSetting poisonedSetting)
            {
                _champion = championData;
                this.poisonedSetting = poisonedSetting;
            }

            public void OnStartAlive()
            {
                if (PlayerNetwork.Instance.isServer)
                {
                    foreach (var enemy in _champion.enemies)
                    {
                        if (GeneralEffectSystem.Instance.applyEffectActions.TryGetValue(enemy.netId, out _) == false)
                        {
                            GeneralEffectSystem.Instance.applyEffectActions.Add(enemy.netId, data => {});
                        }
                        
                        GeneralEffectSystem.Instance.applyEffectActions[enemy.netId] += ApplyPoisonOnPoisonedChampion;
                    }
                }
            }

            private void ApplyPoisonOnPoisonedChampion(StatusData data)
            {
                if (data.creator.netId == _champion.netId && data.key_name == poisonedSetting.name &&
                    data.level == 0)
                { 
                    Debug.Log($"Apply bonus poisoned to champion{data.target.name}");
                    GeneralEffectSystem.Instance.ApplyEffect(data.target, data);
                }
            }

        }
    }
}
