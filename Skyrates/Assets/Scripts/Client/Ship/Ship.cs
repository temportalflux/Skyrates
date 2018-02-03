using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ship : MonoBehaviour
{

    public Transform componentRoot;

    public ShipBuilder blueprint;

    // Data from ShipBuilder used to generate during Generate
    // Only valid after Generate
    public ShipData shipData;

    // The generated object, created during Generate
    [HideInInspector]
    public ShipHull hull;

    private void Start()
    {
        this.shipData = null;
        this.hull = null;

        this.Generate();
    }

    public void Destroy()
    {
        this.componentRoot.DestroyChildren();
    }

    public void Generate()
    {
        this.shipData = this.blueprint.GetData();
        this.hull = this.blueprint.BuildTo(ref this.componentRoot, this.shipData);
    }

}

