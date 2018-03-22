
using UnityEngine;

namespace Skyrates.Ship
{

    public class ShipNavigation : ShipComponent
    {
		
        //TODO: Doxygen
		public float Maneuverability;

        public Animator Animator;

        public void SetAnimatorTurning(bool isTurning)
        {
            this.Animator.SetBool("isTurning", isTurning);
        }


    }

}
