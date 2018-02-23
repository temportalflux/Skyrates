using System;
using System.Collections.Generic;
using System.Linq;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network.Event;
using UnityEngine;

namespace Skyrates.Common.Network
{

    public enum NetworkEventID
    {
        // RakNet IDs

        #region RakNet -> Server
        IncomingConnection = ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_INCOMING,
        #endregion

        #region RakeNet -> ClientData
        ConnectionAccepted = ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_ACCEPTED,
        ConnectionRejected = ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_REJECTED,
        #endregion

        #region RakNet -> Both
        ConnectionLost = ChampNetPlugin.MessageIDs.CONNECTION_LOST,
        ConnectionDropped = ChampNetPlugin.MessageIDs.CLIENT_DISCONNECTION,
        #endregion

        // custom packets
        None = ChampNetPlugin.MessageIDs.NONE,

        #region Server Sent

        //! [Handshake.2] Sent with the ClientData's ID on connection
        HandshakeClientID,

        #endregion

        #region ClientData Sent

        //! [Handshake.1] Sent to initiate handshake and ask server to join
        HandshakeJoin,

        //! [Handshake.3] Sent to tell server that we have accepted the client ID (GameState updates can begin)
        HandshakeAccept,

        #endregion

        #region From/To Both

        Disconnect,

        #endregion

    }

    /// <summary>
    /// The function template to which subscribes must adhere.
    /// </summary>
    /// <param name="evt">The <see cref="NetworkEvent"/> which has been triggered.</param>
    //public delegate void NetworkEventDelegate<T>(T evt) where T: NetworkEvent;
    public delegate void NetworkEventDelegate(NetworkEvent evt);

    public class NetworkEvents : Singleton<NetworkEvents>, IEventDelegate<NetworkEventID, NetworkEventDelegate>
    {
        public static NetworkEvents Instance;

        /// <summary>
        /// Map of <see cref="NetworkEventID"/> to its <see cref="NetworkEvent"/> type, so that data can be deserialized via <see cref="BitSerializeAttribute"/>.
        /// </summary>
        public readonly Dictionary<NetworkEventID, Type> Types = new Dictionary<NetworkEventID, Type>()
        {

            #region RakNet -> Server
            { NetworkEventID.IncomingConnection, typeof(EventRakNet)
            },
            #endregion

            #region RakNet -> ClientData
            { NetworkEventID.ConnectionAccepted, typeof(EventRakNet)
            },
            { NetworkEventID.ConnectionRejected, typeof(EventRakNet) },
            #endregion

            #region RakNet -> Both
            { NetworkEventID.ConnectionLost, typeof(EventRakNet) }, // connection was closed somehow
            { NetworkEventID.ConnectionDropped, typeof(EventRakNet) }, // for client, server shutdown while we were still connected
            #endregion

            #region Server

            {NetworkEventID.HandshakeClientID, typeof(EventHandshakeClientID)},

            #endregion

            #region ClientData
            
            {NetworkEventID.HandshakeJoin, typeof(EventHandshakeJoin)},
            {NetworkEventID.HandshakeAccept, typeof(EventHandshakeAccept)},

            #endregion

            #region From/To Both

            {NetworkEventID.Disconnect, typeof(EventDisconnect) },

            #endregion

        };
        
        void Awake()
        {
            this.loadSingleton(this, ref Instance);
        }

        #region RakNet -> Server

        public event NetworkEventDelegate IncomingConnection;

        #endregion

        #region RakNet -> ClientData

        public event NetworkEventDelegate ConnectionAccepted;
        public event NetworkEventDelegate ConnectionRejected;

        #endregion

        #region RakNet -> Both

        public event NetworkEventDelegate ConnectionLost;
        public event NetworkEventDelegate ConnectionDropped;

        #endregion

        #region Server Sent

        public event NetworkEventDelegate HandshakeClientID;

        #endregion

        #region ClientData Sent
        
        public event NetworkEventDelegate HandshakeJoin;
        public event NetworkEventDelegate HandshakeAccept;

        #endregion

        #region From/To Both

        public event NetworkEventDelegate Disconnect;

        #endregion

        public NetworkEventDelegate Delegate(NetworkEventID eventID)
        {
            switch (eventID)
            {
                #region RakNet -> Server
                case NetworkEventID.IncomingConnection: return IncomingConnection;
                #endregion
                #region RakNet -> ClientData
                case NetworkEventID.ConnectionAccepted: return ConnectionAccepted;
                case NetworkEventID.ConnectionRejected: return ConnectionRejected;
                #endregion
                #region RakNet -> Both
                case NetworkEventID.ConnectionLost: return ConnectionLost;
                case NetworkEventID.ConnectionDropped: return ConnectionDropped;
                #endregion
                #region Server
                case NetworkEventID.HandshakeClientID: return HandshakeClientID;
                #endregion
                #region ClientData
                case NetworkEventID.HandshakeJoin: return HandshakeJoin;
                case NetworkEventID.HandshakeAccept: return HandshakeAccept;
                #endregion
                #region From/To Both
                case NetworkEventID.Disconnect: return Disconnect;
                #endregion
                case NetworkEventID.None:  return null;
                default:
                    Debug.LogWarning(string.Format("No delegate for event {0}", eventID));
                    return null;
            }
        }

    }

}