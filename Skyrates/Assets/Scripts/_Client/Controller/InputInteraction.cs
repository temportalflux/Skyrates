using System;
using System.Collections;
using System.Collections.Generic;
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

    public EntityPlayer EntityPlayer;
    
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

        if (this.input.Shoot > 0.0f)
        {
            if (this.input.ShootRoutine == null)
            {
                this.input.ShootRoutine = StartCoroutine(this.RoutineShoot(this.input.ShootDelay));
            }
        }
        else
        {
            if (this.input.ShootRoutine != null)
            {
                StopCoroutine(this.input.ShootRoutine);
                this.input.ShootRoutine = null;
            }
        }
        
    }

    private IEnumerator RoutineShoot(float delay)
    {
        while (true)
        {
            this.Shoot();

            // [0, this.input.ShootDelay]
            float timeDelay = delay * (1 - this.input.ShootInput);

            // TODO: Scale delay

            yield return new WaitForSeconds(delay);
        }
    }

    private void Shoot()
    {
        // TODO: Optimize this
        Vector3 forwardAim = this.EntityPlayer.View.forward;
        Vector3 upAim = this.EntityPlayer.View.up;
        ShipComponent[] components = this.EntityPlayer.ShipRoot.Hull.GetGeneratedComponent(ShipData.ComponentType.Artillery);
        foreach (ShipComponent component in components)
        {
            ShipArtillery artillery = (ShipArtillery) component;
            Vector3 forwardArtillery = artillery.transform.forward;
            float dot = Vector3.Dot(forwardAim, forwardArtillery);
            if (dot > 0.3)
            {
                artillery.Shooter.fireProjectile(forwardAim + upAim * 0.02f, this.EntityPlayer.Physics.LinearVelocity);
            }
        }
    }

}
