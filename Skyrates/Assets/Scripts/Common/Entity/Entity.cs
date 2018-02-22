using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    /// <summary>
    /// Any entity that in the world. Used predominantly for its ability to be syncced via networking.
    /// </summary>
    public class Entity : MonoBehaviour
    {

        /// <summary>
        /// Data pertaining to how to find the entity in <see cref="EntityList"/>.
        /// </summary>
        [Serializable]
        public class TypeData
        {

            /// <summary>
            /// <see cref="Entity.Type"/> as a number for network serialization.
            /// </summary>
            [BitSerialize(0)]
            [SerializeField]
            public int EntityTypeAsInt;

            /// <summary>
            /// The index of the entity prefab in <see cref="EntityList"/> under the category of <see cref="EntityType"/>.
            /// </summary>
            [BitSerialize(1)]
            [SerializeField]
            public int EntityTypeIndex;

            /// <summary>
            /// The type of entity. Encapsultes <see cref="EntityTypeAsInt"/>.
            /// </summary>
            public Entity.Type EntityType
            {
                get { return (Entity.Type) this.EntityTypeAsInt; }
                set { this.EntityTypeAsInt = (int) value; }
            }

            public TypeData(Entity.Type type, int index)
            {
                this.EntityType = type;
                this.EntityTypeIndex = index;
            }

            public TypeData()
            {
            }

        }

        #region Static

        public enum Type
        {
            // LISTABLES MUST COME FIRST
            Static,
            Dynamic,
            Projectile,

            // Then others...
            Player,
        }

        public static readonly Type[] AllTypes = { Type.Static, Type.Dynamic, Type.Projectile, Type.Player };
        public static readonly string[] AllTypesString = { Type.Static.ToString(), Type.Dynamic.ToString(), Type.Projectile.ToString(), Type.Player.ToString() };
        public static readonly object[] ListableTypes = { Type.Static, Type.Dynamic, Type.Projectile };
        public static readonly System.Type[] ListableClassTypes = { typeof(Entity), typeof(EntityDynamic), typeof(EntityProjectile) };

        #endregion
        
        [SerializeField]
        public TypeData EntityType;
        
        protected virtual void Start()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityStart, this));
        }

        protected virtual void OnDestroy()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityDestroy, this));
        }

        protected virtual void Update()
        {
        }

        /// <summary>
        /// Initializes the entity with a specific <see cref="TypeData"/>.
        /// VERY important for syncing entities over the network.
        /// </summary>
        /// <param name="typeData"></param>
        public void Init(TypeData typeData)
        {
            this.EntityType = typeData;
        }

    }

}
