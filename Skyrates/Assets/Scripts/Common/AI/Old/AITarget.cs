using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AITarget : Entity
{

    void Start()
    {
        //this.Init(null); // create the guid
        this.GetComponent<Collider>().isTrigger = true;
    }
    
}
