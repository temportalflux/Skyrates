using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // for inspector
public class GameStateData : ISerializing
{
        
    public Client[] clients;

    [Serializable] // for inspector
    public struct Client
    {

        [BitSerialize(0)]
        public uint clientID;

        // Flag that marks the guid as valid, so we don't use invalid guids
        [BitSerialize(1)]
        public bool playerEntityGuidValid;

        // TODO: Put on server
        [BitSerialize(2)]
        public Guid playerEntityGuid;

        // TODO: PhysicsData

    }
    
    public GameStateData()
    {
        this.clients = new Client[0];
    }

    public int GetSize()
    {
        int totalSize = 0;

        // number of clients + how much space the array takes up
        totalSize += sizeof(int) + this.clients.Length * BitSerializeAttribute.GetSizeOf(typeof(Client));

        return totalSize;
    }

    public void Serialize(ref byte[] data, ref int lastIndex)
    {
    }

    /// <summary>
    /// Deserializes _gameStateData from a byte array into this event's _gameStateData
    /// </summary>
    /// <param name="data">The _gameStateData.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <remarks>
    /// Author: Dustin Yost
    /// </remarks>
    public void Deserialize(byte[] data, ref int lastIndex)
    {

        int count = System.BitConverter.ToInt32(data, lastIndex); lastIndex += sizeof(System.Int32);
        this.clients = new Client[count];
        for (int i = 0; i < count; i++)
        {
            this.clients[i] = (Client)BitSerializeAttribute.Deserialize(new Client(), data, ref lastIndex);
        }

    }

}


