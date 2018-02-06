using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.Entity
{

    public class Entity : MonoBehaviour
    {

        public enum EntityType
        {
            Static,
            Dynamic,
            Player,
        }

        public static readonly EntityType[] TYPES = new[] {EntityType.Static, EntityType.Dynamic, EntityType.Player};

        // A unique identifier for this entity
        private Guid guid;

        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public void Init(Guid guid)
        {
            this.guid = guid;
        }

        public void Init()
        {
            this.Init(NewGuid());
        }

        public Guid GetGuid()
        {
            return this.guid;
        }

        public virtual void IntegratePhysics(PhysicsData physics)
        {
            this.transform.SetPositionAndRotation(physics.PositionLinear,
                Quaternion.Euler(physics.VelocityRotationEuler));
        }

    }

}
