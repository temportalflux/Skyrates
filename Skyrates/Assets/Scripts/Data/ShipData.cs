using System;
using Skyrates.Util.Serializing;
using UnityEngine;

namespace Skyrates.Ship
{
    /// <summary>
    /// Data object used to send ship rig data over the network (and for tool usage in editor).
    /// Stores references to components as integers for lookup in <see cref="ShipComponentList"/>.
    /// </summary>
    [Serializable]
    public class ShipData : ISerializing
    {

        public enum ComponentType
        {
            ArtilleryLeft, ArtilleryRight, ArtilleryForward,
            Figurehead, Hull,
            NavigationLeft, NavigationRight,
            Propulsion, Sail,
        }

        public static readonly object[] ComponentTypes =
        {
            ComponentType.ArtilleryLeft,
            ComponentType.ArtilleryRight,
            ComponentType.ArtilleryForward,
            ComponentType.Figurehead,
            ComponentType.Hull,
            ComponentType.NavigationLeft,
            ComponentType.NavigationRight,
            ComponentType.Propulsion,
            ComponentType.Sail,
        };
        public static readonly ComponentType[] NonHullComponents = {
            ComponentType.ArtilleryLeft,
            ComponentType.ArtilleryRight,
            ComponentType.ArtilleryForward,
            ComponentType.Figurehead,
            ComponentType.NavigationLeft,
            ComponentType.NavigationRight,
            ComponentType.Propulsion,
            ComponentType.Sail,
        };
        public static readonly int[] HulllessComponentIndex = { 0, 1, 2, 3, -1, 4, 5, 6, 7 };

        public static readonly Type[] ComponentClassTypes =
        {
            typeof(ShipArtillery),
            typeof(ShipArtillery),
            typeof(ShipArtillery),
            typeof(ShipFigurehead),
            typeof(ShipHull),
            typeof(ShipNavigationLeft),
            typeof(ShipNavigationRight),
            typeof(ShipPropulsion),
            typeof(ShipSail),
        };

        public enum BrokenComponentType
        {
            Invalid = -1,
            Artillery,
            Figurehead,
            Hull, //Hull Armor
            Navigation,
            Propulsion,
        }

        public static readonly BrokenComponentType[] BrokenComponentTypes =
        {
            BrokenComponentType.Artillery,
            BrokenComponentType.Figurehead,
            BrokenComponentType.Hull,
            BrokenComponentType.Navigation,
            BrokenComponentType.Propulsion,
        };

        //TODO: Refactor to dictionary or reflection if we ever have to change these often.
        public static readonly BrokenComponentType[] ComponentTypeToBrokenComponentType = { BrokenComponentType.Artillery, BrokenComponentType.Artillery, BrokenComponentType.Artillery, BrokenComponentType.Figurehead, BrokenComponentType.Hull, BrokenComponentType.Navigation, BrokenComponentType.Navigation, BrokenComponentType.Propulsion, BrokenComponentType.Invalid };

        [SerializeField]
        public int[] ComponentTiers = new int[ComponentTypes.Length];

        [SerializeField]
        private bool _hasNewData = false;

        public bool MustBeRebuilt
        {
            get { return this._hasNewData; }
            set { this._hasNewData = value; }
        }

        public int this[ComponentType key]
        {
            get { return this.ComponentTiers[(int)key]; }
            set { this.ComponentTiers[(int)key] = value; }
        }

        /// <summary>
        /// Return the ship component for the selected category.
        /// </summary>
        /// <param name="list">The <see cref="ShipComponentList"/> reference.</param>
        /// <param name="type">The type of component requested.</param>
        /// <returns></returns>
        public ShipComponent GetShipComponent(ShipComponentList list, ComponentType type)
        {
            return list.GetRawComponent(type, this[type]);
        }


        public int GetSize()
        {
            return sizeof(int) + this.ComponentTiers.Length * sizeof(int);
        }

        public void Serialize(ref byte[] data, ref int lastIndex)
        {
            data = BitSerializeAttribute.Serialize(this.ComponentTiers, data, lastIndex);
            lastIndex += this.GetSize();
        }

        public void Deserialize(byte[] data, ref int lastIndex)
        {
            int[] deserializedComponents = (int[])BitSerializeAttribute.Deserialize(this.ComponentTiers, data, ref lastIndex);
            if (deserializedComponents.Length != this.ComponentTiers.Length)
            {
                this._hasNewData = true;
            }
            else
            {
                for (int iComponent = 0; iComponent < this.ComponentTiers.Length; iComponent++)
                {
                    if (this.ComponentTiers[iComponent] != deserializedComponents[iComponent])
                    {
                        this._hasNewData = true;
                        break;
                    }
                }
            }

            if (this._hasNewData)
            {
                this.ComponentTiers = deserializedComponents;
            }
        }

        public void Upgrade(ComponentType type)
        {
            this[type]++;
        }

    }
}