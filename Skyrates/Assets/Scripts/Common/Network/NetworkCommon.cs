using System;
using System.Collections;
using System.Collections.Generic;
using ChampNetPlugin;
using Skyrates.Common.Entity;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network;
using UnityEngine;

namespace Skyrates.Common.Network
{

    /// <summary>
    /// The function template to which subscribes must adhere.
    /// </summary>
    /// <param name="evt">The <see cref="NetworkEvent"/> which has been triggered.</param>
    //public delegate void NetworkEventDelegate<T>(T evt) where T: NetworkEvent;
    public delegate void NetworkEventDelegate(NetworkEvent evt);

    /// <summary>
    /// The base class for any <see cref="Side"/>d network.
    /// TODO: This naming is too vague
    /// essentially this is the base class for both client and server network systems.
    /// All messages/events are routed through this class
    /// https://blog.codinghorror.com/i-shall-call-it-somethingmanager/
    /// 
    /// Usage: Create, Start[ClientData/Server], [Connect], Send/Receive events, Shutdown, Destroy
    /// 
    /// </summary>
    public abstract class NetworkCommon
    {

        /// <summary>
        /// The packet dispatcher
        /// </summary>
        private PacketDispatcher _dispatcher;

        /// <summary>
        /// The packet receiver
        /// </summary>
        private PacketReceiver _receiver;

        protected EntityTracker EntityTracker;

        /// <summary>
        /// Create the network object in the backend. Must use <see cref="Destroy"/> when done.
        /// </summary>
        public virtual void Create()
        {
            this._dispatcher = new PacketDispatcher();
            this._receiver = new PacketReceiver();
            this.EntityTracker = null;
            NetworkPlugin.Create();
        }

        /// <summary>
        /// Destroy the network object in the backend. Must use <see cref="Create"/> beforehand.
        /// </summary>
        public virtual void Destroy()
        {
            this.EntityTracker = null;
            NetworkPlugin.Destroy();
        }

        #region Eeehh
        // TODO: ORganize!!!

        public bool HasSubscribed;

        public virtual void SubscribeEvents()
        {
            this.HasSubscribed = true;
        }

        public virtual void UnsubscribeEvents()
        {
            this.HasSubscribed = false;
        }

        #endregion

        /// <summary>
        /// Called after <see cref="Create"/> but before <see cref="Destroy"/> to start the network in its default configuration.
        /// Implementation should call <see cref="StartClient"/> or <see cref="StartServer"/>, and <see cref="Connect"/> if applicable.
        /// </summary>
        public abstract void StartAndConnect(Session session);

        /// <summary>
        /// Starts the network and sets incoming connections to 1.
        /// </summary>
        /// <param name="session"></param>
        public virtual void StartClient(Session session)
        {
            NetworkPlugin.StartClient();
        }

        /// <summary>
        /// Starts the network using <see cref="Session.Port"/> and sets outgoing connections to <see cref="Session.MaxClients"/>.
        /// </summary>
        /// <param name="session"></param>
        public virtual void StartServer(Session session)
        {
            NetworkPlugin.StartServer(session.Port, session.MaxClients);
        }

        /// <summary>
        /// Connects the network to some target defined by <see cref="Session.TargetAddress"/> and <see cref="Session.Port"/>.
        /// </summary>
        /// <param name="session"></param>
        public virtual void Connect(Session session)
        {
            NetworkPlugin.ConnectToServer(session.TargetAddress, session.Port);
        }

        /// <summary>
        /// TODO: Does this actually link with StartClient/Server or with Connect?
        /// Shutsdown the network interface. Must be called after 
        /// </summary>
        public virtual void Shutdown()
        {
            // TODO: Force send disconnect packet?
            NetworkPlugin.Disconnect();
        }
        
        /// <summary>
        /// Enqueues the network event for dispatching. Dispatched during <see cref="Update"/>.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <param name="address">The server address.</param>
        /// <param name="port">The server port.</param>
        public virtual void Dispatch(NetworkEvent evt, string address, int port, bool broadcast = false)
        {
            // Will only allow server sent messages through
            Session.NetworkMode mode = NetworkComponent.GetSession.Mode;
            Side side = mode == Session.NetworkMode.Host ? Side.Server : Side.Client;
            Debug.Assert(evt.IsSentBy(side), string.Format(
                "{0} sent message attempting to be sent from {1}, these must be mitigated or stopped. {2}",
                side, side == Side.Client ? Side.Server : Side.Client, (NetworkEventID)evt.EventID
            ));
            this._dispatcher.Enqueue(evt, address, port, broadcast);
        }

        /// <summary>
        /// Dispatches the specified event, using the port in <see cref="NetworkComponent"/>.<see cref="Session"/>.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <param name="address">The server address.</param>
        public void Dispatch(NetworkEvent evt, string address, bool broadcast = false)
        {
            this.Dispatch(evt, address, NetworkComponent.GetSession.Port, broadcast);
        }

        /// <summary>
        /// Dispatches the specified event, using the target address and port in <see cref="NetworkComponent"/>.<see cref="Session"/>.
        /// </summary>
        /// <param name="evt">The event.</param>
        public void Dispatch(NetworkEvent evt)
        {
            this.Dispatch(evt, NetworkComponent.GetSession.TargetAddress, false);
        }

        /// <summary>
        /// Dispatches the specified event, using broadcasting to all others connected on the port in <see cref="NetworkComponent"/>.<see cref="Session"/>.
        /// </summary>
        /// <param name="evt">The event.</param>
        public void DispatchAll(NetworkEvent evt)
        {
            this.Dispatch(evt, NetworkComponent.GetSession.Address, true);
        }

        /// <summary>
        /// Updates the network, dispatching all current packets and receiving any packets in the network (in that order).
        /// </summary>
        public virtual void Update()
        {
            this._dispatcher.Update();
            this._receiver.Update();
        }

        protected Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return NetworkComponent.Instance.StartCoroutine(coroutine);
        }

        protected void StopCoroutine(Coroutine coroutine)
        {
            NetworkComponent.Instance.StopCoroutine(coroutine);
        }

        public EntityTracker GetEntityTracker()
        {
            return this.EntityTracker;
        }

    }

    /// <summary>
    /// Handles shipping out packets at the most opportune moment.
    /// </summary>
    public class PacketDispatcher
    {

        /// <summary>
        /// Local structure to keep track of data to be sent out at the next chance
        /// </summary>
        private struct EventData
        {
            public readonly byte[] Data;
            public readonly string Address;
            public readonly int Port;
            public readonly bool Broadcast;

            public EventData(byte[] data, string address, int port, bool broadcast)
            {
                this.Data = data;
                this.Address = address;
                this.Port = port;
                this.Broadcast = broadcast;
            }
        }

        /// <summary>
        /// The collection of events to dispatch
        /// </summary>
        private readonly Queue<EventData> _events;

        public PacketDispatcher()
        {
            this._events = new Queue<EventData>();
        }

        /// <summary>
        /// Adds data to the event queue to be processed in the next <see cref="Update"/> loop.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void Enqueue(NetworkEvent evt, string address, int port, bool broadcast)
        {
            this._events.Enqueue(new EventData(evt.Serialize(), address, port, broadcast));
        }

        /// <summary>
        /// Dispatches all packet data added by <see cref="Enqueue"/>.
        /// </summary>
        public void Update()
        {
            while (this._events.Count > 0)
            {
                EventData evt = this._events.Dequeue();
                this.Dispatch(evt.Data, evt.Address, evt.Port, evt.Broadcast);
            }
        }

        /// <summary>
        /// Dispatches the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <param name="address">The server address.</param>
        /// <param name="port">The server port.</param>
        public virtual void Dispatch(byte[] data, string address, int port, bool broadcast)
        {
            // Send the event to the address
            NetworkPlugin.SendByteArray(address, port, data, data.Length, broadcast);
        }

    }

    /// <summary>
    /// Handles receiving any packets in the network and executing them.
    /// Coupled to <see cref="MessageMap"/> and <see cref="NetworkEvent"/>.
    /// </summary>
    public class PacketReceiver
    {

        /// <summary>
        /// Local structure used during processing of events in <see cref="Update"/>.
        /// </summary>
        private class EventData
        {
            public byte[] Data;
            public string SourceAddress;
            public ulong TransmitTime;
        }

        /// <summary>
        /// The collection of events since last fetch
        /// </summary>
        private readonly Queue<NetworkEvent> _events;

        public PacketReceiver()
        {
            this._events = new Queue<NetworkEvent>();
        }

        /// <summary>
        /// Tells the <see cref="ChampNetPlugin"/> to fetch all packets from RakNet networking.
        /// </summary>
        public void Fetch()
        {
            NetworkPlugin.FetchPackets();
        }

        /// <summary>
        /// Get the next packet in the series. Returns Null if there was no packet, else an actual <see cref="Packet"/>.
        /// </summary>
        /// <returns>A packet or null</returns>
        private EventData GetNextPacket()
        {
            // The packet to return, null because if there is no packet there should be no return
            EventData evt = null;

            // All the data that is sent back via out in NetworkPlugin.PollPacket
            string address;
            byte[] data;
            ulong transmitTime;

            // Get the next packet (returns false if no packet)
            if (NetworkPlugin.PollPacket(out address, out data, out transmitTime))
            {
                // There was packet data, so
                // create the packet
                evt = new EventData();
                // and load in the data
                evt.Data = data;
                evt.SourceAddress = address;
                evt.TransmitTime = transmitTime;
            }

            // Return the packet, will be null if there was no _gameStateData, and will exist if there was a packet
            return evt;
        }

        /// <summary>
        /// Grabs all packet data from the <see cref="ChampNetPlugin"/>, and deserializes as some <see cref="NetworkEvent"/> in <see cref="MessageMap"/>.
        /// </summary>
        private void RetreiveBackendPackets()
        {
            EventData evt;
            while ((evt = this.GetNextPacket()) != null)
            {
                float transmitTimeMS = evt.TransmitTime * 0.001f;

                int messageID = (int)evt.Data[0];

                if (messageID == (int)ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_REJECTED)
                {
                    UnityEngine.Debug.Log("Error: Connection rejected.");
                }

                // Create the network event from the messsage identifier
                NetworkEvent netEvent;
                bool nonRakNetPacket = this.CreateFrom(messageID, out netEvent);

                if (nonRakNetPacket)
                {
                    //Debug.Assert(evt != null, "Error: non-raknet event returned null structure");

                    if (evt != null)
                    {

                        netEvent.SourceAddress = evt.SourceAddress;
                        netEvent.TransmitTime = transmitTimeMS;

                        // Read off the _gameStateData of the packet
                        netEvent.Deserialize(evt.Data);

                        // Push the event + _gameStateData into the queue for processing
                        this._events.Enqueue(netEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a NetworkEvent from its ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        private bool CreateFrom(int id, out NetworkEvent evt)
        {
            // Get the enumerated version of the ID
            NetworkEventID identifier = (NetworkEventID)id;
            // A place to store the type in the map
            Type msgType;

            evt = null;

            // Attempt to retrieve the object
            if (NetworkEvents.Instance.Types.TryGetValue(identifier, out msgType))
            {
                evt = Activator.CreateInstance(msgType) as NetworkEvent;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Received " + id + " without associated structure");
            }

            return evt != null;// id > (int)NetworkEventID.None;
        }


        /// <summary>
        /// Checks for events in the queue and returns the first if there are any.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <returns>true if there is an event</returns>
        private bool PollEvent(out NetworkEvent evt)
        {
            // ensure the event out is always something
            evt = null;
            // check if there are events in the queue
            if (this._events.Count > 0)
            {
                // get the first event
                evt = this._events.Dequeue();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Processes the events in the event queue.
        /// </summary>
        private void ProcessEvents()
        {
            // While there are events
            NetworkEvent evt;
            while (this.PollEvent(out evt))
            {
                // Find the delegate to fire
                NetworkEventDelegate evtDelegate = NetworkEvents.Instance.Delegate((NetworkEventID) evt.EventID);
                if (evtDelegate != null)
                {
                    // Fire off the delegate
                    evtDelegate.Invoke(evt);
                }
                else
                {
                    //Debug.LogWarning(string.Format("Delegate for {0} was null...", (NetworkEventID) evt.EventID));
                    Debug.LogWarning("Could not fire off event " + (NetworkEventID) evt.EventID + ", no event delegate.");
                }
            }
        }
        
        /// <summary>
        /// Fetches and Processes all events stored in the network.
        /// </summary>
        public void Update()
        {
            this.Fetch();
            this.RetreiveBackendPackets();
            this.ProcessEvents();
        }

    }
    
}
