using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : Singleton<PlayerTracker>
{

    public static PlayerTracker Instance;

    public GameState gameState;
    
    void Awake()
    {
        this.loadSingleton(this, ref Instance);
    }

    public static void Spawn(IEnumerable<GameStateData.Client> clients)
    {
        foreach (GameStateData.Client client in clients)
        {

            // TODO: Spawn the client

            Debug.Log("Spawn client " + client.clientID);
            
        }
    }

    public static void Destroy(IEnumerable<GameStateData.Client> clients)
    {
        foreach (GameStateData.Client client in clients)
        {
            // TODO: Destroy it
            Debug.Log("Destroy client " + client.clientID);
        }
    }

}
