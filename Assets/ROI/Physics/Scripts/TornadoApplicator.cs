using System.Collections;
using UnityEngine;
namespace ROI
{
    public class TornadoApplicator : PhysicsEffectApplicator
    {
        // Start is called before the first frame update
        public float lift;
        public float pull_force;
        public float orbit_force;
        public float tornado_time;
        public GameObject tornado_effect;
        private Coroutine tornado_handle;

        public void TornadoPull(Rigidbody body)
        {
            Vector3 dir = gameObject.transform.position - body.position;

            dir.y = 0;

            Debug.Log("Vector3 pull is " + dir * 10);
            Vector3 pendicular = Vector3.Cross(dir, Vector3.up);
            pendicular.y = 0;
            Debug.Log("Vector3 orbit  is " + pendicular * 10);

            Debug.Log("Add force for tornado to body " + body.gameObject.name);

            body.AddForce(dir.normalized * pull_force, ForceMode.Force);
            body.AddForce(pendicular.normalized * orbit_force, ForceMode.Force);
            
        }
        public void TornadoLift(Rigidbody body)
        {
            Debug.Log("Add force for tornado to body " + body.gameObject.name);

            body.AddForce(Vector3.up * lift, ForceMode.Impulse);
        }
        IEnumerator Tornado()
        {
            tornado_effect.SetActive(true);
            tornado_effect.GetComponent<ParticleSystem>().Play();
            affected_body.Clear();
            float counter = 0;
            Debug.Log("Begin active tornado");
            while (counter < tornado_time)
            {
                var list_body = GetRigidBodyAoe();
                if (list_body.Count > 0)
                {
                    foreach (Rigidbody bd in list_body)
                    {

                        if (!affected_body.Contains(bd))
                        {
                            AddPhysicsDisable(bd);
                            TornadoLift(bd);
                        }

                        TornadoPull(bd);
                    }
                }
                counter += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
            tornado_effect.SetActive(false);
            yield return null;
        }

        public void StartTornado()
        {

            tornado_handle = StartCoroutine(Tornado());

        }

        public void StopTornado()
        {
            if(tornado_handle != null)
            StopCoroutine(tornado_handle);
        }
    }
}
