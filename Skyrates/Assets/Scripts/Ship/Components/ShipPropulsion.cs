
using UnityEngine;

namespace Skyrates.Ship
{

    public class ShipPropulsion : ShipComponent
    {
        
        // TODO: Doxygen
		public float Thrust;

        public Animator Animator;

        public void SetAnimatorSpeed(float speedPercent)
        {
            if (this.Animator == null)
                return;
            this.Animator.SetFloat("speed", speedPercent);
        }


    }

}
