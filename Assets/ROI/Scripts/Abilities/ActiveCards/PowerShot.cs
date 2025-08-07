using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class PowerShot : BaseActiveAbilityCard
    {
        public float damage;
        public StatusSetting status;
        public DamageTypes damageTypes;
        public Vector3 offset;
        GameObject charge;
        public GameObject bullet;
        ChampionData closet;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            //skillsPlayer.PlayFeedbacks();
            GetComponent<SkillAnimationSequence>().PlaySequence();
            SoundManager.PlaySfxPrioritize(skillSound);
        }

        public void OnDash(GameObject _dash)
        {
            charge = _dash;
            //charge.transform.SetParent(_championData.transform, false);
            charge.transform.position = _championData.transform.position;
            charge.transform.LookAt(targetPosition);
        }

        public void OnMoveChampion(ROI_OverridePositionFeedback data_position)
        {
            Debug.Log(targetPosition);
            data_position.AnimatePositionTarget = _championData.gameObject;
            data_position.DestinationPosition = targetPosition;
            _championData.transform.LookAt(targetPosition, Vector3.up);
            data_position.MoveObjectDone.AddListener(() =>
            {
                charge?.Recycle();
                _championData.controller.ResetHexPosition();
            });
        }

        public void OnGetBullet(GameObject _bullet)
        {
            bullet = _bullet;
            bullet.transform.position = _championData.transform.position + offset;
        }

        public void OnMoveBullet(ROI_OverridePositionFeedback data_position)
        {
            if (_championData.enemies.Count > 0)
            {
                closet = _championData.enemies[0];
                foreach (var target in _championData.enemies)
                {
                    if (Vector3.Distance(target.transform.position, _championData.transform.position) < Vector3.Distance(closet.transform.position, _championData.transform.position)) closet = target;
                }
            }
            data_position.AnimatePositionTarget = bullet;
            data_position.DestinationPosition = closet.transform.position;
            _championData.transform.LookAt(closet.transform.position, Vector3.up);
            bullet.transform.LookAt(closet.transform.position, Vector3.up);
            //data_position.MoveObjectDone.AddListener(() => bullet.Recycle());
        }
        
        public void OnDealDamage()
        {
            bullet.SetActive(false);
            _championData.attacker.AttackEnemy(closet, damage, DamageSources.ActiveCardSkill, damageTypes);
            GeneralEffectSystem.Instance.ApplyEffect(closet, new StatusData(status.name, closet, _championData.transform.position));
        }
        
    }
}
