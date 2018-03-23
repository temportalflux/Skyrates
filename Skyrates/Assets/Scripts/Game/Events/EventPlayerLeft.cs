using System;

namespace Skyrates.Game.Event
{

    /// <summary>
    /// Dispatched when a player leaves the network interface / client disconnects.
    /// </summary>
    public class EventPlayerLeft : GameEvent
    {

        /// <summary>
        /// The <see cref="Guid"/> of the player which left.
        /// </summary>
        public Guid PlayerGuid;

        public EventPlayerLeft(Guid guid) : base(GameEventID.PlayerLeft)
        {
            this.PlayerGuid = guid;
        }

    }

}