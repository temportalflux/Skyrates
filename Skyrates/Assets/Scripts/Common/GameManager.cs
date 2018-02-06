using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public EntityList EntityList;

    public Transform playerSpawn;

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

    void SpawnPlayer()
    {
        EntityDynamic player = Instantiate(this.EntityList.PrefabPlayer.gameObject).GetComponent<EntityDynamic>();
        player.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);
        
    }

}
