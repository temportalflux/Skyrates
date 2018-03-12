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
    [CreateAssetMenu(menuName = "Data/AI/Composite/Pipeline")]
    public class SteeringPipeline : Steering
    {

        public Behavior[] Behaviors;

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object persistentData)
        {
            // Update steering on a fixed timestep
            foreach (Behavior behavior in this.Behaviors)
            {
                if (behavior != null)
                {
                    behavior.GetUpdate(ref data, ref physics, deltaTime);
                }
            }

            return persistentData;
        }

    }

}
