using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class EntityOwner : EntityTracker, ISerializing
    {

        /// <summary>
        /// The amount of bytes it will take to serialize this.
        /// Calculated in <see cref="CalculateSerialization"/>
        /// </summary>
        private int byteCount;

        //private Dictionary<Entity.EntityType, Entity[]> entityData = 

        public void PreSerialize()
        {
            this.CalculateSerialization();
        }

        private void CalculateSerialization()
        {
            this.byteCount = 0;
        }

        /// 
        public int GetSize()
        {
            return this.byteCount;
        }

        /// 
        public void Serialize(ref byte[] data, ref int lastIndex)
        {

        }

        /// 
        public void Deserialize(byte[] data, ref int lastIndex)
        {

        }

    }

}