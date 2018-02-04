using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSessionID : MonoBehaviour
{

    void Start()
    {
        this.GetComponent<Text>().text = Analytics.SessionID.ToString();
    }

}
