using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// \addtogroup client
/// @{

/// <summary>
/// Base class to assist with objects which only occur once in the lifetime of the game
/// </summary>
/// <typeparam name="T"></typeparam>
/// \author Dustin Yost
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    /// <summary>
    /// Loads the singleton.
    /// Checks for singleton properties, then marks the instance
    /// </summary>
    /// <param name="inst">The instance to check and set</param>
    /// <param name="staticRef">The singleton reference instance</param>
    /// \author Dustin Yost
    protected void loadSingleton(T inst, ref T staticRef)
    {

        if (staticRef != null)
        {
            Destroy(staticRef);
            staticRef = null;
        }

        staticRef = inst;
        DontDestroyOnLoad(staticRef);

    }

    // MUTEX

    /// <summary>
    /// Author: Dustin Yost
    /// Flag for if the object's information is in use
    /// </summary>
    private bool inUse = false;

    /// <summary>
    /// Author: Dustin Yost
    /// Returns if this object is currently being used
    /// </summary>
    /// <returns>
    ///   <c>true</c> if inUse is <c>true</c>; otherwise, <c>false</c>.
    /// </returns>
    public bool isInUse()
    {
        return inUse;
    }

    /// <summary>
    /// Author: Dustin Yost
    /// Locks this instance.
    /// Flags this object as being used
    /// </summary>
    public void Lock() { inUse = true; }

    /// <summary>
    /// Unlocks this instance.
    /// Flags this object as not being used
    /// </summary>
    public void Unlock() { inUse = false; }

}
/// @}
