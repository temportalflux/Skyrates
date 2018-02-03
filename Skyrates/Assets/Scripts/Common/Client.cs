using System.Collections;
using System.Collections.Generic;
using ChampNetPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Naming. This class must be renamed. It is too vague.
// essentially this is the base class for both client and server network systems.
// All messages/events are routed through this class
// https://blog.codinghorror.com/i-shall-call-it-somethingmanager/
public class Client
{

    /// <summary>
    /// The collection of events since last fetch
    /// </summary>
    private Queue<NetworkEvent> _events;

    #region Plugin

    public virtual void Create()
    {
        this._events = new Queue<NetworkEvent>();
        NetworkPlugin.Create();
    }

    public virtual void Disconnect()
    {
        // TODO: Force send disconnect packet?

        NetworkPlugin.Disconnect();
    }

    public virtual void Destroy()
    {
        NetworkPlugin.Destroy();
    }

    /// <summary>
    /// Dispatches the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <param name="address">The server address.</param>
    /// <param name="port">The server port.</param>
    public virtual void Dispatch(NetworkEvent evt, string address, int port)
    {
        byte[] data = BitSerializeAttribute.Serialize(evt);
        // Send the event to the address
        NetworkPlugin.SendByteArray(address, port, data, data.Length);
    }

    public void Dispatch(NetworkEvent evt, string address)
    {
        this.Dispatch(evt, address, NetworkComponent.Session.Port);
    }

    public void Dispatch(NetworkEvent evt)
    {
        this.Dispatch(evt, NetworkComponent.Session.TargetAddress, NetworkComponent.Session.Port);
    }

    #endregion

    #region Packets

    public class Packet
    {
        public string sourceAddress;

        public byte[] data;

        public ulong transmitTime;
    }

    public void Fetch()
    {
        NetworkPlugin.FetchPackets();
    }

    /// <summary>
    /// Get the next packet in the series. Returns Null if there was no packet, else an actual <see cref="Packet"/>.
    /// </summary>
    /// <returns>A packet or null</returns>
    public Packet GetNextPacket()
    {
        // The packet to return, null because if there is no packet there should be no return
        Packet packet = null;

        // All the _gameStateData that is sent back via out in NetworkPlugin.PollPacket
        string address;
        byte[] data;
        ulong transmitTime;

        // Get the next packet (returns false if no packet)
        if (NetworkPlugin.PollPacket(out address, out data, out transmitTime))
        {
            // There was packet _gameStateData, so
            // create the packet
            packet = new Packet();
            // and load in the _gameStateData
            packet.sourceAddress = address;
            packet.data = data;
            packet.transmitTime = transmitTime;
        }

        // Return the packet, will be null if there was no _gameStateData, and will exist if there was a packet
        return packet;
    }

    public virtual void UpdateNetwork()
    {
        this.Fetch();

        Packet packet;
        while ((packet = this.GetNextPacket()) != null)
        {
            float transmitTimeMS = packet.transmitTime * 0.001f;

            int messageID = (int) packet.data[0];

            if (messageID == (int)ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_REJECTED)
            {
                Debug.Log("Error: Connection rejected.");
            }

            // Create the network event from the messsage identifier
            NetworkEvent evt;
            bool nonRakNetPacket = MessageMap.CreateFrom(messageID, out evt);

            if (nonRakNetPacket)
            {
                //Debug.Assert(evt != null, "Error: non-raknet event returned null structure");

                if (evt != null)
                {

                    evt.sourceAddress = packet.sourceAddress;
                    evt.transmitTime = transmitTimeMS;

                    // Read off the _gameStateData of the packet
                    evt.Deserialize(packet.data);

                    // Push the event + _gameStateData into the queue for processing
                    this._events.Enqueue(evt);
                }
            }
        }

        this.ProcessEvents();
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
            // execute them
            evt.Execute();
        }
    }

    #endregion

    #region Event

    public virtual void Start(Session session)
    {
        NetworkPlugin.StartClient();
    }

    public virtual void Connect(Session session)
    {
        NetworkPlugin.ConnectToServer(session.TargetAddress, session.Port);
    }

    public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public virtual void JoinServer()
    {
        this.Dispatch(new EventHandshakeJoin(),
            NetworkComponent.Session.TargetAddress,
            NetworkComponent.Session.Port
        );
    }

    #endregion

}
