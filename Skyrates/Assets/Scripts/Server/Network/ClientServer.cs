using System;
using System.Collections;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
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
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            this.Dispatch(new EventDisconnect());
            base.Destroy();
            this.ClientList = null;
            this._secondsPerUpdate = 0.0f;
        }

        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
            NetworkEvents.Instance.HandshakeJoin += this.OnHandshakeJoin;
            NetworkEvents.Instance.HandshakeAccept += this.OnHandshakeAccept;
            NetworkEvents.Instance.Disconnect += this.OnDisconnect;
            NetworkEvents.Instance.RequestSetPlayerPhysics += this.OnRequestSetPlayerPhysics;
            GameManager.Events.PlayerLeft += this.OnPlayerLeft;
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            NetworkEvents.Instance.HandshakeJoin -= this.OnHandshakeJoin;
            NetworkEvents.Instance.HandshakeAccept -= this.OnHandshakeAccept;
            NetworkEvents.Instance.Disconnect -= this.OnDisconnect;
            NetworkEvents.Instance.RequestSetPlayerPhysics -= this.OnRequestSetPlayerPhysics;
            GameManager.Events.PlayerLeft -= this.OnPlayerLeft;
        }

        /// <inheritdoc />
        public override void StartAndConnect(Session session)
        {
            this.StartServer(session);

            this.ClientList = new ClientList(session.MaxClients);
            this._secondsPerUpdate = session.ServerTickUpdate;
            this.EntityTracker = new EntityDispatcher();
            
            NetworkComponent.GetSession.PlayerGuid = Entity.NewGuid();
            NetworkComponent.GetSession.HandshakeComplete = true;

            this.StartCoroutine(this.DispatchGameState());
        }

        private IEnumerator DispatchGameState()
        {
            while (true)
            {
                EventUpdateGameState evt = new EventUpdateGameState();
                evt.GenerateData();
                this.DispatchAll(evt);
                yield return new WaitForSeconds(this._secondsPerUpdate);
            }
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

            Debug.Log(string.Format("Client {0} ({1}) joined, sending GUID {2}", client.ClientId, client.Address, client.PlayerGuid));
            
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
            
            Debug.Log(string.Format("Client {0} has confirmed handshake.", evtAccept.clientID));

            ClientData client = this.ClientList[(int) evtAccept.clientID];

            EntityPlayer e = GameManager.Instance.SpawnEntity(new TypeData(Entity.Type.Player, -1), client.PlayerGuid) as EntityPlayer;
            System.Diagnostics.Debug.Assert(e != null, "e != null");
            e.OwnerNetworkID = (int) client.ClientId;
            e.SetDummy();
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
            ClientData client;
            if (!this.ClientList.TryRemove(evtDisconnect.clientID, out client))
            {
                Debug.Log(string.Format("Error: Cannot remove client with client ID {0}", evtDisconnect.clientID));
                return;
            }

            Debug.Log(string.Format("Client {0} has disconnected", evtDisconnect.clientID));
            
            GameManager.Events.Dispatch(new EventPlayerLeft(client.PlayerGuid));

        }

        /// <summary>
        /// Receives request from client to set the player physics via <see cref="EventRequestSetPlayerPhysics"/>.
        /// Sets the physics of the player associated with the given client.
        /// </summary>
        /// <param name="evt"></param>
        public void OnRequestSetPlayerPhysics(NetworkEvent evt)
        {
            EventRequestSetPlayerPhysics evtSetPlayerPhysics = evt as EventRequestSetPlayerPhysics;
            System.Diagnostics.Debug.Assert(evtSetPlayerPhysics != null, "evtSetPlayerPhysics != null");
            
            // Get client from gamestate
            ClientData client = this.ClientList[(int)evtSetPlayerPhysics.clientID];

            // Set physic variables for the player owned by them
            EntityDynamic e = this.GetEntityTracker().Entities[Entity.Type.Player].Entities[client.PlayerGuid] as EntityDynamic;
            if (e != null)
            {
                e.Physics = evtSetPlayerPhysics.Physics;
            }
        }

        void OnPlayerLeft(GameEvent evt)
        {
            Entity entity;
            if (this.GetEntityTracker().TryGetValue(Entity.Type.Player, ((EventPlayerLeft) evt).PlayerGuid, out entity))
            {
                UnityEngine.Object.Destroy(entity.gameObject);
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
            for (uint clientID = 0; clientID < this.ClientsData.Length; clientID++)
            {
                if (this.ClientsData[clientID] != null) continue;
                this.ClientsData[clientID] = client = new ClientData {Address = address, ClientId = clientID, PlayerGuid = Entity.NewGuid()};
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to remove a clientID/address pair from the list, returns true if the client ID was present
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public bool TryRemove(uint clientID, out ClientData client)
        {
            bool validID = clientID < this.ClientsData.Length && this.ClientsData[clientID] != null;
            client = null;
            if (validID)
            {
                client = this.ClientsData[clientID];
                this.ClientsData[clientID] = null;
            }
            return validID;
        }

        public ClientData this[int index]
        {
            get { return this.ClientsData[index]; }
        }

    }

}