using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityOwner : EntityTracker
    {
        
        private byte[] _serializedData;

        /// <inheritdoc />
        /// <summary>
        /// Paired with <see cref="EntityReceiver.Deserialize"/>.
        /// </summary>
        public override void GenerateData()
        {
            // Clear any previous data
            this._serializedData = null;

            // Cache to total number of bytes
            int totalBytes = 0;

            // Cache to hold all bytes to be combined later
            Queue<byte[]> bytes = new Queue<byte[]>();

            byte[] dataTypes = BitSerializeAttribute.Serialize(Entity.AllTypes.Length);
            totalBytes += dataTypes.Length;
            bytes.Enqueue(dataTypes);

            foreach (Entity.Type type in Entity.AllTypes)
            {
                EntitySet set = this._entities[type];

                // Serialize the number of entities
                byte[] dataEntityCount = BitSerializeAttribute.Serialize(set._entities.Count);
                totalBytes += dataEntityCount.Length;
                bytes.Enqueue(dataEntityCount);

                // Serialize each entity
                foreach (Entity entity in set._entities.Values)
                {
                    byte[] entityData = BitSerializeAttribute.Serialize(entity);
                    totalBytes += entityData.Length;
                    bytes.Enqueue(entityData);
                }
                
            }

            // Merge all byte data
            this._serializedData = new byte[totalBytes];
            int start = 0;
            while (bytes.Count > 0)
            {
                byte[] data = bytes.Dequeue();
                BitSerializeAttribute.CopyTo(ref this._serializedData, start, data);
                start += data.Length;
            }
            
        }

        /// <inheritdoc />
        public override int GetSize()
        {
            return this._serializedData.Length;
        }

        /// <inheritdoc />
        public override void Serialize(ref byte[] data, ref int lastIndex)
        {
            BitSerializeAttribute.CopyTo(ref data, lastIndex, this._serializedData);
        }

    }

}