using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    void Start()
    {
        
        // Load the waiting screen
        this.Enqueue(SceneData.SceneKey.LoadingWorld);

    }

    /// <summary>
    /// Adds a scene to be queued for loading
    /// </summary>
    /// <param name="sceneKey"></param>
    public void Enqueue(SceneData.SceneKey sceneKey)
    {
        // Create the loading sequence
        LoadingSequence sequence = new LoadingSequence
        {
            SceneName = this.SceneData.GetName(sceneKey)
        };

        // Start the loading
        sequence.Operation = SceneManager.LoadSceneAsync(sequence.SceneName, LoadSceneMode.Single);

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

    }

    /// <summary>
    /// Activates the first scene in the loading queue, thereby removing it from the queue and finishing loading.
    /// </summary>
    public void ActivateNext()
    {
        LoadingSequence sequence = this._loadingScenes.Dequeue();
        sequence.Activate();
    }

}
