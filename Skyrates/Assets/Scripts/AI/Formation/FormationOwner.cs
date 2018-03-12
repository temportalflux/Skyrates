using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.AI.Formation
{

    public class FormationOwner : MonoBehaviour
    {

        public Transform[] Slots;

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
