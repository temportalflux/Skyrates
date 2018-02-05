public class EventUpdateGameState : NetworkEvent
{

    [BitSerialize(1)]
    public GameStateData serverState;

    public EventUpdateGameState()
    {
        this.serverState = new GameStateData();
    }

}