using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    [CreateAssetMenu(menuName = "Data/AI/Decorator/Smooth Target")]
    public class SmoothTarget : Behavior
    {

        public class Data
        {
            public Vector3 TargetInterpolated;
        }

        public float InterpolationSpeed;

        public float DistanceArrived;
        private float _distanceArrivedSq;

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

        public override object CreatePersistentData()
        {
            return new Data();
        }

        public override object GetUpdate(ref BehaviorData behavioral, ref PhysicsData physics,
            float deltaTime, object persistent)
        {
            Data data = (Data) persistent;

            // Every update, the interpolated target gets closer to the destination target (behavioral target)
            Vector3 directionNormalized = (behavioral.Target.LinearPosition - data.TargetInterpolated).normalized;

            if (directionNormalized.sqrMagnitude <= this._distanceArrivedSq)
            {
                data.TargetInterpolated = behavioral.Target.LinearPosition;
            }
            else
            {
                data.TargetInterpolated += directionNormalized * this.InterpolationSpeed * deltaTime;
            }

            // behavioral target set to the interpolated target so behaviors executed afterwards use the interpolated target
            behavioral.Target.LinearPosition = data.TargetInterpolated;

            return data;
        }

    }

}
