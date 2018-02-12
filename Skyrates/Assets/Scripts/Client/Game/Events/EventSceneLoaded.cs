
namespace Skyrates.Client.Game.Event
{

    public class EventSceneLoaded : GameEvent
    {

        public SceneData.SceneKey Scene;

        public EventSceneLoaded(SceneData.SceneKey key) : base(GameEventID.SceneLoaded)
        {
            this.Scene = key;
        }

    }

}
