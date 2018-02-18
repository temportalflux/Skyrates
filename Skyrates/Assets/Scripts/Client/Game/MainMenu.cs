using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Common.Network;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void StartStandalone()
    {
        NetworkComponent.Instance.StartStandalone();
    }

}
