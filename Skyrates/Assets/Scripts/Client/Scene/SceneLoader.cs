using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public static SceneLoader Instance;

    public SceneData sceneData;

    public struct LoadingSequence
    {

        public string sceneName;

        public Coroutine routine;

        public AsyncOperation operation;

        public Scene loadedScene;

        public void Activate()
        {
            this.operation.allowSceneActivation = true;
        }

    }

    private Queue<LoadingSequence> loadingScenes;

    void Awake()
    {
        this.loadSingleton(this, ref Instance);
        this.loadingScenes = new Queue<LoadingSequence>();
    }

    void Start()
    {
        
        // Load the waiting screen
        this.Enqueue(SceneData.SceneKey.LoadingWorld);

    }

    public void Enqueue(SceneData.SceneKey sceneKey)
    {
        // Create the loading sequence
        LoadingSequence sequence = new LoadingSequence();

        sequence.sceneName = this.sceneData.GetName(sceneKey);

        sequence.operation = SceneManager.LoadSceneAsync(sequence.sceneName, LoadSceneMode.Single);

        // DONT ENABLE THE SCENE YET
        sequence.operation.allowSceneActivation = false;

        sequence.routine = StartCoroutine(this._load(sequence));
        
        this.loadingScenes.Enqueue(sequence);
    }

    private IEnumerator _load(LoadingSequence sequence)
    {
        // Will wait for ActivateNext()/LoadingSequence.Activate() to be called
        yield return sequence.operation;

        // Has finished loading
        sequence.loadedScene = SceneManager.GetSceneByName(sequence.sceneName);

    }

    public void ActivateNext()
    {
        LoadingSequence sequence = this.loadingScenes.Dequeue();
        sequence.Activate();
        // Previous scene unloads cause not additive
    }

}
