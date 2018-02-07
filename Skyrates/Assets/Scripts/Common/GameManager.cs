using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public static GameManager Instance;

    public EntityList EntityList;

    public Transform playerSpawn;

    void Awake()
    {
        this.loadSingleton(this, ref Instance);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneLoader.Instance.SceneData.GameName)
        {
            // Do game prep
            this.SpawnPlayer();
        }
    }

    public void SpawnEntity(Entity.Type type, Guid guid)
    {
        // TODO: Spawn shit
        Debug.Log(string.Format("Spawn {0} {1}", type, guid));
    }

    void SpawnPlayer()
    {
        EntityDynamic player = Instantiate(this.EntityList.PrefabPlayer.gameObject).GetComponent<EntityDynamic>();
        player.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);
        
    }

}
