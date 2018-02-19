using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Scene
{
    /// <summary>
    /// Data for easily referencing scenes without needing to manually key in scene names all the time.
    /// </summary>
    [CreateAssetMenu(menuName = "Data/Scene data")]
    public class SceneData : ScriptableObject
    {

        /// <summary>
        /// The keys for all scenes used in the build settings.
        /// </summary>
        public enum SceneKey
        {
            MenuMain,
            LoadingWorld,
            World,
            WorldNonClient,
        }

        /// <summary>
        /// The name of the main menu scene.
        /// </summary>
        [Tooltip("The name of the menu scene")]
        public string MenuName;

        /// <summary>
        /// The name of the loading scene.
        /// </summary>
        public string GameLoading;

        /// <summary>
        /// The name of the game world scene.
        /// </summary>
        [Tooltip("The name of the main game scene (what scene is loaded when the player goes to pkay)")]
        public string GameName;

        /// <summary>
        /// The name of the non-client world scene (additively loaded with <see cref="GameName"/>).
        /// </summary>
        [Tooltip("Where all objects go when they are only owned by server")]
        public string WorldNonClient;

        /// <summary>
        /// List of scenes from SceneKey to their scene name.
        /// </summary>
        public readonly Dictionary<SceneKey, string> SceneNames = new Dictionary<SceneKey, string>();
        /// <summary>
        /// List of scenes from their scene name to SceneKey.
        /// </summary>
        public readonly Dictionary<string, SceneKey> NameToScene = new Dictionary<string, SceneKey>();

        /// <inheritdoc />
        private void OnEnable()
        {
            this.Init();
        }

        /// <summary>
        /// Initializes the scene listings IF they are not already populated.
        /// </summary>
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

        /// <summary>
        /// Returns the name of the scene for the scene key.
        /// </summary>
        /// <param name="sceneKey"></param>
        /// <returns></returns>
        public string GetName(SceneKey sceneKey)
        {
            return this.SceneNames[sceneKey];
        }

    }
}