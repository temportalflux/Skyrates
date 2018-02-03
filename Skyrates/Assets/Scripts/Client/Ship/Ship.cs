using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ship : MonoBehaviour
{

    public Transform ComponentRoot;

    public ShipBuilder Blueprint;

    // data from ShipBuilder used to generate during Generate
    // Only valid after Generate
    public ShipData ShipData;

    // The generated object, created during Generate
    [HideInInspector]
    public ShipHull Hull;

    private void Start()
    {
        this.ShipData = null;
        this.Hull = null;

        this.Generate();
    }

    public void Destroy()
    {
        this.ComponentRoot.DestroyChildren();
    }

    public void Generate()
    {
        this.ShipData = this.Blueprint.GetData();
        this.Hull = this.Blueprint.BuildTo(ref this.ComponentRoot, this.ShipData);
    }

}

