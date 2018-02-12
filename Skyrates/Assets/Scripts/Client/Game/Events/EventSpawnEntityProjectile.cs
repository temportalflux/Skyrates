using UnityEngine;

namespace Skyrates.Client.Game.Event
{

    public class EventSpawnEntityProjectile : GameEvent
    {
        
        public Common.Entity.Entity.TypeData TypeData;

        public Transform Spawn;

        public Vector3 Velocity;

        public Vector3 ImpluseForce;

        public EventSpawnEntityProjectile(Common.Entity.Entity.TypeData type, Transform spawn, Vector3 velocity, Vector3 impluseForce) : base(GameEventID.SpawnEntityProjectile)
        {
            this.TypeData = type;
            this.Spawn = spawn;
            this.Velocity = velocity;
            this.ImpluseForce = impluseForce;
        }

    }

}
