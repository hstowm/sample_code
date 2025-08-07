

namespace ROI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class FearRandomPositionEffectCC : MonoBehaviour, IEffectApplier<FearRandomEffectData>, IEffectRemover<FearRandomEffectData>
    {
        float timeRandomPosition = 2;
        float rangeRandomPos = 15f;
        float numberLoop = 3;

        Dictionary<uint, IEnumerator> _listEffectOnTime = new Dictionary<uint, IEnumerator>();

        public void ApplyEffect(ChampionData championData, FearRandomEffectData arg)
        {
            
            timeRandomPosition = arg.timeRandomPosition;
            numberLoop = arg.numberLoop;
            //championData.currentEffect.AddEffect(ChampionEffects.Fear);
            // championData.controller.Pause();
            championData.animatorNetwork.animator.SetFloat(AnimHashIDs.MoveSpeed, championData.moveData.moveSpeed);
            championData.agent.OnStartMove();

            arg.netID = championData.netId;
            arg.handle = OnActionFear(championData);
            StartCoroutine(arg.handle);
        }
        /// <summary>
        /// Action fear random
        /// </summary>
        /// <param name="championData"></param>
        /// <returns></returns>
        IEnumerator OnActionFear(ChampionData championData)
        {
            Transform tf = championData.transform;
            float numberLoopInturn = numberLoop;
            float speed = championData.agent.moveSpeed;
            float timeCount = 0;
            float deltatime = Time.fixedDeltaTime;

                while (numberLoopInturn > 0)
                {
                    Vector3 vtRotateChampion = tf.rotation.eulerAngles;
                    Vector3 vtRotateRandom = Vector3.up * Random.Range(0, 360);
                    tf.rotation = Quaternion.Euler(vtRotateRandom);
                    Vector3 vtRandom = tf.TransformPoint(Vector3.forward * rangeRandomPos);
                    tf.rotation = Quaternion.Euler(vtRotateChampion);
                    vtRandom.y = tf.position.y;
                    championData.agent.currentDestination = vtRandom;
                yield return new WaitForSeconds(timeRandomPosition);
            }

            championData.agent.moveSpeed = 0;
           // championData.controller.Resume(); // vi chua xu ly dieu kien nen dung tam de resume
        }

        public void RemoveEffect(ChampionData championData, FearRandomEffectData arg)
        {
            //championData.currentEffect.RemoveEffect(ChampionEffects.Fear);
            championData.animatorNetwork.animator.SetFloat(AnimHashIDs.MoveSpeed, 0);
            championData.agent.moveSpeed = 0;
            //championData.controller.Resume(); // vi chua xu ly dieu kien nen dung tam de resume

            if (arg.netID == championData.netId)
            {
                if (arg.handle != null)
                    StopCoroutine(arg.handle);
            }

        }
    }
}

