using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    [CreateAssetMenu(fileName = "AcidicRetribution", menuName = "ROI/Data/AbilityPassiveCards/AcidicRetribution", order = 1)]
    public class AcidicRetribution : BasePassiveAbilityCard
    {
        public float explosionRadius;
        public GameObject explosionVFX;
        public StatusSetting poisonedSetting;
        [SerializeField] private AudioClip skillSound;
        public override void OnInit(ChampionData champion)
        {
            champion.handles.OnDeads.Add(new ExplosionOnDeath(champion, explosionRadius, explosionVFX, poisonedSetting, skillSound));
        }
        private class ExplosionOnDeath:IOnDead
        {
            private ChampionData _championData;
            private float _explosionRadius;
            private GameObject explosionVFX;
            private StatusSetting poisonedSetting;
            private AudioClip skillSound;
            public ExplosionOnDeath(ChampionData data, float explosionRadius, GameObject explosionVFX, StatusSetting poisonedSetting, AudioClip skillSound)
            {
                _championData = data;
                _explosionRadius = explosionRadius;
                this.explosionVFX = explosionVFX;
                this.poisonedSetting = poisonedSetting;
                this.skillSound = skillSound;
            }
            public void OnDead()
            {
                Instantiate(explosionVFX).transform.position = _championData.transform.position;
                SoundManager.PlaySfx(skillSound);
                if(!PlayerNetwork.Instance.isServer) return;
                List<ChampionData> enemiesHitBySkill = new List<ChampionData>();
                foreach (var enemy in _championData.enemies)
                {
                    if (!enemy.IsDeath && Vector3.Distance(enemy.transform.position, _championData.transform.position) < _explosionRadius)
                    {
                        enemiesHitBySkill.Add(enemy);
                    }
                }
                _championData.ApplyEffectToChampionsBySkill(enemiesHitBySkill, poisonedSetting.name);
            }
        }
    }
}

