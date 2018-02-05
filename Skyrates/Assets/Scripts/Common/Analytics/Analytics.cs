
//#define FORCE_ANAYLTICS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analytics : Singleton<Analytics>
{

    public static Analytics Instance;

    public Version Version;

    private Guid _sessionID;
    public static Guid SessionID { get { return Instance._sessionID; } }

    void Awake()
    {
        this.loadSingleton(this, ref Instance);
        this._sessionID = Guid.NewGuid();
        new EventSessionStart().Dispatch();
    }

    void OnDestroy()
    {
        new EventSessionEnd().Dispatch();
    }

    public static void Dispatch(EnumAnalyticEvent name, IDictionary<string, object> data)
    {
#if !UNITY_EDITOR || FORCE_ANAYLTICS
        AnalyticsResult result = UnityEngine.Analytics.Analytics.CustomEvent(name.ToString(), data);
        Debug.Log("Sent " + name + ": " + result + ". " + data.ToStringLong());
#endif
    }

}
