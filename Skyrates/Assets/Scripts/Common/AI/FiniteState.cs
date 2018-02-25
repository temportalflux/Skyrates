using UnityEngine;

namespace Skyrates.Common.AI
{

    /// <summary>
    /// 
    /// Derived from 
    /// Artifical Intelligence for Games 2nd Edition
    /// Ian Millington & John Funge
    /// </summary>
    [CreateAssetMenu(menuName = "Data/AI/Control/State Machine")]
    public class FiniteState : Steering
    {



        /// <inheritdoc />
        /// https://gamedev.stackexchange.com/questions/121469/unity3d-smooth-rotation-for-seek-steering-behavior
        public override PhysicsData GetUpdate(BehaviorData data, PhysicsData physics)
        {
            return physics;
        }

    }

}
