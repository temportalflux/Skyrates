using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static partial class ExtensionMethods
{

    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static Vector3 Flatten(this Vector3 vector, Vector3 axis)
    {
        return vector - Vector3.Project(vector, axis);
    }

    /// <summary>
    /// Integrates a Vector3 by another Vector3 over time.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="amount"></param>
    /// <param name="deltaTime"></param>
    public static void Integrate(ref Vector3 start, Vector3 amount, float deltaTime)
    {
        start += amount * deltaTime;
    }

    // TODO: Move me!!
    public static void Integrate(ref Quaternion start, Vector3 amount, float deltaTime)
    {
        Vector3 euler = start.eulerAngles;
        ExtensionMethods.Integrate(ref euler, amount, deltaTime);
        start = Quaternion.Euler(euler);
    }

    /// <summary>
    /// Integrates a Quaternion by another Quaternion over time.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="amount"></param>
    /// <param name="deltaTime"></param>
    // TODO: Move me!!
    public static void Integrate(ref Quaternion start, Quaternion amount, float deltaTime)
    {
        ExtensionMethods.Integrate(ref start, amount.eulerAngles, deltaTime);
    }

    // TODO: Move me!!
    public static Transform DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            #if UNITY_EDITOR
                GameObject.DestroyImmediate(child.gameObject);
            #else
                GameObject.Destroy(child.gameObject);
            #endif
        }
        return transform;
    }

    // TODO: Move me!!
    public static string ToStringLong<T, U>(this IDictionary<T, U> dictionary)
    {
        string str = "{";

        foreach (KeyValuePair<T, U> pair in dictionary)
        {
            str += string.Format("{{{0}:{1}}}", pair.Key.ToString(), pair.Value.ToString());
        }

        return str + "}";
    }

    // TODO: Move me!!
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) >> layer == 1;
    }

    // TODO: Move me!!
    public static bool Contains(this LayerMask mask, GameObject obj)
    {
        return mask.Contains(obj.layer);
    }

}
