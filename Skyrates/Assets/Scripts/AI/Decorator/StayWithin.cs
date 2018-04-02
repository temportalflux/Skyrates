using System;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    [CreateAssetMenu(menuName = "Data/AI/Decorator/Stay Within")]
    public class StayWithin : Behavior
    {

        [Serializable]
        public class Persistent : DataPersistent
        {
            public Vector3 PositionOnEnter;
        }

        public float Distance = 100.0f;

        public float Influence = 100.0f;

#if UNITY_EDITOR
        public bool ToggleGizmo = false;
        public Color GizmoColor = Colors.Avocado;
#endif

        public override DataPersistent CreatePersistentData()
        {
            return new Persistent
            {
                PositionOnEnter = Vector3.zero,
            };
        }

        public override DataPersistent OnEnter(PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent)
        {
            Persistent data = (Persistent) persistent;
            data.PositionOnEnter = physics.LinearPosition;
            return data;
        }

        // https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-wander--gamedev-1624
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent,
            float deltaTime)
        {
            Vector3 start = ((Persistent) persistent).PositionOnEnter;
            Vector3 vecToStart = start - physics.LinearPosition;
            float distSqFromStart = vecToStart.sqrMagnitude;

            if (distSqFromStart >= this.Distance * this.Distance)
            {
                behavioral.Target.LinearPosition += vecToStart * this.Influence;
            }

            return persistent;
        }

#if UNITY_EDITOR
        public override void DrawGizmosSelected(PhysicsData physics, DataPersistent persistent)
        {
            if (this.ToggleGizmo)
            {
                Gizmos.color = this.GizmoColor;
                Gizmos.DrawWireSphere(((Persistent)persistent).PositionOnEnter, this.Distance);
            }
        }
#endif

    }

}


