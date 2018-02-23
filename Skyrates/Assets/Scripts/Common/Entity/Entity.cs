using System;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Common.Entity
{
    /// <summary>
    /// Any entity that in the world. Used predominantly for its ability to be syncced via networking.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        
        protected virtual void Start()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityStart, this));
        }

        protected virtual void OnDestroy()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityDestroy, this));
        }

        protected virtual void Update()
        {
        }
        
    }

}
