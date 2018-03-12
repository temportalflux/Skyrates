using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Skyrates.Client.Game
{

    /// <summary>
    /// Monobehaviour used to start the main-menu scene on startup (so that there is a scene in which singletons are only loaded once,
    /// and the main scene can be returned to without reloading singletons.
    /// </summary>
    public class StartScene : MonoBehaviour {

        // Use this for initialization
        void Start () {
            SceneManager.LoadScene("main-menu", LoadSceneMode.Single);
        }
	
    }

}