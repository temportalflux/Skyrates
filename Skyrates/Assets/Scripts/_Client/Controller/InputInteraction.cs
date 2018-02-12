using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using UnityEngine;

public class InputInteraction : MonoBehaviour
{
    /// <summary>
    /// Structure to contain input values from controllers.
    /// </summary>
    [Serializable]
    public struct InputData
    {

        // The input which steers the object forward
        [HideInInspector]
        public float ShootInput;

        public Coroutine ShootRoutine;

        [Tooltip("The multiple of the input for ShootInput")]
        public float ShootSpeed;

        public float ShootDelay;

        public float ShootDelayMin;

        public float Shoot
        {
            get { return this.ShootInput * this.ShootSpeed; }
        }
        
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
        this.input.ShootInput = Input.GetAxis("xbox_trigger_r") * this.input.ShootSpeed;
    }

    private void UpdateInput()
    {
        if (this.input.ShootRoutine == null)
        {
            this.input.ShootRoutine = StartCoroutine(this.RoutineShoot());
        }
    }

    private IEnumerator RoutineShoot()
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

            if (cooldownRemaining > 0.0f || !(this.input.Shoot > 0.0f)) continue;

            this.Shoot();
            // TODO: Scale delay
            // [0, this.input.ShootDelay]
            //float timeDelay = delay * (1 - this.input.ShootInput);
            cooldownRemaining = this.input.ShootDelay;
        }
    }

    private void Shoot()
    {
        this.EntityPlayerShip.Shoot();
    }

}
