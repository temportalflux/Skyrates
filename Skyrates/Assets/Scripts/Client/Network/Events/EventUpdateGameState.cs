using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUpdateGameState : NetworkEvent
{

    [BitSerialize(1)]
    public GameStateData serverState;

    public EventUpdateGameState()
    {
        this.serverState = new GameStateData();
    }

}
