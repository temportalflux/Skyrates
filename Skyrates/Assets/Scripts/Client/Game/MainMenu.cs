﻿using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Scene;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client.Game
{

    /// <summary>
    /// MonoBehavior used in the main menu scene to connect the main menu to the <see cref="GameManager"/> & <see cref="NetworkComponent"/>.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {

        public void StartStandalone()
        {
            NetworkComponent.Instance.StartStandalone();
        }

        public void StartCredits()
        {
            SceneLoader.Instance.Enqueue(SceneData.SceneKey.Credits);
            SceneLoader.Instance.ActivateNext();
        }

    }

}