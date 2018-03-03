using System;
using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// Full force steering - just go for it.
    /// 
    /// Derived from pg 57 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Composite/Weighted")]
    public class WeightedSteering : Steering
    {

        [Serializable]
        public class SteeringWeight
        {
            [SerializeField]
            public float Weight;

            [SerializeField]
            public Steering Steering;
        }

        public SteeringWeight[] Steerings;

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persistentData)
        {
            PhysicsData compiled = new PhysicsData()
            {
                LinearPosition = physics.LinearPosition,
                RotationPosition = physics.RotationPosition,
            };

            // Update steering on a fixed timestep
            foreach (SteeringWeight weightedSteering in this.Steerings)
            {
                if (weightedSteering != null)
                {
                    PhysicsData physicsNext = new PhysicsData()
                    {
                        LinearPosition = physics.LinearPosition,
                        RotationPosition = physics.RotationPosition,
                    };
                    weightedSteering.Steering.GetUpdate(ref data, ref physicsNext, deltaTime);
                    compiled += physicsNext * weightedSteering.Weight;
                }
            }

            return persistentData;
        }

    }

}
