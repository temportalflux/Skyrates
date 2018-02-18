using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

/// <summary>
/// Asynchronously loads and unloads scenes
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    /// <summary>
    /// The singleton instance
    /// </summary>
    public static SceneLoader Instance;

    /// <summary>
    /// The data used to select scenes via <see cref="SceneData.SceneKey"/>
    /// </summary>
    public SceneData SceneData;

    /// <summary>
    /// One loading operation
    /// </summary>
    public struct LoadingSequence
    {

        public SceneData.SceneKey SceneKey;

        /// <summary>
        /// The name of the scene loading
        /// </summary>
        public string SceneName;

        /// <summary>
        /// The <see cref="Coroutine"/> doing the scene loading
        /// </summary>
        public Coroutine Routine;

        /// <summary>
        /// The actual async op that is loading the scene inside the coroutine
        /// </summary>
        public AsyncOperation Operation;

        /// <summary>
        /// The scene, onces <see cref="Operation"/> has finished
        /// </summary>
        public Scene LoadedScene;

        /// <summary>
        /// Allows <see cref="Operation"/> to finish loading the scene (by allowing it to activate)
        /// </summary>
        public void Activate()
        {
            this.Operation.allowSceneActivation = true;
        }

    }

    /// <summary>
    /// The queue of scene loading operation
    /// </summary>
    private Queue<LoadingSequence> _loadingScenes;

    void Awake()
    {
        this.loadSingleton(this, ref Instance);
        this._loadingScenes = new Queue<LoadingSequence>();
        this.SceneData.Init();
    }

    public void Enqueue(SceneData.SceneKey sceneKey)
    {
        this.Enqueue(sceneKey, LoadSceneMode.Single);
    }

    /// <summary>
    /// Adds a scene to be queued for loading
    /// </summary>
    /// <param name="sceneKey"></param>
    /// <param name="mode"></param>
    public void Enqueue(SceneData.SceneKey sceneKey, LoadSceneMode mode)
    {
        // Create the loading sequence
        LoadingSequence sequence = new LoadingSequence
        {
            SceneKey = sceneKey,
            SceneName = this.SceneData.GetName(sceneKey)
        };

        // Start the loading
        sequence.Operation = SceneManager.LoadSceneAsync(sequence.SceneName, mode);

        // DONT ENABLE THE SCENE YET
        sequence.Operation.allowSceneActivation = false;

        // Push operation into coroutine
        sequence.Routine = StartCoroutine(this._load(sequence));
        
        // Add the loading op to the queue (for activation)
        this._loadingScenes.Enqueue(sequence);
    }

    private IEnumerator _load(LoadingSequence sequence)
    {
        // Will wait for ActivateNext()/LoadingSequence.Activate() to be called
        yield return sequence.Operation;

        // Has finished loading
        sequence.LoadedScene = SceneManager.GetSceneByName(sequence.SceneName);

        GameManager.Events.Dispatch(new EventSceneLoaded(sequence.SceneKey));
    }

    /// <summary>
    /// Activates the first scene in the loading queue, thereby removing it from the queue and finishing loading.
    /// </summary>
    public void ActivateNext()
    {
        if (this._loadingScenes.Count <= 0)
        {
            Debug.LogWarning("Tried to active when no scenes were enqueued");
            return;
        }

        LoadingSequence sequence = this._loadingScenes.Dequeue();
        sequence.Activate();
    }
    
}
