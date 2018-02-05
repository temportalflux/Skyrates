using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumAnalyticEvent
{
    SessionStart,
    SessionEnd,

}

public enum EnumAnalyticParam
{
    SessionID, // Guid
    DateTime, // System.DateTime
    Version, // String: #.#.# (Version.GetSemantic)

}
