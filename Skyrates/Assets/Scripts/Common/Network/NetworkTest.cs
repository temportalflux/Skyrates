using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        NetworkComponent.Session.Port = 425;
        NetworkComponent.Instance.StartClient();
    }
    
}
