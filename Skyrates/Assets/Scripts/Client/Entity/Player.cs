using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

    public Ship ShipRoot;

    public void GenerateShip()
    {
        this.ShipRoot.Destroy();
        this.ShipRoot.Generate();
    }

}
