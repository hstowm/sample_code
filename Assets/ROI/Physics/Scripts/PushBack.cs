using UnityEngine;
namespace ROI
{
    public class PushBack : PhysicsEffectApplicator
    {
        // Start is called before the first frame update
        public float force;
        public float lift;

        public void Active()
        {
            var list_body = GetInRadiusBody();

            foreach (Rigidbody bd in list_body)
            {
                PushBackBody(bd, force);
            }
        }

        public void PushBackBody(Rigidbody body, float power)
        {
            body.mass = 75;
            Vector3 dir = body.gameObject.transform.position - gameObject.transform.position;
            body.AddForce(Vector3.up * lift, ForceMode.Impulse);
            body.AddForce(dir.normalized * power, ForceMode.Impulse);
        }
    }
}
