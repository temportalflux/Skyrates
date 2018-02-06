using System;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class Entity : MonoBehaviour
    {

        #region Static

        public enum Type
        {
            // LISTABLES MUST COME FIRST
            Static,
            Dynamic,

            // Then others...
            Player,
        }

        public static readonly Type[] AllTypes = { Type.Static, Type.Dynamic, Type.Player };
        public static readonly object[] ListableTypes = { Type.Static, Type.Dynamic };
        public static readonly System.Type[] ListableClassTypes = { typeof(Entity), typeof(EntityDynamic) };
        
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

        #region Network

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity's data was successfully deserialized into it.
        /// </summary>
        public void OnDeserializeSuccess()
        {

        }

        /// <summary>
        /// Called by <see cref="EntityReceiver.Deserialize"/> when the entity was not found in the server state and thus should be removed.
        /// </summary>
        public void OnDeserializeFail()
        {
            Destroy(this.gameObject);
        }

        #endregion
        
    }

}
