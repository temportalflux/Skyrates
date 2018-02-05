﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityTracker : Singleton<EntityTracker>
{

    public static EntityTracker Instance;

    public GameState GameState;

    public Player PlayerPrefab;

    public Transform spawn;

    private readonly Dictionary<Guid, Player> _playerEntities = new Dictionary<Guid, Player>();
    private readonly Dictionary<Guid, Entity> _npcEntities = new Dictionary<Guid, Entity>();

    void Awake()
    {
        this.loadSingleton(this, ref Instance);

        Debug.Assert(this.GameState != null);
        Debug.Assert(this.PlayerPrefab != null);
        
    }

    public static bool Contains(Guid entityGuid)
    {
        return Instance._playerEntities.ContainsKey(entityGuid) || Instance._npcEntities.ContainsKey(entityGuid);
    }

    public static bool TryGetValue(Guid id, out Entity e)
    {
        Player p;
        e = null;
        bool hasPlayer = Instance._playerEntities.TryGetValue(id, out p);
        if (hasPlayer) e = p;
        bool hasEntity = hasPlayer || Instance._npcEntities.TryGetValue(id, out e);
        return hasEntity;

    }

    public static void Spawn(GameStateData.Client client)
    {
        Debug.Log("Spawning player for client " + client.clientID);
        SpawnPlayer(client.playerEntityGuid).SetDummy(!client.IsLocalClient);
    }

    public static Player SpawnPlayer(Guid entityID)
    {
        // TODO: This is put on a DoNotDestroyOnLoad object
        Player player = GameObject.Instantiate(Instance.PlayerPrefab.gameObject, Instance.transform).GetComponent<Player>();

        player.transform.SetPositionAndRotation(Instance.spawn.position, Instance.spawn.rotation);

        // Generate identifier
        player.Init(entityID);
        // Generate ship object
        player.GenerateShip();

        Instance._playerEntities.Add(player.GetGuid(), player);

        return player;
    }

    public static void Destroy(GameStateData.Client client)
    {
        Debug.Log("Destroy player for client " + client.clientID);

        if (client.playerEntityGuidValid)
        {

            Debug.Assert(Instance._playerEntities.ContainsKey(client.playerEntityGuid),
                "Cannot destroy an entity that is not tracked");

            Player player = Instance._playerEntities[client.playerEntityGuid];
            Instance._playerEntities.Remove(client.playerEntityGuid);

            Destroy(player.gameObject);

        }

    }

}
