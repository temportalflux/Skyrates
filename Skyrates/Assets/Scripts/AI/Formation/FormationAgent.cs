using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{

    public class FormationAgent : MonoBehaviour
    {

        [SerializeField]
        public FormationOwner FormationOwner;

        // TODO: Make editor dropdown which refers to formation owner
        [SerializeField]
        public int FormationSlot;

        public bool HasFormation
        {
            get { return this.FormationOwner != null; }
        }

        public PhysicsData GetTarget()
        {
            return this.FormationOwner != null ? this.FormationOwner.GetTarget(this.FormationSlot) : null;
        }

    }

}
