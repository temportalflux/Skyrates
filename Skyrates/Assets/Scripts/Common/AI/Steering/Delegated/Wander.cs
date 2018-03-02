using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// 
    /// 
    /// Derived from pg 74 of
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Delegated/Wander")]
    public class Wander : Face
    {

        /// <inheritdoc />
        public override object GetUpdate(ref BehaviorData data, ref PhysicsData physics, float deltaTime, object pData)
        {
            // TODO: Implement Wander steering
            return pData;
        }

    }

}
