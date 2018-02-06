using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityReceiver : EntityTracker
    {

        private int _totalBytes;

        public override int GetSize()
        {
            return this._totalBytes;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Paired with <see cref="EntityOwner.GenerateData"/>.
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
            int entityTypes = (int)BitSerializeAttribute.Deserialize(0, data, ref lastIndex);

            // Look through entity set data:
            for (int iType = 0; iType < entityTypes; iType++)
            {
                Entity.EntityType type = Entity.TYPES[iType];

                // BEFORE:
                // Make a list of all GUIDs
                List<Guid> dirtyEntities = new List<Guid>(this._entities[type]._entities.Keys);

                int entityCount = (int) BitSerializeAttribute.Deserialize(0, data, ref lastIndex);

                for (int iEntity = 0; iEntity < entityCount; iEntity++)
                {
                    // Peak at the GUID (don't incrememnt index counter)
                    // ASSUME: Guid is the first thing serialized by Entities
                    Guid entityGuid = (Guid) BitSerializeAttribute.Deserialize(Guid.Empty, data, lastIndex);
                    // NOT CHANGING LAST INDEX (entities need the data of their entity guid)

                    // if that entity is already being tracked
                    if (this._entities[type].ContainsKey(entityGuid))
                    {
                        // Remove a guid from dirtyEntities when it is encountered
                        dirtyEntities.Remove(entityGuid);

                        // deserialize into that entity (and then call integrate)
                        Entity entity = this._entities[type]._entities[entityGuid];
                        entity = (Entity)BitSerializeAttribute.Deserialize(entity, data, ref lastIndex);
                        this._entities[type]._entities[entityGuid] = entity;
                        entity.OnDeserializeSuccess();

                    }
                    // else
                    else
                    {
                        // entity should be added locally
                        this.SpawnEntity(type, entityGuid);
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
