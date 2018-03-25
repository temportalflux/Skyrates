using System;
using Skyrates.AI;
using Skyrates.AI.Steering;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Composite
{

    /// <summary>
    /// A behavior pipeline in which a set of behaviors are executed in turn, their outputs weighted by a number, and then that data added to the continuous total.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Composite/Pipeline/Weighted")]
    public class WeightedBehavior : BehaviorPipeline<WeightedBehavior.Element>
    {

        /// <summary>
        /// The object which specifies each behavior and its weight.
        /// </summary>
        [Serializable]
        public class Element
        {

            /// <summary>
            /// The multiple of the output for the specified behavior.
            /// </summary>
            [SerializeField]
            public float Weight;

            /// <summary>
            /// The behavior to execute.
            /// </summary>
            [SerializeField]
            public Behavior Behavior;

        }


        /// <inheritdoc />
        public override Behavior GetBehaviorFrom(Element arrayElement)
        {
            return arrayElement.Behavior;
        }

        /// <inheritdoc />
        protected override void UpdateElement(ref PhysicsData physics, ref DataBehavioral behavioral, float deltaTime, Element element, ref DataPersistent elementData)
        {
            // skip nil behaviors
            if (element.Behavior == null) return;

            // Create template data for the next physics operation
            PhysicsData physicsNext = physics.Copy();

            // Execute GetUpdate for the sub-behavior, passing along its persistent data
            elementData = element.Behavior.GetUpdate(ref physicsNext, ref behavioral, elementData, deltaTime);

            // Scale the outputted data by its weight, and then add to the total physics
            physics += physicsNext * element.Weight;
        }

    }
}
