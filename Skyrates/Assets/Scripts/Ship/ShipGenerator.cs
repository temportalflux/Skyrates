using System.Collections.Generic;
using Skyrates.Entity;
using Skyrates.Util.Extension;
using Skyrates.Util.Serializing;
using UnityEngine;

namespace Skyrates.Ship
{

    /// <inheritdoc />
    /// <summary>
    /// The object which contails all <see cref="ShipComponent" />s for a specific <see cref="ShipRig" /> rig.
    /// </summary>
    public class ShipGenerator : MonoBehaviour
    {

        public EntityShip Owner;

        /// <summary>
        /// The root to which all components are set as children of.
        /// </summary>
        public Transform ComponentRoot;

        /// <summary>
        /// The blueprint for this ship. Indicates which components create the ship.
        /// </summary>
        public ShipRig Blueprint;

        /// <summary>
        /// Data from <see cref="ShipRig"/> used to <see cref="Generate"/> the ship. Only valid after <see cref="Generate"/> is called.
        /// </summary>
        [BitSerialize(0)]
        [HideInInspector]
        public ShipData ShipData;

        /// <summary>
        /// Destroys all children of the <see cref="ComponentRoot"/>.
        /// </summary>
        public void Destroy()
        {
            this.ComponentRoot.DestroyChildren();
        }

        /// <summary>
        /// Generates the ship via the current <see cref="Blueprint"/>. Stores relevant data in <see cref="ShipData"/> and the actual components put under <see cref="ComponentRoot"/>.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ShipData Generate(ShipData data = null)
        {
            if (data == null) data = this.Blueprint.ShipData;
            this.Blueprint.BuildTo(this.Owner, ref this.ComponentRoot, data);
            this.ShipData.MustBeRebuilt = false;
            return data;
        }

        public void ReGenerate()
        {
            this.Destroy();
            this.Generate(this.ShipData);
        }

        public void UpgradeComponents(IEnumerable<ShipData.ComponentType> components)
        {
            foreach (ShipData.ComponentType type in components)
            {
                this.ShipData.Upgrade(type);
            }
        }

    }

}

