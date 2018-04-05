using System.Linq;
using UnityEngine;

namespace Skyrates.Respawn
{

    public class RespawnAreaList : Singleton<RespawnAreaList>
    {

        public static RespawnAreaList Instance;

        public RespawnArea[] Checkpoints;

        void Awake()
        {
            this.loadSingleton(this, ref Instance, false);
        }

        public RespawnArea GetCheckpoint(uint index)
        {
            return index < this.Checkpoints.Length ? this.Checkpoints[index] : null;
        }

        public RespawnArea GetRandomCheckpoint()
        {
            return this.GetCheckpoint((uint)Random.Range(0, this.Checkpoints.Length));
        }

        public RespawnArea GetClosestCheckpoint(Vector3 position)
        {
            return this.Checkpoints.OrderBy(area => (area.transform.position - position).sqrMagnitude).First();
        }

    }

}
