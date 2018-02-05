using System;
using System.Collections.Generic;
using Skyrates.Client.Network.Event;
using Skyrates.Server.Network.Event;

namespace Skyrates.Common.Network
{

    public enum NetworkEventID
    {
        // RakNet IDs
        ConnectionAccepted = ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_ACCEPTED,
        ConnectionRejected = ChampNetPlugin.MessageIDs.CLIENT_CONNECTION_REJECTED,

        // custom packets
        None = ChampNetPlugin.MessageIDs.NONE,

        // Who is it sent by?
        #region Server

        //! [Handshake.2] Sent with the Client's ID on connection
        HandshakeClientID,

        UpdateGamestate,

        #endregion

        #region Client

        //! [Handshake.1] Sent to initiate handshake and ask server to join
        HandshakeJoin,

        //! [Handshake.3] Sent to tell server that we have accepted the client ID (GameState updates can begin)
        HandshakeAccept,

        //! [Update] Sent to server to update physics data about the player
        RequestToUpdatePlayerPhysics,

        Disconnect,

        #endregion
    }

    public class NetworkEvents
    {

        #region Server

        public static event NetworkEventDelegate ConnectionAccepted;
        public static event NetworkEventDelegate ConnectionRejected;
        public static event NetworkEventDelegate HandshakeClientID;
        public static event NetworkEventDelegate UpdateGamestate;

        #endregion

        #region Client
        
        public static event NetworkEventDelegate HandshakeJoin;
        public static event NetworkEventDelegate HandshakeAccept;
        public static event NetworkEventDelegate RequestToUpdatePlayerPhysics;

        #endregion

        /// <summary>
        /// Map of <see cref="NetworkEventID"/> to its <see cref="NetworkEvent"/> type, so that data can be deserialized via <see cref="BitSerializeAttribute"/>.
        /// </summary>
        public static readonly Dictionary<NetworkEventID, Type> Types = new Dictionary<NetworkEventID, Type>()
        {
            #region Server

            {NetworkEventID.ConnectionAccepted, typeof(EventConnection)},
            {NetworkEventID.ConnectionRejected, typeof(EventConnection)},
            {NetworkEventID.HandshakeClientID, typeof(EventHandshakeClientID)},
            {NetworkEventID.UpdateGamestate, typeof(EventUpdateGameState)},

            #endregion

            #region Client
            
            {NetworkEventID.HandshakeJoin, typeof(EventHandshakeJoin)},
            {NetworkEventID.HandshakeAccept, typeof(EventHandshakeAccept)},
            {NetworkEventID.RequestToUpdatePlayerPhysics, typeof(EventRequestToUpdatePlayerPhysics)},

            #endregion
        };

        /// <summary>
        /// Map of <see cref="NetworkEventID"/> to its <see cref="NetworkEventDelegate"/> event delegate, so that the <see cref="NetworkEvent"/> can be fired by <see cref="NetworkCommon._receiver"/>.
        /// </summary>
        public static readonly Dictionary<NetworkEventID, NetworkEventDelegate> Delegates =
            new Dictionary<NetworkEventID, NetworkEventDelegate>()
            {
                #region Server
                
                {NetworkEventID.ConnectionAccepted, ConnectionAccepted},
                {NetworkEventID.ConnectionRejected, ConnectionRejected},
                {NetworkEventID.HandshakeClientID, HandshakeClientID},
                {NetworkEventID.UpdateGamestate, UpdateGamestate},
                
                #endregion

                #region Client
                
                {NetworkEventID.HandshakeJoin, HandshakeJoin},
                {NetworkEventID.HandshakeAccept, HandshakeAccept},
                {NetworkEventID.RequestToUpdatePlayerPhysics, RequestToUpdatePlayerPhysics},

                #endregion
            };

    }

}