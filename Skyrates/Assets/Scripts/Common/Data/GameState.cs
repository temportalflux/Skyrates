using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "data/Game State")]
public partial class GameState : ScriptableObject
{
    
    [BitSerialize()]
    public GameStateData data;

    public void SetClients(GameStateData.Client[] clients)
    {
        this.data.clients = clients;
    }

    public void Integrate(GameStateData serverState, float deltaTimeMS)
    {
        //Debug.Log("Integrate GameState");

        this.IntegrateClients(serverState.clients, deltaTimeMS);

    }

    private void IntegrateClients(GameStateData.Client[] clients, float deltaTimeMS)
    {
        //this.SetClients(clients);

        // TODO: Linq may be expensive, as this function will happen often, this needs more research
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
        // Server state, except those that are on the current (server - current = new)
        IEnumerable<GameStateData.Client> incoming = clients.Except(this.data.clients);
        // Current state, except those that are on the server (current - server = gone)
        IEnumerable<GameStateData.Client> outgoing = this.data.clients.Except(clients);
        // All the ones that stayed
        IEnumerable<GameStateData.Client> stayed = this.data.clients.Intersect(clients);
        
        this.SetClients(stayed.Concat(incoming).ToArray());

        EntityTracker.Destroy(outgoing);
        EntityTracker.Spawn(incoming);

    }

}
