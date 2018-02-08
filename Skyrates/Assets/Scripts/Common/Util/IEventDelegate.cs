using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventDelegate<in TEnumId, out TDelegateClass>
{

    TDelegateClass Delegate(TEnumId eventID);

}
