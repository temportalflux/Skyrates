using System;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class Entity : MonoBehaviour
    {

        #region Static
    
        public enum EntityType
        {
            Static,
            Dynamic,
            Player,
        }

        public static readonly EntityType[] TYPES = new[] { EntityType.Static, EntityType.Dynamic, EntityType.Player };

        #endregion

        /// <summary>
        /// A unique identifier for this entity
        /// </summary>
        [BitSerialize(0)]
        public Guid Guid;
        
        #region Guid

        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public void Init(Guid guid)
        {
            this.Guid = guid;
        }

        public void Init()
        {
            this.Init(NewGuid());
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public virtual void IntegratePhysics()
        {
        }

    }

}
