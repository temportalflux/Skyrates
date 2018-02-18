using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for easily referencing scenes without needing to manually key in scene names all the time.
/// </summary>
[CreateAssetMenu(menuName = "Data/Scene data")]
public class SceneData : ScriptableObject
{

    /// <summary>
    /// The keys for all scenes used in the build settings
    /// </summary>
    public enum SceneKey
    {
        MenuMain,
        LoadingWorld,
        World,
        WorldNonClient,
    }

    [Tooltip("The name of the menu scene")]
    public string MenuName;

    public string GameLoading;

    [Tooltip("The name of the main game scene (what scene is loaded when the player goes to pkay)")]
    public string GameName;

    [Tooltip("Where all objects go when they are only owned by server")]
    public string WorldNonClient;

    public readonly Dictionary<SceneKey, string> SceneNames = new Dictionary<SceneKey, string>();
    public readonly Dictionary<string, SceneKey> NameToScene = new Dictionary<string, SceneKey>();

    void OnEnable()
    {
        this.Init();
    }

    public void Init()
    {
        if (this.SceneNames.Count > 0) return;

        SceneNames[SceneKey.MenuMain] = MenuName;
        SceneNames[SceneKey.LoadingWorld] = GameLoading;
        SceneNames[SceneKey.World] = GameName;
        SceneNames[SceneKey.WorldNonClient] = WorldNonClient;

        foreach (KeyValuePair<SceneKey, string> pair in SceneNames)
        {
            this.NameToScene[pair.Value] = pair.Key;
        }
    }

    public string GetName(SceneKey sceneKey)
    {
        return this.SceneNames[sceneKey];
    }

}
