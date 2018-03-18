using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Assumes the field is a double array.
/// Takes the keys to the array (as enum).
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DictionaryArrayAttribute : Attribute
{

    public Func<Enum[]> Keys;

    public DictionaryArrayAttribute(Func<Enum[]> getKeys)
    {
        this.Keys = getKeys;
    }

}
