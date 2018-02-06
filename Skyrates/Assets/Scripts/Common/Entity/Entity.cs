using System;
using System.Collections;
using System.Collections.Generic;
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
        private Guid _guid;

        #region Guid

        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public void Init(Guid guid)
        {
            this._guid = guid;
        }

        public void Init()
        {
            this.Init(NewGuid());
        }

        public Guid GetGuid()
        {
            return this._guid;
        }

        #endregion

        [Deprecated]
        public virtual void IntegratePhysics(PhysicsData physics)
        {
            this.transform.SetPositionAndRotation(physics.PositionLinear,
                Quaternion.Euler(physics.VelocityRotationEuler));
        }

    }

}
