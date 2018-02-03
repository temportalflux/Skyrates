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

}
