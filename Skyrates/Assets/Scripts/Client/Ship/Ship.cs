using Skyrates.Client.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client.Ship
{

    /// <inheritdoc />
    /// <summary>
    /// The object which contails all <see cref="ShipComponent" />s for a specific <see cref="ShipBuilder" /> rig.
    /// </summary>
    public class Ship : MonoBehaviour
    {

        /// <summary>
        /// The root to which all components are set as children of.
        /// </summary>
        public Transform ComponentRoot;

        /// <summary>
        /// The blueprint for this ship. Indicates which components create the ship.
        /// </summary>
        public ShipBuilder Blueprint;

        /// <summary>
        /// Data from <see cref="ShipBuilder"/> used to <see cref="Generate"/> the ship. Only valid after <see cref="Generate"/> is called.
        /// </summary>
        [BitSerialize(0)]
        [HideInInspector]
        public ShipData ShipData;

        // The generated object, created during Generate
        [HideInInspector]
        public ShipHull Hull;

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
        public ShipData Generate(EntityShip owner, ShipData data = null)
        {
            if (data == null) data = this.Blueprint.ShipData;
            this.ShipData = data;
            this.Hull = this.Blueprint.BuildTo(owner, ref this.ComponentRoot, this.ShipData);
            this.ShipData.MustBeRebuilt = false;
            return this.ShipData;
        }

    }

}

