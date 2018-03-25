using System;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    /// <summary>
    /// Overwrites the behavioral target to linearly interpolate from the previous location to the target location.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Smooth Target")]
    public class SmoothTarget : Behavior
    {

        /// <summary>
        /// The speed at which the target will move from its last known location to the next location.
        /// </summary>
        public float InterpolationSpeed;

        /// <summary>
        /// The distance at which the target is snapped to (prevents long decimals to approximate the location).
        /// </summary>
        public float DistanceArrived;

        /// <summary>
        /// <see cref="DistanceArrived"/> * <see cref="DistanceArrived"/>.
        /// </summary>
        private float _distanceArrivedSq;

        /// <inheritdoc />
        [Serializable]
        public class Persistent : DataPersistent
        {
            /// <summary>
            /// The current target of the agent, which is interpolating to its ultimate target in <see cref="DataBehavioral"/>.
            /// </summary>
            public Vector3 TargetInterpolated;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this._distanceArrivedSq = this.DistanceArrived * this.DistanceArrived;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this._distanceArrivedSq = 0.0f;
        }

        /// <inheritdoc />
        public override DataPersistent CreatePersistentData()
        {
            return new Persistent();
        }

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            Persistent data = (Persistent) persistent;

            // Every update, the interpolated target gets closer to the destination target (behavioral target)
            Vector3 directionNormalized = (behavioral.Target.LinearPosition - data.TargetInterpolated).normalized;

            // Determine if the current target is "close enough" to the main target
            // if so, snap there
            if (directionNormalized.sqrMagnitude <= this._distanceArrivedSq)
            {
                data.TargetInterpolated = behavioral.Target.LinearPosition;
            }
            // Otherwise, linearly interpolate to the target
            else
            {
                data.TargetInterpolated += directionNormalized * this.InterpolationSpeed * deltaTime;
            }

            // behavioral target set to the interpolated target so behaviors executed afterwards use the interpolated target
            behavioral.Target.LinearPosition = data.TargetInterpolated;

            return data;
        }

#if UNITY_EDITOR
        public override void DrawGizmos(PhysicsData physics, DataPersistent persistent)
        {
            if (persistent != null)
                Gizmos.DrawWireSphere(((Persistent)persistent).TargetInterpolated, 1);
        }
#endif

    }

}
