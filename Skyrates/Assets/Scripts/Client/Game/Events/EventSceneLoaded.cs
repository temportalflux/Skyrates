
namespace Skyrates.Client.Game.Event
{

    /// <summary>
    /// Dispatched when a scene has finished loading via <see cref="SceneLoader"/>.
    /// </summary>
    public class EventSceneLoaded : GameEvent
    {

        public SceneData.SceneKey Scene;

        public EventSceneLoaded(SceneData.SceneKey key) : base(GameEventID.SceneLoaded)
        {
            this.Scene = key;
        }

    }

}
