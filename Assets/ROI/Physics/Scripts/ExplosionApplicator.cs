using System.Collections.Generic;
using UnityEngine;


namespace ROI
{
    public class ExplosionApplicator : PhysicsEffectApplicator
    {
        // Start is called before the first frame update
        public float explosion_force;

        internal void ApplyExplosionForce(Rigidbody body,float up_vector = 0)
        {
            body.AddExplosionForce(explosion_force, center_position, radius, up_vector);
            AddPhysicsDisable(body);
        }

        public void ApplyExplosionForceAOE()
        {
            List<Rigidbody> bodies = GetRigidBodyAoe();

            foreach(Rigidbody bd in bodies)
            {
                ApplyExplosionForce(bd);
            }
        }

    }
}
