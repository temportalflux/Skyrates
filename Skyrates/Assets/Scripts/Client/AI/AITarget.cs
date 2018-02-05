using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AITarget : Entity
{

    void Start()
    {
        this.Init(); // create the guid
        this.GetComponent<Collider>().isTrigger = true;
    }
    
}
