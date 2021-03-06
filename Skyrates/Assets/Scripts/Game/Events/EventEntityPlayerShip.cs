﻿using Skyrates.Entity;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// Generic event in which an entity in the event is a player ship.
    /// </summary>
    public class EventEntityPlayerShip : EventEntityShip
    {

        /// <summary>
        /// The entity player ship.
        /// </summary>
        public EntityPlayerShip PlayerShip
        {
            get { return this.Entity as EntityPlayerShip; }
        }

        public EventEntityPlayerShip(GameEventID id, EntityPlayerShip playerShip) : base(id, playerShip)
        {
        }

    }

}
