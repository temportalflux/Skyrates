using Skyrates.Scene;
using UnityEngine;

namespace Skyrates.Game
{

    /// <summary>
    /// MonoBehavior used in the main menu scene to connect the main menu to the <see cref="GameManager"/> & <see cref="NetworkComponent"/>.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {

        public void StartStandalone()
        {
            GameManager.Instance.StartStandalone();
        }

        public void StartCredits()
        {
            SceneLoader.Instance.Enqueue(SceneData.SceneKey.Credits);
            SceneLoader.Instance.ActivateNext();
        }

    }

}