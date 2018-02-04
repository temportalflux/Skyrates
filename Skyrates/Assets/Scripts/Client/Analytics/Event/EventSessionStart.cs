using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSessionStart : EventDateTime
{

    public EventSessionStart() : base(EnumAnalyticEvent.SessionStart)
    {
        this[EnumAnalyticParam.Version] = Analytics.Instance.Version.getSemantic();
    }

}
