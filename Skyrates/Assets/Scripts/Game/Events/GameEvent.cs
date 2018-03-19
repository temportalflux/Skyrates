
namespace Skyrates.Game.Event
{

    /// <summary>
    /// General event class used to send information via <see cref="GameEvents"/>.
    /// </summary>
    public class GameEvent
    {

        /// <summary>
        /// The type of event that this instance is.
        /// </summary>
        public GameEventID EventID;

        public GameEvent(GameEventID id)
        {
            this.EventID = id;
        }

    }

}
