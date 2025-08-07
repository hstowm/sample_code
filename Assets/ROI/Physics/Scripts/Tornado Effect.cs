using ROI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoEffect : PhysicsEffectApplicator
{
    // Start is called before the first frame update
    public float pull_force;
    public float tornado_time;
    public GameObject tornado_effect;


    private bool tornado_active;
    public float lift_force;
    private Coroutine tornado_handle;

    private List<Rigidbody> tornado_affected_bodies;

    public List<Rigidbody> affected;
    void Start()
    {

        tornado_affected_bodies = new List<Rigidbody>();

        //Physics.IgnoreLayerCollision(3, 6);


        Physics.IgnoreLayerCollision(6, 6);
    }
    // Update is called once per frame
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
        body.AddForce(pendicular.normalized * pull_force, ForceMode.Force);
    }
    public void TornadoLift(Rigidbody body)
    {
        Debug.Log("Add force for tornado to body " + body.gameObject.name);

        body.AddForce(Vector3.up * lift_force, ForceMode.Impulse);
    }
    IEnumerator Tornado()
    {
        tornado_effect.SetActive(true);
        tornado_effect.GetComponent<ParticleSystem>().Play();
        tornado_affected_bodies.Clear();
        float counter = 0;
        Debug.Log("Begin active tornado");
        while (counter < tornado_time)
        {
            var list_body = GetInRadiusBody();
            if (list_body.Count > 0)
            {
                foreach (Rigidbody bd in list_body)
                {

                    if (!tornado_affected_bodies.Contains(bd))
                    {
                        AddPhysicsDisable(bd);
                        tornado_affected_bodies.Add(bd);
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

    public void Active()
    {
        tornado_active = !tornado_active;

        if (tornado_active)
        {
            tornado_handle = StartCoroutine(Tornado());
        }
        else
        {
            StopCoroutine(tornado_handle);
        }
    }
}
