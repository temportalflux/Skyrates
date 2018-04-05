
using UnityEngine;

namespace Skyrates.Ship
{

    public class ShipNavigation : ShipComponent
    {
		
        //TODO: Doxygen
		public float TurnSpeed;

        public Animator Animator;

        public void SetAnimatorTurning(bool isTurning)
        {
            if (this.Animator == null) return;
            this.Animator.SetBool("isTurning", isTurning);
        }


    }

}
