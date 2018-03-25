using System.Collections.Generic;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.AI.Formation
{

    /// <summary>
    /// An object which has references to potential slots which can be filled by AI agents.
    /// </summary>
    public class FormationOwner : MonoBehaviour
    {

        /// <summary>
        /// All slots that AI could potentially occupy.
        /// </summary>
        public Transform[] Slots;

        private Vector3 SlotAveragePositionOffset;

        private List<FormationAgent>[] _subscribedAgents = null;

        public float NearbyRange;

        private List<PhysicsData> NearbyTargets;

        void Awake()
        {
            this.NearbyTargets = new List<PhysicsData>();
            this.TryInitAgents();
            this.SlotAveragePositionOffset = this.CalculateAveragePositionOffset();
        }

        private void TryInitAgents()
        {
            if (this._subscribedAgents != null) return;
            this._subscribedAgents = new List<FormationAgent>[this.Slots.Length];
            for (int i = 0; i < this.Slots.Length; i++)
            {
                this._subscribedAgents[i] = new List<FormationAgent>();
            }
        }

        private Vector3 CalculateAveragePositionOffset()
        {
            int count = 1;
            Vector3 average = this.transform.position;

            foreach (Transform slot in this.Slots)
            {
                if (slot == null) continue;

                // TODO: Account for this.tranform being rotated
                // cannot take dif of rotations, as the slot may intentionally be rotated
                // take inverse of rotation? just need to counteract the main transform quaternion
                average += slot.position;

                count++;
            }

            average /= count;
            average -= this.transform.position;

            return average;
        }

        /// <summary>
        /// Returns the <see cref="PhysicsData"/> for the slot specified.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public PhysicsData GetTarget(int slot)
        {
            if (slot >= 0 && slot < this.Slots.Length)
            {
                return PhysicsData.From(this.Slots[slot]);
            }

            return null;
        }

        public void Subscribe(FormationAgent agent)
        {
            this.TryInitAgents();
            this._subscribedAgents[agent.FormationSlot].Add(agent);
        }

        public void Unsubscribe(FormationAgent agent)
        {
            this._subscribedAgents[agent.FormationSlot].Remove(agent);
        }

        public bool ContainsNearby(PhysicsData physics)
        {
            return this.NearbyTargets.Exists(data => ReferenceEquals(data, physics));
        }

        public void OnDetect(FormationAgent source, EntityAI other, float maxDistance)
        {
            if (!this.ContainsNearby(other.PhysicsData))
            {
                this.NearbyTargets.Add(other.PhysicsData);
            }
        }

        void FixedUpdate()
        {
            // TODO: put this on a timer, not to execute every physics update
            float distSq = this.NearbyRange * this.NearbyRange;
            this.NearbyTargets.RemoveAll(data =>
                (data.LinearPosition - this.transform.position).sqrMagnitude > distSq);
        }

        public List<PhysicsData> GetNearbyTargets()
        {
            return this.NearbyTargets;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.SlotAveragePositionOffset + this.transform.position, this.NearbyRange);
        }
#endif

    }

}
