using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{
    
    public class FormationSlot : MonoBehaviour
    {

        [SerializeField]
        public FormationOwner Owner;

        [SerializeField]
        public int Slot;

        void Update()
        {
            if (this.Owner == null) return;
            PhysicsData target = this.Owner.GetTarget(this.Slot);
            this.transform.position = target.LinearPosition;
            this.transform.rotation = target.RotationPosition;
        }
        
    }

}
