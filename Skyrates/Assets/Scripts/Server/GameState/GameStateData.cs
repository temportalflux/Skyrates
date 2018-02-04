using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // for inspector
public class GameStateData : ISerializing
{
        
    public List<Client> clients;

    [Serializable] // for inspector
    public class Client
    {

        [BitSerialize(0)]
        public uint clientID;

        // Flag that marks the guid as valid, so we don't use invalid guids
        [BitSerialize(1)]
        public bool playerEntityGuidValid;

        [BitSerialize(2)]
        public Guid playerEntityGuid;

        [BitSerialize(3)]
        public PhysicsData physics;

        // TODO: Add to network state
        public uint lootCount;

        public bool IsLocalClient
        {
            get { return this.clientID == NetworkComponent.Session.ClientID; }
        }

        public void Integrate(Client serverState)
        {
            this.clientID = serverState.clientID;

            if (!serverState.playerEntityGuidValid)
            {
                this.playerEntityGuid = Guid.Empty;
            }
            else if (this.playerEntityGuid == Guid.Empty)
            {
                Debug.Log("Found valid player guid");
                this.playerEntityGuid = serverState.playerEntityGuid;
                if (!EntityTracker.Contains(this.playerEntityGuid))
                {
                    EntityTracker.Spawn(this);
                }
            }
            else
            {
                this.physics = serverState.physics;
                Entity e;
                if (EntityTracker.TryGetValue(this.playerEntityGuid, out e))
                {
                    e.IntegratePhysics(this.physics);
                }
            }

            this.lootCount = serverState.lootCount;
        }

    }
    
    public GameStateData()
    {
        this.clients = new List<Client>();
    }

    public int GetSize()
    {
        int totalSize = 0;

        // number of clients + how much space the array takes up
        totalSize += sizeof(int) + this.clients.Count * BitSerializeAttribute.GetSizeOf(typeof(Client));

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
        this.clients.Clear();
        for (int i = 0; i < count; i++)
        {
            this.clients.Add((Client) BitSerializeAttribute.Deserialize(new Client(), data, ref lastIndex));
        }

    }

}


