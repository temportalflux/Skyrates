using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Data;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.AI;
using UnityEngine;

namespace Skyrates.Client.Input
{

    public class InputInteraction : MonoBehaviour
    {

        public LocalData ControllerData;

        public EntityPlayerShip EntityPlayerShip;

        void Update()
        {
            this.UpdateInput();
        }

        private void UpdateInput()
        {
            if (this.ControllerData.input.ShootRoutineRight == null)
            {
                this.ControllerData.input.ShootRoutineRight = StartCoroutine(this.RoutineShoot(this.ControllerData.input.ShootRight, ShipData.ComponentType.ArtilleryRight));
            }
            if (this.ControllerData.input.ShootRoutineLeft == null)
            {
                this.ControllerData.input.ShootRoutineLeft = StartCoroutine(this.RoutineShoot(this.ControllerData.input.ShootLeft, ShipData.ComponentType.ArtilleryLeft));
            }
        }

        private IEnumerator RoutineShoot(LocalData.InputConfig input, ShipData.ComponentType artillery)
        {
            float timePrevious = Time.time;
            float timeElapsed = 0.0f;
            float cooldownRemaining = 0.0f;
            while (true)
            {
                yield return null;

                timeElapsed = Time.time - timePrevious;
                timePrevious = Time.time;

                cooldownRemaining = Mathf.Max(0, cooldownRemaining - timeElapsed);

                if (cooldownRemaining > 0.0f || !(input.Value > 0.0f))
                    continue;

                this.Shoot(artillery);

                // TODO: Scale delay
                // [0, this.input.ShootDelay]
                //float timeDelay = delay * (1 - this.input.ShootInput);
                cooldownRemaining = this.ControllerData.input.ShootDelay;
            }
        }

        private void Shoot(ShipData.ComponentType artillery)
        {
            this.EntityPlayerShip.Shoot(artillery);
        }

    }

}
