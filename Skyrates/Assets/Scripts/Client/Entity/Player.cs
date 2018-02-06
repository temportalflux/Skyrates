using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

public class Player : Entity
{

    // The object which takes input from the local instance
    public GameObject controller;

    public Ship ShipRoot;

    private bool isLocallyControlled;

    public void GenerateShip()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

    /// <summary>
    /// Marks the player as a dummy (cannot be controlled locally)
    /// </summary>
    public void SetDummy(bool isDummy)
    {
        this.isLocallyControlled = !isDummy;

        this.controller.SetActive(this.isLocallyControlled);
    }

    public void IntegratePhysics(PhysicsData physics)
    {
        // TODO: Move this to a steering?
        if (!this.isLocallyControlled)
        {
            this.transform.position = physics.PositionLinear;
            this.ShipRoot.transform.rotation = Quaternion.Euler(physics.PositionRotational);
        }
    }

}
