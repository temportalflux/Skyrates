using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{

    public Vector2 Velocity;
	
	// Update is called once per frame
    void Update()
    {
        this.transform.position += (Vector3)this.Velocity * Time.deltaTime;
    }

}
