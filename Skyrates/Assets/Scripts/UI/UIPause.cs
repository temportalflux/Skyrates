using System.Collections;
using System.Collections.Generic;
using Skyrates.Game;
using Skyrates.Game.Event;
using Skyrates.Scene;
using UnityEngine;

public class UIPause : MonoBehaviour
{

    public void ExitPauseMenu()
    {
        GameManager.Events.Dispatch(EventMenu.Close(EventMenu.CanvasType.Pause));
    }

    public void ExitToMain()
    {
        // Go back to main menu                
        SceneLoader.Instance.Enqueue(SceneData.SceneKey.MenuMain);
        SceneLoader.Instance.ActivateNext();
    }

    public void ExitGame()
    {
        // Exit the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();          
#endif
    }

}
