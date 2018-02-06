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
        public ClientList ClientList;

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
            this.ClientList = null;
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
            this.ClientList = null;
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

            this.ClientList = new ClientList(session.MaxClients);
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
            ClientData client;
            if (!this.ClientList.TryAdd(evt.SourceAddress, out client))
            {
                Debug.Log("ERROR: Server is full, but another user has connected - disconnecting new user");
                this.Dispatch(new EventDisconnect(), evt.SourceAddress);
                return;
            }

            Debug.Log("Client joined ");
            
            this.Dispatch(new EventHandshakeClientID(client), evt.SourceAddress);
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
            if (!this.ClientList.TryRemove(evtDisconnect.clientID))
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
                //this.DispatchAll(new EventUpdateGameState());
                yield return new WaitForSeconds(this._secondsPerUpdate);
            }
        }

    }

    /// <summary>
    /// Unique information to each client.
    /// Generated by server and sent via <see cref="EventHandshakeClientID"/> and subsequent <see cref="EventUpdateGameState"/>s.
    /// </summary>
    [Serializable]
    public class ClientData
    {

        /// <summary>
        /// NOT SERIALIZED. This is the IP address of this client.
        /// </summary>
        public string Address;

        /// <summary>
        /// The unique ID of the client itself.
        /// </summary>
        [BitSerialize(0)]
        public uint ClientId;

        /// <summary>
        /// The <see cref="Guid"/> (inheirently unique) of the client's player entity.
        /// </summary>
        [BitSerialize(1)]
        public Guid PlayerGuid;

    }

    /// <summary>
    /// Keeps track of all the client addresses and their IDs
    /// </summary>
    public class ClientList
    {

        /// <summary>
        /// The internal single dimensional array of clientID to their address
        /// </summary>
        [BitSerialize(0)]
        public readonly ClientData[] ClientsData;

        public ClientList(int maxClients)
        {
            this.ClientsData = new ClientData[maxClients];
        }

        /// <summary>
        /// Tries to add an address to the list. If possible, returns true and clientID is a valid ID.
        /// </summary>
        /// <param name="address">The IPv4 address</param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool TryAdd(string address, out ClientData client)
        {
            client = null;
            for (int clientID = 0; clientID < this.ClientsData.Length; clientID++)
            {
                if (this.ClientsData[clientID] != null) continue;
                this.ClientsData[clientID] = client = new ClientData {Address = address};
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
            bool validID = clientID < this.ClientsData.Length && this.ClientsData[clientID] != null;
            if (validID) this.ClientsData[clientID] = null;
            return validID;
        }

    }

}