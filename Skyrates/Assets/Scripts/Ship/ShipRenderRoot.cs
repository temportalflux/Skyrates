using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using Skyrates.Common.Entity;
using UnityEngine;

public class ShipRenderRoot : MonoBehaviour
{

    public EntityShip Owner;

    protected virtual void OnTriggerEnter(Collider other)
    {
        this.Owner.OnTriggerEnter(other);
    }

}
