using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using Skyrates.Common.AI;
using UnityEngine;

public class InputInteraction : MonoBehaviour
{
    /// <summary>
    /// Structure to contain input values from controllers.
    /// </summary>
    [Serializable]
    public struct InputData
    {
        
        [SerializeField]
        public UserControlled.InputConfig ShootRight;

        [SerializeField]
        public UserControlled.InputConfig ShootLeft;

        public Coroutine ShootRoutineRight;
        public Coroutine ShootRoutineLeft;

        [SerializeField]
        public float ShootDelay;
        [SerializeField]
        public float ShootDelayMin;
        
    }

    public InputData input;

    public EntityPlayerShip EntityPlayerShip;
    
    void Update()
    {
        this.GetInput();
        this.UpdateInput();
    }

    private void GetInput()
    {
        // ForwardInput is left stick (up/down)
        this.input.ShootRight.Input = Input.GetAxis("xbox_trigger_r");
        this.input.ShootLeft.Input = Input.GetAxis("xbox_trigger_l");
    }

    private void UpdateInput()
    {
        if (this.input.ShootRoutineRight == null)
        {
            this.input.ShootRoutineRight = StartCoroutine(this.RoutineShoot(this.input.ShootRight, ShipData.ComponentType.ArtilleryRight));
        }
        if (this.input.ShootRoutineLeft == null)
        {
            this.input.ShootRoutineLeft = StartCoroutine(this.RoutineShoot(this.input.ShootLeft, ShipData.ComponentType.ArtilleryLeft));
        }
    }

    private IEnumerator RoutineShoot(UserControlled.InputConfig input, ShipData.ComponentType artillery)
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

            if (cooldownRemaining > 0.0f || !(input.Value > 0.0f)) continue;

            this.Shoot(artillery);

            // TODO: Scale delay
            // [0, this.input.ShootDelay]
            //float timeDelay = delay * (1 - this.input.ShootInput);
            cooldownRemaining = this.input.ShootDelay;
        }
    }

    private void Shoot(ShipData.ComponentType artillery)
    {
        this.EntityPlayerShip.Shoot(artillery);
    }

}
