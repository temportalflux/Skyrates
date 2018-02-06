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

        UpdateGamestate,

        #endregion

        #region ClientData Sent

        //! [Handshake.1] Sent to initiate handshake and ask server to join
        HandshakeJoin,

        //! [Handshake.3] Sent to tell server that we have accepted the client ID (GameState updates can begin)
        HandshakeAccept,

        //! [Update] Sent to server to update physics data about the player
        RequestSetPlayerPhysics,

        #endregion

        #region From/To Both
        
        Disconnect,

        #endregion

    }

    public class NetworkEvents : Singleton<NetworkEvents>
    {
        public static NetworkEvents Instance;

        /// <summary>
        /// Map of <see cref="NetworkEventID"/> to its <see cref="NetworkEvent"/> type, so that data can be deserialized via <see cref="BitSerializeAttribute"/>.
        /// </summary>
        public Dictionary<NetworkEventID, Type> Types;

        /// <summary>
        /// Map of <see cref="NetworkEventID"/> to its <see cref="NetworkEventDelegate"/> event delegate, so that the <see cref="NetworkEvent"/> can be fired by <see cref="NetworkCommon._receiver"/>.
        /// </summary>
        public Dictionary<NetworkEventID, NetworkEventDelegate> Delegates;

        void Awake()
        {
            this.loadSingleton(this, ref Instance);

            this.Types = new Dictionary<NetworkEventID, Type>()
            {

                #region RakNet -> Server
                { NetworkEventID.IncomingConnection, typeof(EventRakNet) },
                #endregion

                #region RakNet -> ClientData
                { NetworkEventID.ConnectionAccepted, typeof(EventRakNet) },
                { NetworkEventID.ConnectionRejected, typeof(EventRakNet) },
                #endregion

                #region RakNet -> Both
                { NetworkEventID.ConnectionLost, typeof(EventRakNet) }, // connection was closed somehow
                { NetworkEventID.ConnectionDropped, typeof(EventRakNet) }, // for client, server shutdown while we were still connected
                #endregion

                #region Server

                {NetworkEventID.HandshakeClientID, typeof(EventHandshakeClientID)},
                {NetworkEventID.UpdateGamestate, typeof(EventUpdateGameState)},

                #endregion

                #region ClientData
            
                {NetworkEventID.HandshakeJoin, typeof(EventHandshakeJoin)},
                {NetworkEventID.HandshakeAccept, typeof(EventHandshakeAccept)},
                {NetworkEventID.RequestSetPlayerPhysics, typeof(EventRequestSetPlayerPhysics)},

                #endregion

                #region From/To Both

                {NetworkEventID.Disconnect, typeof(EventDisconnect) },

                #endregion

            };

            this.Delegates = new Dictionary<NetworkEventID, NetworkEventDelegate>()
            {

                #region RakNet -> Server

                {NetworkEventID.IncomingConnection, IncomingConnection},

                #endregion

                #region RakNet -> ClientData

                {NetworkEventID.ConnectionAccepted, ConnectionAccepted},
                {NetworkEventID.ConnectionRejected, ConnectionRejected},

                #endregion

                #region RakNet -> Both

                {NetworkEventID.ConnectionLost, ConnectionLost},
                {NetworkEventID.ConnectionDropped, ConnectionDropped},

                #endregion

                #region Server

                {NetworkEventID.HandshakeClientID, HandshakeClientID},
                {NetworkEventID.UpdateGamestate, UpdateGamestate},

                #endregion

                #region ClientData

                {NetworkEventID.HandshakeJoin, HandshakeJoin},
                {NetworkEventID.HandshakeAccept, HandshakeAccept},
                {NetworkEventID.RequestSetPlayerPhysics, RequestSetPlayerPhysics},

                #endregion

                #region From/To Both

                {NetworkEventID.Disconnect, Disconnect},

                #endregion

            };

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
        public event NetworkEventDelegate UpdateGamestate;

        #endregion

        #region ClientData Sent
        
        public event NetworkEventDelegate HandshakeJoin;
        public event NetworkEventDelegate HandshakeAccept;
        public event NetworkEventDelegate RequestSetPlayerPhysics;

        #endregion

        #region From/To Both

        public event NetworkEventDelegate Disconnect;

        #endregion

        

    }

}