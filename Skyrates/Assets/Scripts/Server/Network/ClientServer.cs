using System;
using System.Collections;
using Skyrates.Common.Network;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network.Event;
using UnityEngine;

namespace Skyrates.Server.Network
{
    [SideOnly(Side.Server)]
    public class ClientServer : NetworkCommon
    {

        /// <summary>
        /// The list of clients connected at any one time
        /// </summary>
        private ClientList _clientList;

        /// <summary>
        /// How many seconds are between game state dispatches
        /// </summary>
        private float _secondsPerUpdate;

        public static ClientServer Instance()
        {
            return NetworkComponent.GetNetwork() as ClientServer;
        }

        /// <inheritdoc />
        public override void Create()
        {
            base.Create();
            this._clientList = null;
            this._secondsPerUpdate = 0.0f;
            NetworkEvents.HandshakeJoin += this.OnHandshakeJoin;
            NetworkEvents.HandshakeAccept += this.OnHandshakeAccept;
            NetworkEvents.Disconnect += this.OnDisconnect;
            NetworkEvents.RequestSetPlayerPhysics += this.OnRequestSetPlayerPhysics;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
            this._clientList = null;
            this._secondsPerUpdate = 0.0f;
            NetworkEvents.HandshakeJoin -= this.OnHandshakeJoin;
            NetworkEvents.HandshakeAccept -= this.OnHandshakeAccept;
            NetworkEvents.Disconnect -= this.OnDisconnect;
            NetworkEvents.RequestSetPlayerPhysics -= this.OnRequestSetPlayerPhysics;
        }

        /// <inheritdoc />
        public override void StartAndConnect(Session session)
        {
            this.StartServer(session);

            this._clientList = new ClientList(session.MaxClients);
            this._secondsPerUpdate = session.ServerTickUpdate;
            this.EntityTracker = new EntityDispatcher();

            this.StartCoroutine(this.DispatchGameState());
        }

        /// <summary>
        /// Receives a handshake request of <see cref="EventHandshakeJoin"/>.
        /// Generates client ID and adds client to listings.
        /// Responds with <see cref="EventHandshakeClientID"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventHandshakeJoin"/></param>
        public void OnHandshakeJoin(NetworkEvent evt)
        {
            uint clientID;
            if (!this._clientList.TryAdd(evt.SourceAddress, out clientID))
            {
                Debug.Log("ERROR: Server is full, but another user has connected - disconnecting new user");
                this.Dispatch(new EventDisconnect(), evt.SourceAddress);
                return;
            }

            Debug.Log("Client joined ");

            // TODO: Create client entry in gamestate
            
            this.Dispatch(new EventHandshakeClientID(clientID), evt.SourceAddress);
        }

        /// <summary>
        /// Receives a handshake request of <see cref="EventHandshakeAccept"/>.
        /// Copies client's player <see cref="System.Guid"/> and marks them as valid.
        /// Does not respond, but client can now start processing <see cref="GameState"/> updates via <see cref="EventUpdateGameState"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventHandshakeAccept"/></param>
        public void OnHandshakeAccept(NetworkEvent evt)
        {
            EventHandshakeAccept evtAccept = evt as EventHandshakeAccept;

            System.Diagnostics.Debug.Assert(evtAccept != null, "evtAccept != null");

            Guid guid = evtAccept.playerEntityGuid;
            
            // TODO: copy over player GUID
            // TODO: Mark client as with valid player GUID
            
        }

        /// <summary>
        /// Receives a notification that a client has disconnected via <see cref="EventDisconnect"/>.
        /// Removes the client via its ID from the client list.
        /// </summary>
        /// <param name="evt"></param>
        public void OnDisconnect(NetworkEvent evt)
        {
            EventDisconnect evtDisconnect = evt as EventDisconnect;
            System.Diagnostics.Debug.Assert(evtDisconnect != null, "evtDisconnect != null");
            if (!this._clientList.TryRemove(evtDisconnect.clientID))
            {
                Debug.Log(string.Format("Error: Cannot remove client with client ID {0}", evtDisconnect.clientID));
            }
        }

        /// <summary>
        /// Receives request from client to set the player physics via <see cref="EventRequestSetPlayerPhysics"/>.
        /// Sets the physics of the player associated with the given client.
        /// </summary>
        /// <param name="evt"></param>
        public void OnRequestSetPlayerPhysics(NetworkEvent evt)
        {
            EventRequestSetPlayerPhysics evtSetPlayerPhysics = evt as EventRequestSetPlayerPhysics;

            // evtSetPlayerPhysics.clientID
            // TODO: Get client from gamestate
            // TODO: Set physic variables for the player owned by them

            // TODO: This requires an EntityTracker

        }

        private IEnumerator DispatchGameState()
        {
            while (true)
            {
                // TODO: Dispatch valid gamestate
                //this.Dispatch(new EventUpdateGameState(), true);
                yield return new WaitForSeconds(this._secondsPerUpdate);
            }
        }

    }

    /// <summary>
    /// Keeps track of all the client addresses and their IDs
    /// </summary>
    public class ClientList
    {

        /// <summary>
        /// The internal single dimensional array of clientID to their address
        /// </summary>
        private readonly string[] _clientAddresses;

        public ClientList(int maxClients)
        {
            this._clientAddresses = new string[maxClients];
        }

        /// <summary>
        /// Tries to add an address to the list. If possible, returns true and clientID is a valid ID.
        /// </summary>
        /// <param name="address">The IPv4 address</param>
        /// <returns></returns>
        public bool TryAdd(string address, out uint clientID)
        {
            for (clientID = 0; clientID < this._clientAddresses.Length; clientID++)
            {
                if (this._clientAddresses[clientID] != null) continue;
                this._clientAddresses[clientID] = address;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to remove a clientID/address pair from the list, returns true if the client ID was present
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public bool TryRemove(uint clientID)
        {
            bool validID = clientID < this._clientAddresses.Length && this._clientAddresses[clientID] != null;
            if (validID) this._clientAddresses[clientID] = null;
            return validID;
        }

    }

}