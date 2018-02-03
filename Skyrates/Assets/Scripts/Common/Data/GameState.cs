using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Game State")]
public class GameState : ScriptableObject
{
    
    [BitSerialize()]
    public Data data;

    [Serializable] // for inspector
    public class Data : ISerializing
    {

        //public int maxClients;
        
        public Client[] clients;

        [Serializable] // for inspector
        public struct Client
        {
            [BitSerialize(0)]
            public uint clientID;
        }

        public Data()
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
        /// Deserializes data from a byte array into this event's data
        /// </summary>
        /// <param name="data">The data.</param>
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

    [Serializable]
    public struct EditorSettings
    {
        public bool toggleClientsBlock;
        public bool[] toggleClientBlocks;
    }

    public EditorSettings editor;

    // data object for THIS client, the local one
    private Data.Client client;

    public void SetClients(Data.Client[] clients)
    {
        this.data.clients = clients;
        Array.Resize(ref this.editor.toggleClientBlocks, this.data.clients.Length);
    }

    public void SetMainClient(Data.Client clients)
    {
        this.client = client;
    }

    public void Integrate(GameState.Data serverState, float deltaTimeMS)
    {
        //Debug.Log("Integrate GameState");

        this.IntegrateClients(serverState.clients, deltaTimeMS);

    }

    private void IntegrateClients(GameState.Data.Client[] clients, float deltaTimeMS)
    {
        //this.SetClients(clients);

        // TODO: Linq may be expensive, as this function will happen often, this needs more research
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
        // Server state, except those that are on the current (server - current = new)
        IEnumerable<Data.Client> incoming = clients.Except(this.data.clients);
        // Current state, except those that are on the server (current - server = gone)
        IEnumerable<Data.Client> outgoing = this.data.clients.Except(clients);
        // All the ones that stayed
        IEnumerable<Data.Client> stayed = this.data.clients.Intersect(clients);
        
        this.SetClients(stayed.Concat(incoming).ToArray());

        PlayerTracker.Destroy(outgoing);
        PlayerTracker.Spawn(incoming);

    }

}
