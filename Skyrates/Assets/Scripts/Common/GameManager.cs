using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
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
            this.SpawnPlayer(NetworkComponent.GetSession.PlayerGuid);
        }
    }

    public Entity SpawnEntity(TypeData typeData, Guid guid)
    {
        switch (typeData.EntityType)
        {
            case Entity.Type.Player:
                EntityPlayer entityPlayer = Instantiate(this.EntityList.PrefabEntityPlayer.gameObject).GetComponent<EntityPlayer>();
                entityPlayer.Physics.SetPositionAndRotation(this.playerSpawn.position, this.playerSpawn.rotation);

                // TODO: Use events to let network know that an entity has spawned
                entityPlayer.Init(guid, typeData);
                NetworkComponent.GetNetwork().GetEntityTracker().Add(entityPlayer);

                return entityPlayer;
            default:
                Debug.Log(string.Format("Spawn {0}:{1} {2}", typeData.EntityType, typeData.EntityTypeIndex, guid));
                return null;
        }
    }

    void SpawnPlayer(Guid playerID)
    {
        this.SpawnEntity(new TypeData(Entity.Type.Player, -1), playerID);
    }

}
