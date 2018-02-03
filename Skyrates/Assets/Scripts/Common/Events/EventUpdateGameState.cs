using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUpdateGameState : NetworkEvent
{

    [BitSerialize(1)]
    public GameState.Data serverState;

    public EventUpdateGameState()
    {
        this.serverState = new GameState.Data();
    }

    public override void Execute()
    {
        if (NetworkComponent.Session.Connected)
        {
            NetworkComponent.GameState.Integrate(this.serverState, this.transmitTime);
        }
    }

}
