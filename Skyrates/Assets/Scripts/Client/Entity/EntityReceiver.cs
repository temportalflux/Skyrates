using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:Skyrates.Common.Entity.EntityTracker" /> to handle deserializing entities.
    /// </summary>
    public class EntityReceiver : EntityTracker
    {

        /// <summary>
        /// The total number of bytes used during <see cref="Deserialize"/>.
        /// </summary>
        private int _totalBytes;

        /// <inheritdoc />
        public override int GetSize()
        {
            return this._totalBytes;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Paired with <see cref="EntityDispatcher.GenerateData"/>.
        /// </summary>
        public override void Deserialize(byte[] data, ref int lastIndex)
        {
            int startIndex = lastIndex;

            // Get data in terms of EntitySets
            // Look through entity set data:
            // - get guid of each entity (don't incrememnt index counter)
            // - if that entity is already being tracked
            // - - deserialize into that entity (and then call integrate)
            // - else
            // - - entity should be added locally

            // Get data in terms of EntitySets

            // Look through entity set data:
            foreach (Entity.Type type in Entity.AllTypes)
            {

                // BEFORE:
                // Make a list of all GUIDs
                List<Guid> dirtyEntities = new List<Guid>(this.Entities[type].Entities.Keys);

                int entityCount = (int) BitSerializeAttribute.Deserialize(0, data, ref lastIndex);

                for (int iEntity = 0; iEntity < entityCount; iEntity++)
                {
                    // Peak at the GUID (don't incrememnt index counter)
                    // ASSUME: Guid is the first thing serialized by Entities
                    int indexEntityPeak = lastIndex;
                    Guid entityGuid = (Guid) BitSerializeAttribute.Deserialize(Guid.Empty, data, ref indexEntityPeak);
                    // NOT CHANGING LAST INDEX (entities need the data of their entity guid)

                    // if that entity is already being tracked
                    if (this.Entities[type].ContainsKey(entityGuid))
                    {
                        // Remove a guid from dirtyEntities when it is encountered
                        dirtyEntities.Remove(entityGuid);

                        // deserialize into that entity (and then call integrate)
                        Entity entity = this.Entities[type].Entities[entityGuid];
                        // marker for entities like local players who are controlled locally and shouldnt deserialize info
                        if (entity.ShouldDeserialize())
                        {
                            entity = (Entity)BitSerializeAttribute.Deserialize(entity, data, ref lastIndex);
                            this.Entities[type].Entities[entityGuid] = entity;
                            entity.OnDeserializeSuccess();
                        }

                    }
                    // else
                    else
                    {
                        // entity should be added locally
                        // peak at the type of entity
                        TypeData entityTypeData =
                            (TypeData)BitSerializeAttribute.Deserialize(new TypeData(), data, ref indexEntityPeak);
                        // Spawn the entity
                        Entity entity = GameManager.Instance.SpawnEntity(entityTypeData, entityGuid,
                            isLocal: entityTypeData.OwnerNetworkID == NetworkComponent.GetSession.NetworkID);
                        if (entity != null)
                        {
                            // have the entity fully deserialize
                            entity = (Entity) BitSerializeAttribute.Deserialize(entity, data, ref lastIndex);
                            // tell them it worked
                            entity.OnDeserializeSuccess();
                        }
                    }

                }

                // The remaining dirty guids are not in the data, and should be removed
                foreach (Guid guid in dirtyEntities)
                {
                    Entity entity = this.Remove(type, guid);
                    entity.OnDeserializeFail();
                }

            }

            this._totalBytes = lastIndex - startIndex;
        }

    }

}
