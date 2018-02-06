using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SideOnlyAttribute : Attribute
    {

        private Side _side;

        public SideOnlyAttribute(Side side)
        {
            this._side = side;
        }

    }
}
