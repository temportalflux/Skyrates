using System;
using System.Collections.Generic;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Composite
{

    /// <summary>
    /// A behavior pipeline in which a set of behaviors are executed in turn.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Composite/Pipeline/Pipeline")]
    public class BehaviorPipeline : BehaviorPipeline<Behavior>
    {

        public override Behavior GetBehaviorFrom(Behavior arrayElement)
        {
            return arrayElement;
        }

        protected override void UpdateElement(ref PhysicsData physics, ref DataBehavioral behavioral, ref Persistent persistent, float deltaTime, Behavior element, ref DataPersistent elementData)
        {
            elementData = element.GetUpdate(ref physics, ref behavioral, elementData, deltaTime);
        }

    }

    /// <summary>
    /// A behavior pipeline in which a set of behaviors are executed in turn.
    /// </summary>
    public abstract class BehaviorPipeline<T> : Steering.Steering
    {

        /// <summary>
        /// The persistent data object for <see cref="BehaviorPipeline"/>.
        /// </summary>
        [Serializable]
        public class Persistent : DataPersistent
        {
            /// <summary>
            /// The <see cref="DataPersistent"/> for each behavior in the pipeline.
            /// </summary>
            public DataPersistent[] PeristentData;
        }

        /// <summary>
        /// The list of behaviors to execute.
        /// </summary>
        public T[] Behaviors;

        /// <summary>
        /// Returns the behavior specified by the object at the array element in <see cref="Behaviors"/>.
        /// </summary>
        /// <param name="arrayElement"></param>
        /// <returns></returns>
        public abstract Behavior GetBehaviorFrom(T arrayElement);

        /// <inheritdoc />
        public override DataPersistent CreatePersistentData()
        {
            // Create a list of persistent data objects
            List<DataPersistent> persistentDataSet = new List<DataPersistent>();
            // Create the persistent data for each sub-behavior
            foreach (T element in this.Behaviors)
            {
                Behavior behavior = this.GetBehaviorFrom(element);
                persistentDataSet.Add(behavior != null ? behavior.CreatePersistentData() : null);
            }
            // Return our data object
            return new Persistent()
            {
                PeristentData = persistentDataSet.ToArray()
            };
        }

        /// <inheritdoc />
        public override DataPersistent OnEnter(PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent)
        {
            // Iterate over all behaviors
            Persistent persistentPipeline = (Persistent) persistent;
            for (int iBehavior = 0; iBehavior < this.Behaviors.Length; iBehavior++)
            {
                // Get the behavior for the list (if this was it, foreach would be better)
                Behavior behavior = this.GetBehaviorFrom(this.Behaviors[iBehavior]);
                // If null, skip execution
                if (behavior == null) continue;
                // Execute OnEnter for the sub-behavior, passing along its persistent data
                persistentPipeline.PeristentData[iBehavior] = behavior.OnEnter(physics, ref behavioral,
                    persistentPipeline.PeristentData[iBehavior]);
            }
            // STUB: this wont actually do anything but return the data passed in
            return base.OnEnter(physics, ref behavioral, persistentPipeline);
        }

        /// <inheritdoc />
        public override void OnExit(PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent)
        {
            // Iterate over all behaviors
            Persistent persistentPipeline = (Persistent)persistent;
            for (int iBehavior = 0; iBehavior < this.Behaviors.Length; iBehavior++)
            {
                // Get the behavior for the list (if this was it, foreach would be better)
                Behavior behavior = this.GetBehaviorFrom(this.Behaviors[iBehavior]);
                // If null, skip execution
                if (behavior == null) continue;
                // Execute OnExit for the sub-behavior, passing along its persistent data
                behavior.OnExit(physics, ref behavioral, persistentPipeline.PeristentData[iBehavior]);
            }
            // STUB: this wont actually do anything
            base.OnExit(physics, ref behavioral, persistentPipeline);
        }

        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral behavioral, DataPersistent persistent, float deltaTime)
        {
            // Iterate over all behaviors
            Persistent persistentPipeline = (Persistent)persistent;
            for (int iBehavior = 0; iBehavior < this.Behaviors.Length; iBehavior++)
            {
                // Get the behavior for the list (if this was it, foreach would be better)
                Behavior behavior = this.GetBehaviorFrom(this.Behaviors[iBehavior]);
                // If null, skip execution
                if (behavior == null) continue;
                // Execute GetUpdate for the sub-behavior, passing along its persistent data
                this.UpdateElement(ref physics, ref behavioral, ref persistentPipeline, deltaTime, this.Behaviors[iBehavior], ref persistentPipeline.PeristentData[iBehavior]);
            }
            // Return the updated persistent data
            return persistentPipeline;
        }

        /// <summary>
        /// Updates physics and data using the specified array element from <see cref="Behaviors"/>.
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="behavioral"></param>
        /// <param name="persistent"></param>
        /// <param name="deltaTime"></param>
        /// <param name="element"></param>
        /// <param name="elementData"></param>
        protected abstract void UpdateElement(ref PhysicsData physics, ref DataBehavioral behavioral, ref Persistent persistent, float deltaTime, T element, ref DataPersistent elementData);

#if UNITY_EDITOR
        public override void DrawGizmos(DataPersistent persistent)
        {
            Persistent pipelinePersistent = (Persistent)persistent;
            for (int iBehavior = 0; iBehavior < this.Behaviors.Length; iBehavior++)
            {
                if (this.Behaviors[iBehavior] == null) continue;
                Behavior behavior = this.GetBehaviorFrom(this.Behaviors[iBehavior]);
                if (behavior == null) continue;
                behavior.DrawGizmos(pipelinePersistent.PeristentData[iBehavior]);
            }
        }
#endif

    }

}
