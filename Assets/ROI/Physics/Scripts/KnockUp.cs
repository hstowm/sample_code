using UnityEngine;

namespace ROI
{
    public class KnockUp : PhysicsEffectApplicator
    {
        // Start is called before the first frame update

        public float knockup_radius;
        public float knockUp_force;

        public void Active()
        {
            var list_body = GetInRadiusBody();

            foreach (Rigidbody bd in list_body)
            {
                KnockUpBody(bd, knockUp_force);
            }
        }

        public void KnockUpBody(Rigidbody body, float power)
        {
            body.mass = 75;
            //Vector3 dir = body.gameObject.transform.position - gameObject.transform.position;

            Debug.Log("Knock Up " + body.gameObject.name + " with force of " + knockUp_force);
            body.AddForce(Vector3.up * knockUp_force, ForceMode.Impulse);
        }


    }
}
