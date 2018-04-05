using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Decorator
{

    /// <summary>
    /// Offsets the current target in some direction by some amount.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Decorator/Closet Side")]
    public class FindClosestSide : Behavior
    {

        // TODO: Decompose this class into more modular parts

        public float ScaleSide = 1.0f;

        public float ScaleForward = 1.0f;

        /// <inheritdoc />
        public override DataPersistent GetUpdate(ref PhysicsData physics, ref DataBehavioral data, DataPersistent persistent, float deltaTime)
        {
            Vector3 targetL = data.Target.LinearPosition - data.Target.Right * this.ScaleSide;
            Vector3 targetR = data.Target.LinearPosition + data.Target.Right * this.ScaleSide;
            float distSqL = (targetL - physics.LinearPosition).sqrMagnitude;
            float distSqR = (targetR - physics.LinearPosition).sqrMagnitude;

            data.Target.LinearPosition = distSqL < distSqR ? targetL : targetR;

            data.Target.LinearPosition += data.Target.Forward * this.ScaleForward;

            return persistent;
        }

    }

}