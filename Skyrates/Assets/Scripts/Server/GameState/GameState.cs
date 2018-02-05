using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Skyrates.Common.Network;
using UnityEngine;

[CreateAssetMenu(menuName = "data/Game State")]
public partial class GameState : ScriptableObject
{
    
    [BitSerialize()]
    public GameStateData data;

    public void Integrate(GameStateData serverState, float deltaTimeMS)
    {
        //Debug.Log("Integrate GameState");

        this.IntegrateClients(serverState.clients, deltaTimeMS);

    }

    private void IntegrateClients(List<GameStateData.Client> clients, float deltaTimeMS)
    {
        //this.SetClients(clients);

        // TODO: Linq may be expensive, as this function will happen often, this needs more research
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
        // Server state, except those that are on the current (server - current = new)
        //IEnumerable<GameStateData.Client> incoming = clients.Except(this.data.clients);
        // Current state, except those that are on the server (current - server = gone)
        //IEnumerable<GameStateData.Client> outgoing = this.data.clients.Except(clients);
        // All the ones that stayed
        //IEnumerable<GameStateData.Client> stayed = this.data.clients.Intersect(clients);

        // TODO: Sorting will be weird, find a way to do manually according to clientID

        //GameStateData.Client[] updatedClients = stayed.Concat(incoming).OrderBy(client => client.clientID).ToArray();
        
        //this.SetClients();

        //EntityTracker.Destroy(outgoing);

        // Spawning to be done when the guid is valid
        //EntityTracker.Spawn(incoming);

        Queue<GameStateData.Client> previousClients = new Queue<GameStateData.Client>(this.data.clients.OrderBy(client => client.clientID));
        Queue<GameStateData.Client> orderedClients = new Queue<GameStateData.Client>(clients.OrderBy(client => client.clientID));

        GameStateData.Client cPrev = null;
        GameStateData.Client cInco = null;

        this.data.clients.Clear();

        while (previousClients.Count > 0 && orderedClients.Count > 0 || cPrev != null || cInco != null)
        {
            if (previousClients.Count > 0 && cPrev == null)
                cPrev = previousClients.Dequeue();
            if (orderedClients.Count > 0 && cInco == null)
                cInco = orderedClients.Dequeue();

            // Search until the IDs match
            while ((cPrev != null && cInco == null) || (cPrev == null && cInco != null) || cPrev.clientID != cInco.clientID)
            {
                // A client in the incoming list is not in the previous list
                if ((cPrev == null && cInco != null) || cInco.clientID < cPrev.clientID)
                {
                    // Client is incoming
                    this.IntegrateClient(null, cInco);

                    // Get next client
                    cInco = orderedClients.Count == 0 ? null : orderedClients.Dequeue();
                }
                // A client on the previous list is not in the incoming list
                else if ((cPrev != null && cInco == null) || cPrev.clientID < cInco.clientID)
                {
                    // Client is outgoing
                    this.IntegrateClient(cPrev, null);

                    // Get next client
                    cPrev = previousClients.Count == 0 ? null : previousClients.Dequeue();
                }

                if (cInco == null && cPrev == null)
                    break;
            }
            // there is a client that is still present
            if (cPrev != null && cInco != null)
            {
                // Client has stayed
                this.IntegrateClient(cPrev, cInco);

                // Get next client
                cPrev = previousClients.Count == 0 ? null : previousClients.Dequeue();
                cInco = orderedClients.Count == 0 ? null : orderedClients.Dequeue();
            }
        }
        
        while (previousClients.Count > 0)
        {
            // All remaining are outgoing
            this.IntegrateClient(previousClients.Dequeue(), null);
        }
        
        while (orderedClients.Count > 0)
        {
            // All remaining are incoming
            this.IntegrateClient(null, orderedClients.Dequeue());
        }

    }

    private void IntegrateClient(GameStateData.Client previous, GameStateData.Client next)
    {
        // Incoming
        if (previous == null)
        {
            next.playerEntityGuid = Guid.Empty;
            this.data.clients.Add(next);
        }
        // Outgoing
        else if (next == null)
        {
            EntityTracker.Destroy(previous);
        }
        // Stayed
        else
        {
            previous.Integrate(next);
            this.data.clients.Add(previous);
        }
    }

}
