using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityAI : EntityDynamic
    {

        public EntityDynamic Target;

        protected override void FixedUpdate()
        {
            this.SteeringData.Target = this.Target.Physics;
            base.FixedUpdate();
        }
    }

}
