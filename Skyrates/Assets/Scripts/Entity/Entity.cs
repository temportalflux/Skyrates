using Skyrates.Game;
using Skyrates.Game.Event;
using UnityEngine;

namespace Skyrates.Entity
{
    /// <summary>
    /// Any entity that in the world. Used predominantly for its ability to be syncced via networking.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        
        /// <summary>
        /// Called during the first frame
        /// </summary>
        protected virtual void Start()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityStart, this));
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        {
            GameManager.Events.Dispatch(new EventEntity(GameEventID.EntityDestroy, this));
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        protected virtual void Update()
        {
        }
        
    }

}
