

namespace ROI
{
    using System.Collections;
    using UnityEngine;
    public class FearStayAwayLocationEffectCC : DisplayableEffect, IEffectSystem, IPauseHandle
    {
        
        float timeRandomPosition = 2;

        public void ApplyEffect(ChampionData championData, StatusData arg)
        {
            arg.type = StatusData.EffectType.DeBuff;
            timeRandomPosition = arg.remain_duration;
            championData.currentEffect.AddEffect(ChampionEffects.Fear);
            championData.controller.Pause(this);
            championData.agent.OnStartMove();


            StartCoroutine(OnActionFear(championData, arg.position, arg));

        }
        /// <summary>
        /// Action fear Stay Away Location
        /// </summary>
        /// <param name="championData"></param>
        /// <param name="positionCenterFear">champion must go in the opposite direction </param>
        /// <returns></returns>
        IEnumerator OnActionFear(ChampionData championData, Vector3 positionCenterFear, StatusData arg)
        {
            Transform tf = championData.transform;
            //Calculate direction
            Vector3 _direction = tf.position - positionCenterFear;
            _direction.y = 0;
            Vector3 _lookup_point = tf.position + _direction.normalized * championData.agent.moveSpeed* timeRandomPosition;
            tf.LookAt(_lookup_point);
            ApplyIcon(championData, 1, ChampionEffects.Fear, arg.remain_duration, arg.setting.duration);
            ApplyVFX(championData);
            championData.agent.currentDestination = _lookup_point;
            yield return new WaitForSeconds(timeRandomPosition);
            championData.agent.moveSpeed = 0;
            IsPaused = false; // vi chua xu ly dieu kien nen dung tam de resume
        }

        public void RemoveEffect(ChampionData championData, StatusData arg)
        {
            IsPaused = false; // vi chua xu ly dieu kien nen dung tam de resume
            RemoveIcon(championData, ChampionEffects.Fear, arg.level + 1);
            championData.controller.ResetHexPosition();
            ClearVFX();
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
            if (effectSound)
            {
                SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
            }
        }

        public bool IsPaused { get;  set; }
    }
}
