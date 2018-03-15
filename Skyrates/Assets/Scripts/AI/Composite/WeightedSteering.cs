using System;
using Skyrates.AI;
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
            public Behavior Behavior;
        }

        public SteeringWeight[] Steerings;

        public override void AddPersistentDataTo(ref BehaviorData behavioralData)
        {
            base.AddPersistentDataTo(ref behavioralData);
            foreach (SteeringWeight steeringWeight in this.Steerings)
            {
                if (steeringWeight.Behavior != null)
                {
                    steeringWeight.Behavior.AddPersistentDataTo(ref behavioralData);
                }
            }
        }

        public override object OnEnter(ref BehaviorData data, PhysicsData physics, object persistentData)
        {
            foreach (SteeringWeight steeringWeight in this.Steerings)
            {
                if (steeringWeight.Behavior != null)
                {
                    steeringWeight.Behavior.OnEnter(ref data, physics);
                }
            }
            return base.OnEnter(ref data, physics, persistentData);
        }

        public override void OnExit(ref BehaviorData data, PhysicsData physics, object persistentData)
        {
            foreach (SteeringWeight steeringWeight in this.Steerings)
            {
                if (steeringWeight.Behavior != null)
                {
                    steeringWeight.Behavior.OnExit(ref data, physics);
                }
            }
            base.OnExit(ref data, physics, persistentData);
        }

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
                    weightedSteering.Behavior.GetUpdate(ref data, ref physicsNext, deltaTime);
                    compiled += physicsNext * weightedSteering.Weight;
                }
            }

            return persistentData;
        }

    }

}
