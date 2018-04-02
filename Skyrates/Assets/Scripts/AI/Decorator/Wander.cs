using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    [CreateAssetMenu(menuName = "Data/AI/Decorator/Wander")]
    public class Wander : Behavior
    {

        public float Distance = 100.0f;

        public float AngleChange = 10.0f;

#if UNITY_EDITOR
        public bool ToggleGizmo = false;
        public Color GizmoColor = Colors.Avocado;
#endif
        
        // https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-wander--gamedev-1624
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent,
            float deltaTime)
        {
            // Calculate the circle center
            Vector3 forward = physics.Forward;
            Vector3 circleCenter = forward * this.Distance;

            // Change wanderAngle just a bit
            float wanderAngle = Random.value * this.AngleChange - this.AngleChange * 0.5f;

            // Calculate the displacement force
            // Randomly change the vector direction
            // by making it change its current angle
            Vector3 displacement = new Vector3(
                Mathf.Cos(wanderAngle),
                0,
                Mathf.Sin(wanderAngle)
            ) * this.Distance;

            // Finally calculate and return the wander force
            behavioral.Target.LinearPosition += circleCenter + displacement;

            return persistent;
        }

#if UNITY_EDITOR
        public override void DrawGizmosSelected(PhysicsData physics, DataPersistent persistent)
        {
            base.DrawGizmos(physics, persistent);
            if (this.ToggleGizmo)
            {
                Gizmos.color = this.GizmoColor;
                Gizmos.DrawWireSphere(physics.LinearPosition + physics.Forward * this.Distance, this.Distance);
            }
        }
#endif

    }

}


