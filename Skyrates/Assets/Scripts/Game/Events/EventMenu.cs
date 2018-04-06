namespace Skyrates.Game.Event
{

    public class EventMenu : GameEvent
    {

        public enum CanvasType
        {
            None,
            Hud,
            Upgrades,
            Pause,
        }

        public CanvasType Type;

        public bool ShouldOpen
        {
            get { return this.EventID == GameEventID.MenuOpen; }
        }

        private EventMenu(GameEventID id, CanvasType canvasKey) : base(id)
        {
            this.Type = canvasKey;
        }

        public static EventMenu Open(CanvasType canvasKey)
        {
            return new EventMenu(GameEventID.MenuOpen, canvasKey);
        }

        public static EventMenu Close(CanvasType canvasKey)
        {
            return new EventMenu(GameEventID.MenuClose, canvasKey);
        }

    }

}
