﻿using Skyrates.Client.Entity;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;

namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// A special case of causing damage to a ship via another ship's ram.
    /// </summary>
    public class EventEntityShipHitByRam : EventEntityShipDamaged
    {

        /// <summary>
        /// The ramming instrument used to cause damage.
        /// </summary>
        public ShipFigurehead Figurehead;

        public EventEntityShipHitByRam(EntityShip ship, ShipFigurehead ram, float damage)
            : base(GameEventID.EntityShipHitByRam, ram.Ship, ship, damage)
        {
            this.Figurehead = ram;
        }


    }

}