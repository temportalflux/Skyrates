using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using Skyrates.Common.Network.Event;
using UnityEngine;

namespace Skyrates.Client.Network.Event
{

    [NetworkEvent(Side.Client, Side.Server)]
    public class EventRequestSpawnEntityProjectile : NetworkEvent
    {

        [BitSerialize(1)]
        public uint clientID;

        [BitSerialize(2)]
        public Common.Entity.Entity.TypeData TypeData;

        [BitSerialize(3)]
        public Guid EntityGuid;

        [BitSerialize(4)]
        public Vector3 Position;

        [BitSerialize(5)]
        public Vector3 RotationEuler;

        [BitSerialize(6)]
        public Vector3 Velocity;

        [BitSerialize(7)]
        public Vector3 ImpluseForce;

        public Quaternion Rotation
        {
            get { return Quaternion.Euler(this.RotationEuler); }
            set { this.RotationEuler = value.eulerAngles; }
        }

        public EventRequestSpawnEntityProjectile() : base(NetworkEventID.RequestSpawnEntityProjectile)
        {
        }

        public EventRequestSpawnEntityProjectile(Common.Entity.Entity.TypeData typeData, Transform spawn, Vector3 velocity, Vector3 impluseForce) : this()
        {
            this.clientID = NetworkComponent.GetSession.ClientID;
            this.TypeData = typeData;
            this.Position = spawn.position;
            this.Rotation = spawn.rotation;
            this.Velocity = velocity;
            this.ImpluseForce = impluseForce;
        }

    }

}

