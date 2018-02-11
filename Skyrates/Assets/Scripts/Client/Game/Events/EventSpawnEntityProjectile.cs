
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventSpawnEntityProjectile : GameEvent
    {
        
        public Entity.TypeData TypeData;

        public Transform Spawn;

        public Vector3 Velocity;

        public EventSpawnEntityProjectile(Entity.TypeData type, Transform spawn, Vector3 velocity) : base(GameEventID.SpawnEntityProjectile)
        {
            this.TypeData = type;
            this.Spawn = spawn;
            this.Velocity = velocity;
        }

    }

}
