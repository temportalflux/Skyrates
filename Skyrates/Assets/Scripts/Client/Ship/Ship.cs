using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{

    public Transform componentRoot;
    public ShipBuilder blueprint;

    private void Start()
    {
        this.blueprint.BuildTo(ref this.componentRoot);
    }

}

