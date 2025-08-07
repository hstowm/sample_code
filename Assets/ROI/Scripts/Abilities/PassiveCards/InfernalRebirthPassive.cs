using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "InfernalRebirthPassive", menuName = "ROI/Data/AbilityPassiveCards/InfernalRebirth")]
    public class InfernalRebirthPassive : BasePassiveAbilityCard
    {
        private ChampionData _championData;
        [SerializeField] private int rangeExplosion = 4;
        [SerializeField] private GameObject objExplosion;
        private MapSystem _mapSystem;
        [SerializeField] private AudioClip skillSound;
        public override void OnInit(ChampionData champion)
        {
            _mapSystem = FindObjectOfType<MapSystem>();
            champion.handles.OnDeads.Add(new InfernalRebirthInject(champion,this, skillSound));
        }
        

        class InfernalRebirthInject : IOnDead
        {
            private ChampionData championData;
            private InfernalRebirthPassive infernalRebirthPassive;
            [SerializeField] private AudioClip skillSound;
            
            private List<ChampionData> GetTargetsNearby()
            {
                var range = infernalRebirthPassive._mapSystem.ConvertToUnit(4);
                var lstTarget = championData.enemies;
                var lstExplosion = new List<ChampionData>();
                for (int i = 0; i < lstTarget.Count; i++)
                {
                    var enemy = lstTarget[i];
                    if (!enemy.IsDeath)
                    {
                        var distance = Vector3.Distance(championData.transform.position, enemy.transform.position);
                        if (distance <= range)
                        {
                            lstExplosion.Add(enemy);
                        }
                    }
                }

                return lstExplosion;
            }
            
            public InfernalRebirthInject(ChampionData championData, InfernalRebirthPassive infernalRebirthPassive, AudioClip skillSound)
            {
                this.championData = championData;
                this.infernalRebirthPassive = infernalRebirthPassive;
                this.skillSound = skillSound;
            }
            
            public void OnDead()
            {
                Debug.Log("dead explosion - by passive");
                var lstTarget = GetTargetsNearby();
                Instantiate(infernalRebirthPassive.objExplosion, championData.transform.position, Quaternion.identity);
                SoundManager.PlaySfx(skillSound);
                for (int i = 0; i < lstTarget.Count; i++)
                {
                    if (!lstTarget[i].IsDeath)
                    {
                        GeneralEffectSystem.Instance.ApplyEffect(lstTarget[i], new StatusData("TimeKeeperPassive_damage", championData, Vector3.zero));
                    }
                }
            }
        }
        
    }
}
