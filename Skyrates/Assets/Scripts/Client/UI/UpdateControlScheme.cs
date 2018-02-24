using Skyrates.Common.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UpdateControlScheme : MonoBehaviour
{

    public UserControlled source;

    private Text render;

    private void Start()
    {
        this.render = this.GetComponent<Text>();
    }

    private void Update()
    {
        this.render.text = this.source.ControlScheme.ToString();
    }

}
