using System;
using System.Collections;
using System.Collections.Generic;
using ChampNetPlugin;
using UnityEngine;

public class MessageMap
{

    public enum MessageID
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

        Disconnect,

        #endregion
    }

    private static readonly Dictionary<MessageID, Type> MessageTypes = new Dictionary<MessageID, Type>()
    {
        { MessageID.ConnectionAccepted, typeof(EventConnection) },
        { MessageID.ConnectionRejected, typeof(EventConnection) },
        { MessageID.HandshakeJoin, typeof(EventHandshakeJoin) },
        { MessageID.HandshakeClientID, typeof(EventHandshakeClientID) },
        { MessageID.UpdateGamestate, typeof(EventUpdateGameState) },
    };

    public static bool CreateFrom(int id, out NetworkEvent evt)
    {
        // Get the enumerated version of the ID
        MessageID identifier = (MessageID) id;
        // A place to store the type in the map
        Type msgType;

        evt = null;
        
        // Attempt to retrieve the object
        if (MessageTypes.TryGetValue(identifier, out msgType))
        {
            evt = Activator.CreateInstance(msgType) as NetworkEvent;
        }
        else
        {
            Debug.LogWarning("Received " + id + " without associated structure");
        }

        return evt != null;// id > (int)MessageID.None;
    }



}
