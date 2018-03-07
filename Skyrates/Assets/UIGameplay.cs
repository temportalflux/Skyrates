using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

public class UIGameplay : MonoBehaviour
{

    public UIFillCutoffBar ActiveReloadStarboard;
    public UIFillCutoffBar ActiveReloadPort;

    void OnEnable()
    {
        GameManager.Events.ActiveReloadBegin += this.OnActiveReloadBegin;
    }

    void OnDisable()
    {
        GameManager.Events.ActiveReloadBegin -= this.OnActiveReloadBegin;
    }

    void OnActiveReloadBegin(GameEvent evt)
    {
        EventActiveReloadBegin evtReload = (EventActiveReloadBegin) evt;
        UIFillCutoffBar reloadBar = evtReload.IsStarboard ? this.ActiveReloadStarboard : this.ActiveReloadPort;
        reloadBar.Execute(evtReload.ActiveReloadStart, evtReload.ActiveReloadEnd, evtReload.GetPercentUpdate);
    }

}
