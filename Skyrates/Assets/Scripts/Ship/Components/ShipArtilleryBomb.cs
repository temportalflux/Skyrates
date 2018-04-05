using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Mono;
using UnityEngine;

namespace Skyrates.Ship
{

    public class ShipArtilleryBomb : ShipArtillery
    {

        public float DropDelay = 0.25f;
        public int DropAmount = 5;

        public override void Shoot(Func<ShipArtillery, Vector3> getDirection, Vector3 velocity)
        {
            StartCoroutine(this.Drop(getDirection, velocity));
        }

        private IEnumerator Drop(Func<ShipArtillery, Vector3> getDirection, Vector3 velocity)
        {
            int i = 0;
            while (i++ < this.DropAmount)
            {
                base.Shoot(getDirection, velocity);
                yield return new WaitForSeconds(this.DropDelay);
            }
        }

    }

}
