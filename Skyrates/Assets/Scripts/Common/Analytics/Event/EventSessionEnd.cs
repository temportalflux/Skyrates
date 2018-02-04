using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSessionEnd : EventDateTime
{

    public EventSessionEnd() : base(EnumAnalyticEvent.SessionEnd)
    {
    }

}
