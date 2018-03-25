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

        private List<FormationAgent>[] _subscribedAgents = null;

        void Awake()
        {
            this.TryInitAgents();
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

        public void OnDetect(FormationAgent source, EntityAI other, float maxDistance)
        {
            foreach (List<FormationAgent> agentsInSlot in this._subscribedAgents)
            {
                foreach (FormationAgent agent in agentsInSlot)
                {
                    agent.OnDetectEntityNearFormation(source, other, maxDistance);
                }
            }
        }

    }

}
