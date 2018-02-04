using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDateTime : AnalyticEvent
{

    public EventDateTime(EnumAnalyticEvent id) : base(id)
    {
        this[EnumAnalyticParam.DateTime] = System.DateTime.Now;
    }

}
