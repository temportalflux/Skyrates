using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Scene data")]
public class SceneData : ScriptableObject
{

    public enum SceneKey
    {
        MenuMain,
        LoadingWorld,
        World,
    }

    [Tooltip("The name of the menu scene")]
    public string menuName;

    public string gameLoading;

    [Tooltip("The name of the main game scene (what scene is loaded when the player goes to pkay)")]
    public string gameName;

    public readonly Dictionary<SceneKey, string> SceneNames = new Dictionary<SceneKey, string>();

    void OnEnable()
    {
        SceneNames[SceneKey.MenuMain] = menuName;
        SceneNames[SceneKey.LoadingWorld] = gameLoading;
        SceneNames[SceneKey.World] = gameName;
    }

    public string GetName(SceneKey sceneKey)
    {
        return this.SceneNames[sceneKey];
    }

}
