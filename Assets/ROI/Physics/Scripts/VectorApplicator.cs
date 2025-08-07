using System.Collections.Generic;
using UnityEngine;

namespace ROI
{
    public class VectorApplicator : PhysicsEffectApplicator
    {
        // Start is called before the first frame update
        public Vector3 applied_vector;
        public float vector_force;

        public void ApplyVector(Rigidbody body)
        {
            AddPhysicsDisable(body);
            body.AddForce(applied_vector.normalized * vector_force,ForceMode.Impulse);
        }

        public void ApplyVector(List<Rigidbody> bodies)
        {
            foreach(Rigidbody body in bodies)
            {
                ApplyVector(body);
            }
        }

        public void ApplyVectorAOE()
        {
            List<Rigidbody> bodies = GetRigidBodyAoe();

            ApplyVector(bodies);
        }

    }
}
