using UnityEngine;

namespace Skyrates.Ship
{

    public class ShipHullArmor : ShipComponent
    {

        [Tooltip("The base amount of damage subtracted from damage taken")]
        public float Defense;

        [Tooltip("The percentage of damage subtracted from damage taken")]
        public float Protection;

        /// <summary>
        /// Gets the amount of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The amount of damage subtracted from damage taken</returns>
        public float GetDefense()
        {
            return this.Defense;
        }

        /// <summary>
        /// Gets the percentage of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The percentage of damage subtracted from damage taken</returns>
        public float GetProtection()
        {
            return this.Protection;
        }

    }

}
