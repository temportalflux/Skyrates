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

    public static void Spawn(IEnumerable<GameState.Data.Client> clients)
    {
        foreach (GameState.Data.Client client in clients)
        {
            Debug.Log("Spawn client " + client.clientID);

            // Mark in gamestate the client that is this client
            if (client.clientID == NetworkComponent.Session.ClientID)
            {
                Instance.gameState.SetMainClient(client);
            }

        }
    }

    public static void Destroy(IEnumerable<GameState.Data.Client> clients)
    {
        foreach (GameState.Data.Client client in clients)
        {
            Debug.Log("Destroy client " + client.clientID);
        }
    }

}
