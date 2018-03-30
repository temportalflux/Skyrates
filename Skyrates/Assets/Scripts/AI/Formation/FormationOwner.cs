using System.Collections.Generic;
using Skyrates.Entity;
using Skyrates.Physics;
using UnityEngine;

using NearbyTarget = Skyrates.AI.Behavior.DataBehavioral.NearbyTarget;

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
        [SerializeField]
        public Transform[] Slots;

        private Vector3 SlotAveragePositionOffset;

        private List<FormationAgent>[] _subscribedAgents = null;

        [SerializeField]
        public float NearbyRange;

        [SerializeField]
        public float ThreatRange;

        private List<NearbyTarget> NearbyTargets;
        private List<NearbyTarget> NearbyThreats;

        void Awake()
        {
            this.NearbyTargets = new List<NearbyTarget>();
            this.NearbyThreats = new List<NearbyTarget>();
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

        public bool ContainsNearbyTarget(PhysicsData physics)
        {
            return this.NearbyTargets.Exists(data => ReferenceEquals(data.Target, physics));
        }

        public bool ContainsNearbyThreat(PhysicsData physics)
        {
            return this.NearbyThreats.Exists(data => ReferenceEquals(data.Target, physics));
        }

        public void OnDetect(FormationAgent source, EntityAI other, float maxDistance)
        {
            if (!this.ContainsNearbyTarget(other.PhysicsData))
            {
                this.NearbyTargets.Add(new NearbyTarget
                {
                    Target = other.PhysicsData,
                    MaxDistanceSq = maxDistance * maxDistance,
                });
            }
        }

        public List<NearbyTarget> GetNearbyTargets()
        {
            return this.NearbyTargets;
        }

        public List<NearbyTarget> GetNearbyThreats()
        {
            return this.NearbyThreats;
        }

        public void OnDamagedBy(FormationAgent agent, EntityAI other)
        {
            if (!this.ContainsNearbyThreat(other.PhysicsData))
            {
                this.NearbyThreats.Add(new NearbyTarget
                {
                    Target = other.PhysicsData,
                    MaxDistanceSq = this.ThreatRange * this.ThreatRange,
                });
            }
        }

        void FixedUpdate()
        {
            // TODO: put this on a timer, not to execute every physics update
            Vector3 center = this.transform.position + this.SlotAveragePositionOffset;

            float distSq = this.NearbyRange * this.NearbyRange;
            this.NearbyTargets.RemoveAll(data =>
                (data.Target.LinearPosition - center).sqrMagnitude > distSq);

            distSq = this.ThreatRange * this.ThreatRange;
            this.NearbyThreats.RemoveAll(data =>
                (data.Target.LinearPosition - center).sqrMagnitude > distSq);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(this.SlotAveragePositionOffset + this.transform.position, this.NearbyRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.SlotAveragePositionOffset + this.transform.position, this.ThreatRange);

            if (this.NearbyTargets != null)
            {
                foreach (NearbyTarget target in this.NearbyTargets)
                {
                    target.Target.DrawGizmos(1.0f, 0.5f, Color.gray);
                }
            }
        }
#endif

    }

}
