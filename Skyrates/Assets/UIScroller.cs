using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScroller : MonoBehaviour
{

    public float Speed;

    void Update()
    {
        this.transform.position += Vector3.up * this.Speed * Time.deltaTime;
    }

}
