using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker : Singleton<EntityTracker>
{

    public static EntityTracker Instance;

    public GameState GameState;

    public Player PlayerPrefab;

    private readonly Dictionary<Guid, Player> _playerEntities = new Dictionary<Guid, Player>();
    private readonly Dictionary<Guid, Entity> _npcEntities = new Dictionary<Guid, Entity>();

    void Awake()
    {
        this.loadSingleton(this, ref Instance);

        Debug.Assert(this.GameState != null);
        Debug.Assert(this.PlayerPrefab != null);
        
    }

    public static void Spawn(IEnumerable<GameStateData.Client> clients)
    {
        foreach (GameStateData.Client client in clients)
        {

            Debug.Log("Spawning player for client " + client.clientID);

            Player player = GameObject.Instantiate(Instance.PlayerPrefab.gameObject, Instance.transform).GetComponent<Player>();
            // Generate identifier
            player.Init(client.playerEntityGuid);
            // Generate ship object
            player.GenerateShip();

            Instance._playerEntities.Add(player.GetGuid(), player);
            
        }
    }

    public static void Destroy(IEnumerable<GameStateData.Client> clients)
    {
        foreach (GameStateData.Client client in clients)
        {
            Debug.Log("Destroy player for client " + client.clientID);

            Debug.Assert(Instance._playerEntities.ContainsKey(client.playerEntityGuid), "Cannot destroy an entity that is not tracked");

            Player player = Instance._playerEntities[client.playerEntityGuid];
            Instance._playerEntities.Remove(client.playerEntityGuid);

            Destroy(player.gameObject);

        }
    }

}
