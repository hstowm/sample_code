using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace ROI
{
    public enum AreaType
    {
        Rect, Radius
    }

    public enum FilterTargetType
    {
        Allies, Enemies, Both
    }

    public class PhysicsEffectApplicator : MonoBehaviour
    {
        public AreaType AOE_Type;
        public float radius;
        public Vector2 RectSize;
        public Vector3 center_position;
        public int max_collider = 20;
        internal List<Rigidbody> affected_body = new List<Rigidbody>();
        internal float limit_veclocity = 0;
        public ChampionData creator;
        public FilterTargetType filterTarget;


        public static T AddApplicator<T>(GameObject gameObject) where T : PhysicsEffectApplicator
        {
            // Logs.Info("PhysicsEffectApplicator => AddApplicator() => gameObject: " + gameObject);
            var t = gameObject.GetComponent<T>();
            if (t)
                return t;

            T new_applicator = gameObject.AddComponent<T>();
            return new_applicator;
        }

        public static T AddApplicator<T>(Vector3 position) where T : PhysicsEffectApplicator
        {
            // Logs.Info("PhysicsEffectApplicator => AddApplicator() => position: " + position);   
            GameObject obj = new GameObject();
            T new_applicator = obj.AddComponent<T>();
            obj.transform.position = position;
            return new_applicator;
        }

        // TODO: This seems useless at the moment, move the entire physics system to a separate global script as units could be affected by multiple physics effects at the same time and these could stack
        private void FixedUpdate()
        {
            // Logs.Info("PhysicsEffectApplicator => FixedUpdate() => Count: " + affected_body.Count);
            if (affected_body.Count > 0)
            {
                for (int i = affected_body.Count - 1; i >= 0; i--)
                {
                    Rigidbody bd = affected_body[i];
                    // Logs.Info("PhysicsEffectApplicator => FixedUpdate() => bd: " + bd + " velocity: " + bd.velocity + " magnitude: " + bd.velocity.magnitude);
                    // TODO: For some reason sometimes the bd.velocity is 0 and the unit is not removed from the list prematurely, moving this to a separate script could fix this
                    if (bd.linearVelocity.magnitude <= limit_veclocity)
                    {
                        RemovePhysicsDisable(bd);
                        // TODO: Add stun effect if unit was flying and now landed with a specific force
                    }
                }
            }
        }

        // TODO: This seems useless at the moment, move the entire physics system to a separate global script as units could be affected by multiple physics effects at the same time and these could stack
        public void RemovePhysicsDisable(Rigidbody bd)
        {
            // Logs.Info("PhysicsEffectApplicator => RemovePhysicsDisable() => bd: " + bd);
            //bd.isKinematic = true;
            if (affected_body.Contains(bd))
            {
                // Logs.Info("PhysicsEffectApplicator => RemovePhysicsDisable() => OK!!!!!!!!!!!!");
                affected_body.Remove(bd);
            }
        }
        
        // TODO: This seems useless at the moment, move the entire physics system to a separate global script as units could be affected by multiple physics effects at the same time and these could stack
        public void AddPhysicsDisable(Rigidbody bd)
        {
            // Logs.Info("PhysicsEffectApplicator => AddPhysicsDisable() => bd:" + bd);
            //bd.isKinematic = false;
            if (!affected_body.Contains(bd))
            {
                // Logs.Info("PhysicsEffectApplicator => AddPhysicsDisable() => OK!!!!!!!!!!!!");
                affected_body.Add(bd);
            }
        }

        public List<Rigidbody> GetRigidBodyAoe()
        {
            return AOE_Type switch
            {
                AreaType.Rect => GetInRectSizeBody(),
                AreaType.Radius => GetInRadiusBody(),
                _ => GetInRadiusBody(),
            };
        }

        // Start is called before the first frame update
        public List<Rigidbody> GetInRadiusBody()
        {
            Collider[] colliders = new Collider[max_collider];
            int amount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, radius, colliders, LayerMask.GetMask("Champion"));

            Debug.Log("Collider get active");

            List<Rigidbody> result = new List<Rigidbody>();

            if (colliders.Length > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    var c = colliders[i];
                    if (!CheckAllegiance(c)) continue;
                    Debug.Log("Collider get pull by Tornadol " + c.gameObject.name);
                    result.Add(c.gameObject.GetComponent<Rigidbody>());
                }
            }
            return result;
        }


        private bool CheckAllegiance(Collider cd)
        {
            if (cd.gameObject.TryGetComponent<ChampionData>(out ChampionData champion))
            {
                if (filterTarget == FilterTargetType.Both) return true;
                return filterTarget == FilterTargetType.Allies ^ !creator.gameObject.CompareTag(champion.gameObject.tag);
            }
            return false;
        }

        public List<Rigidbody> GetInRectSizeBody()
        {
            Vector3 extent = new Vector3(RectSize.x, 1, RectSize.y);
            Collider[] colliders = new Collider[max_collider];
            int amount = Physics.OverlapBoxNonAlloc(gameObject.transform.position, extent, colliders, quaternion.identity, LayerMask.GetMask("Champion"));
            Debug.Log("Collider get active");
            List<Rigidbody> result = new List<Rigidbody>();
            if (colliders.Length > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    var c = colliders[i];
                    if (!CheckAllegiance(c)) continue;
                    Debug.Log("Collider get pull by Tornadol " + c.gameObject.name);
                    result.Add(c.gameObject.GetComponent<Rigidbody>());
                }
            }
            return result;
        }
    }
}
