﻿using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:Skyrates.Common.Entity.EntityTracker" />,
    /// which is able to serialize entities for dispatching to clients.
    /// </summary>
    public class EntityDispatcher : EntityTracker
    {

        /// <summary>
        /// Total number of bytes used during <see cref="GenerateData"/>.
        /// </summary>
        private int _totalBytes;

        /// <summary>
        /// The data generated by <see cref="GenerateData"/>.
        /// </summary>
        private Queue<byte[]> _serializedData;

        /// <inheritdoc />
        /// <summary>
        /// Paired with <see cref="EntityReceiver.Deserialize"/>.
        /// </summary>
        public override void GenerateData()
        {
            // Clear any previous data
            // Cache to hold all bytes to be combined later
            this._serializedData = new Queue<byte[]>();

            // Cache for how many bytes are used.
            this._totalBytes = 0;

            foreach (Entity.Type type in Entity.AllTypes)
            {
                EntitySet set = this.Entities[type];

                // Serialize the number of entities
                byte[] dataEntityCount = BitSerializeAttribute.Serialize(set.Entities.Count);
                this._totalBytes += dataEntityCount.Length;
                this._serializedData.Enqueue(dataEntityCount);

                // Serialize each entity
                foreach (Entity entity in set.Entities.Values)
                {
                    byte[] entityData = BitSerializeAttribute.Serialize(entity);
                    this._totalBytes += entityData.Length;
                    this._serializedData.Enqueue(entityData);
                }
                
            }
            
        }

        /// <inheritdoc />
        public override int GetSize()
        {
            return this._totalBytes;
        }

        /// <inheritdoc />
        public override void Serialize(ref byte[] data, ref int lastIndex)
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

            // TODO: Remove this tmp shit fro mnetwork testing branh
            lastIndex = lastIndex;

        }


        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
            GameManager.Events.EntityStart += this.OnEntityStart;
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            GameManager.Events.EntityStart -= this.OnEntityStart;
        }

        // NOTE: Only fired on server side (dispatcher)
        void OnEntityStart(GameEvent evt)
        {
            this.OnEntityStart(((EventEntity) evt).Entity);
        }

    }

}