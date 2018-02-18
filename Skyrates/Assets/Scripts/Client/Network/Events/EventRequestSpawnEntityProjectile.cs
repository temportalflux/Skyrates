using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using UnityEngine;

namespace Skyrates.Client.Network.Event
{

    /// <summary>
    /// Dispatched from client to server to spawn a projectile (fired from their <see cref="EntityPlayerShip"/>).
    /// </summary>
    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestSpawnEntityProjectile : NetworkEvent
    {

        /// <summary>
        /// The client's network ID.
        /// </summary>
        [BitSerialize(1)]
        public uint ClientID;

        /// <summary>
        /// The prefab key data for the projectile.
        /// </summary>
        [BitSerialize(2)]
        public Common.Entity.Entity.TypeData TypeData;
        
        /// <summary>
        /// The position of the spawn.
        /// </summary>
        [BitSerialize(3)]
        public Vector3 Position;

        /// <summary>
        /// The rotation of the spawn.
        /// </summary>
        [BitSerialize(4)]
        public Vector3 RotationEuler;

        /// <summary>
        /// The present velocity of the spawn location.
        /// </summary>
        [BitSerialize(5)]
        public Vector3 Velocity;

        /// <summary>
        /// The initial velocity of the projectile.
        /// </summary>
        [BitSerialize(6)]
        public Vector3 ImpluseForce;

        /// <summary>
        /// The rotation of the spawn, as a quaternion.
        /// </summary>
        public Quaternion Rotation
        {
            get { return Quaternion.Euler(this.RotationEuler); }
            set { this.RotationEuler = value.eulerAngles; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventRequestSpawnEntityProjectile() : base(NetworkEventID.RequestSpawnEntityProjectile)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeData"></param>
        /// <param name="spawn"></param>
        /// <param name="velocity"></param>
        /// <param name="impluseForce"></param>
        public EventRequestSpawnEntityProjectile(Common.Entity.Entity.TypeData typeData, Transform spawn, Vector3 velocity, Vector3 impluseForce) : this()
        {
            this.ClientID = NetworkComponent.GetSession.ClientID;
            this.TypeData = typeData;
            this.Position = spawn.position;
            this.Rotation = spawn.rotation;
            this.Velocity = velocity;
            this.ImpluseForce = impluseForce;
        }

    }

}

