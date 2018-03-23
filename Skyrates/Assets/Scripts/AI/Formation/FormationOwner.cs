using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{

    /// <summary>
    /// An object which has references to potential slots which can be filled by AI agents.
    /// </summary>
    public class FormationOwner : MonoBehaviour
    {

        /// <summary>
        /// All slots that AI could potentially occupy.
        /// </summary>
        public Transform[] Slots;

        /// <summary>
        /// Returns the <see cref="PhysicsData"/> for the slot specified.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public PhysicsData GetTarget(int slot)
        {
            if (slot >= 0 && slot < this.Slots.Length)
            {
                return new PhysicsData()
                {
                    LinearPosition = this.Slots[slot].position,
                    RotationPosition = this.Slots[slot].rotation
                };
            }

            return null;
        }

    }

}
