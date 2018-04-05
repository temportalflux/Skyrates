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
        public FormationSlot[] Slots;

        [SerializeField]
        private PhysicsData[] SlotOffsets;

        private Vector3 SlotAveragePositionOffset;

        private List<FormationAgent>[] _subscribedAgents = null;

        [SerializeField]
        public float NearbyRange;

        [SerializeField]
        public float ThreatRange;

#if UNITY_EDITOR
        public Color GizmoColorNearby = Colors.White;
        public Color GizmoColorNearbyTargets = Colors.AshGray;
        public Color GizmoColorThreat = Colors.Red;
        public Color GizmoColorSlots = Colors.OgreOdor;
#endif

        private List<NearbyTarget> NearbyTargets;
        private List<NearbyTarget> NearbyThreats;

        void Awake()
        {
            this.SlotOffsets = new PhysicsData[this.Slots.Length];
            for (int i = 0; i < this.Slots.Length; i++)
            {
                this.SlotOffsets[i] = PhysicsData.From(this.Slots[i].transform);
                this.SlotOffsets[i].LinearPosition -= this.transform.position;
                this.SlotOffsets[i].LinearPosition = Quaternion.Inverse(this.transform.rotation) * this.SlotOffsets[i].LinearPosition;
                this.SlotOffsets[i].RotationPosition *= Quaternion.Inverse(this.transform.rotation);
            }

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

            foreach (PhysicsData slot in this.SlotOffsets)
            {
                // TODO: Account for this.tranform being rotated
                // cannot take dif of rotations, as the slot may intentionally be rotated
                // take inverse of rotation? just need to counteract the main transform quaternion
                average += slot.LinearPosition;

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
                if (this.SlotOffsets[slot] == null) return null;
                PhysicsData slotOffset = this.SlotOffsets[slot].Copy();
                //slotOffset.LinearPosition = this.transform.rotation * slotOffset.LinearPosition + this.transform.position;
                slotOffset.LinearPosition = this.transform.rotation * slotOffset.LinearPosition;
                slotOffset.LinearPosition += this.transform.position;
                slotOffset.RotationPosition *= this.transform.rotation;
                return slotOffset;
            }

            return null;
        }

        public void Subscribe(FormationAgent agent)
        {
            this.TryInitAgents();
            if (agent.FormationSlot < this._subscribedAgents.Length)
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
        void OnDrawGizmosSelected()
        {
            Gizmos.color = this.GizmoColorNearby;
            Gizmos.DrawWireSphere(this.SlotAveragePositionOffset + this.transform.position, this.NearbyRange);

            Gizmos.color = this.GizmoColorThreat;
            Gizmos.DrawWireSphere(this.SlotAveragePositionOffset + this.transform.position, this.ThreatRange);

            if (this.NearbyTargets != null)
            {
                foreach (NearbyTarget target in this.NearbyTargets)
                {
                    target.Target.DrawGizmos(1.0f, 0.5f, this.GizmoColorNearbyTargets);
                }
            }

            if (Application.isPlaying)
            {
                for (int i = 0; i < this.Slots.Length; i++)
                {
                    PhysicsData data = this.GetTarget(i);
                    Gizmos.color = this.GizmoColorThreat;
                    Gizmos.DrawWireSphere(data.LinearPosition, 1.0f);
                    Gizmos.DrawLine(data.LinearPosition, data.LinearPosition + data.RotationPosition * Vector3.forward * 5);
                }
            }

        }
#endif

    }

}
