﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    public class Ship : MonoBehaviour
    {

        public Transform ComponentRoot;

        public ShipBuilder Blueprint;

        // data from ShipBuilder used to generate during Generate
        // Only valid after Generate
        public ShipData ShipData;

        // The generated object, created during Generate
        [HideInInspector]
        public ShipHull Hull;

        public void Destroy()
        {
            this.ComponentRoot.DestroyChildren();
        }

        public void Generate()
        {
            this.ShipData = this.Blueprint.ShipData;
            this.Hull = this.Blueprint.BuildTo(ref this.ComponentRoot, this.ShipData);
        }

    }

}

