using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    // A unique identifier for this entity
    private Guid guid;

    public static Guid NewGuid()
    {
        return Guid.NewGuid();
    }

    public void Init(Guid guid)
    {
        this.guid = guid;
    }

    public void Init()
    {
        this.Init(NewGuid());
    }

    public Guid GetGuid()
    {
        return this.guid;
    }


}
