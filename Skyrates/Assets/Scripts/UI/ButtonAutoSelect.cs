using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAutoSelect : MonoBehaviour
{

    public Button Button;

    void Start()
    {
        this.Button.Select(); //Select the first button if available.
    }

}
