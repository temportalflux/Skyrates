using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SideOnlyAttribute : Attribute
    {

        public Side Side { get; private set; }

        public SideOnlyAttribute(Side side)
        {
            this.Side = side;
        }

    }
}
