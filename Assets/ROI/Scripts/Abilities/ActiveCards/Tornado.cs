using System.Collections.Generic;
using UnityEngine;
namespace ROI
{
    public class Tornado : BaseActiveAbilityCard
    {
        public StatusSetting statusTornado;
        GameObject tornado;
        bool isServer;
        public float tornadoExistTime = 2;
        [SerializeField] private AudioClip skillSound;
        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            base.StartSkill(inputPosition, targets, isServer);
            //targetPosition = _championData.transform.position;
            skillsPlayer.PlayFeedbacks();
            this.isServer = isServer;
            SoundManager.PlaySfxPrioritize(skillSound);
        }

        public void OnActionEnemy()
        {
            if(isServer)
            for (int i = 0; i < championsEffectBySkill.Count; i++)
            {
                if(championsEffectBySkill[i] != null)
                {
                    DealDame(championsEffectBySkill[i]);
                }
            }
        }

        public void DealDame(ChampionData enemyData)
        {
            GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData(statusTornado.name, _championData, Vector3.zero));
        }

        public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
        {
           // instantiateObject.GetDynamicObject.AddListener(Fx => objectsDestroyOnSkillDone.Add(Fx));
           if(_championData != null)
                instantiateObject.TargetPosition = _championData.transform.position;
        }

        public void GetTornado(GameObject tornado)
        {
            this.tornado = tornado;
            this.tornado.transform.position = _championData.transform.position;
        }

        public void MoveTornado(ROI_OverridePositionFeedback positionFeedback)
        {
            if (tornado == null) return;
                positionFeedback.AnimatePositionTarget = tornado;
            positionFeedback.InitialPosition = _championData.transform.position;
            positionFeedback.DestinationPosition = targetPosition;
            var tornadoApplicator = PhysicsEffectApplicator.AddApplicator<TornadoApplicator>(tornado);
            //tornadoApplicator. = Mathf.Abs(entry.Value);
            //tornadoApplicator.applied_vector = Vector3.back * entry.Value;
            tornadoApplicator.radius = 2;
            tornadoApplicator.orbit_force = 5;
            tornadoApplicator.pull_force = 10;
            tornadoApplicator.lift = 150 * 2;
            tornadoApplicator.AOE_Type = AreaType.Radius;
            tornadoApplicator.tornado_time = tornadoExistTime;
            tornadoApplicator.creator = _championData;
            tornadoApplicator.filterTarget = FilterTargetType.Enemies;
            tornadoApplicator.tornado_effect = tornado;
            tornadoApplicator.StartTornado();
        }

    }
}
