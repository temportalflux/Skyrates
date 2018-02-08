using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;

namespace Skyrates.Server.Network.Event
{

    [NetworkEvent(Side.Server, Side.Client)]
    public class EventUpdateGameState : NetworkEvent, ISerializing
    {

        private int _totalBytes;
        private Queue<byte[]> _serializedData;

        public EventUpdateGameState() : base(NetworkEventID.UpdateGamestate)
        {}

        public void GenerateData()
        {
            this._totalBytes = 0;
            this._serializedData = new Queue<byte[]>();

            ClientServer server = NetworkComponent.GetNetwork() as ClientServer;
            Debug.Assert(server != null, "server != null");

            // EventID
            this._totalBytes += sizeof(byte);
            this._serializedData.Enqueue(BitSerializeAttribute.Serialize(this.EventID));

            // Entities
            EntityTracker tracker = server.GetEntityTracker();
            tracker.GenerateData();
            byte[] trackerData = new byte[tracker.GetSize()];
            int trackerIndex = 0;
            tracker.Serialize(ref trackerData, ref trackerIndex);
            this._totalBytes += trackerData.Length;
            this._serializedData.Enqueue(trackerData);

        }

        /// <inheritdoc />
        public int GetSize()
        {
            return this._totalBytes;
        }

        /// <inheritdoc />
        public void Serialize(ref byte[] data, ref int lastIndex)
        {
            // Merge all byte data
            while (this._serializedData.Count > 0)
            {
                // Get the next chunk
                byte[] dataChunk = this._serializedData.Dequeue();
                // Copy the chunk to the full data
                BitSerializeAttribute.CopyTo(ref data, lastIndex, dataChunk);
                // Increment the index with the number of bytes populated
                lastIndex += dataChunk.Length;
            }
        }

        /// <inheritdoc />
        public void Deserialize(byte[] data, ref int lastIndex)
        {
            if (!NetworkComponent.GetSession.HandshakeComplete) return;

            // Event ID
            this.EventID = (byte) BitSerializeAttribute.Deserialize(this.EventID, data, ref lastIndex);

            Client.Network.Client client = NetworkComponent.GetNetwork() as Client.Network.Client;
            Debug.Assert(client != null, "client != null");

            client.DeserializeGameState(data, ref lastIndex);
        }

    }

}