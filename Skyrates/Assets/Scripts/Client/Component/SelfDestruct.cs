using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public float delay;

    void Start()
    {
        Destroy(this.gameObject, this.delay);
    }
    
}
