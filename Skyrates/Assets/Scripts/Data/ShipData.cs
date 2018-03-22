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
            ArtilleryLeft, ArtilleryRight,
            ArtilleryForward, ArtilleryDown,
            Figurehead,
            HullArmor,
            NavigationLeft, NavigationRight,
            Propulsion,
        }

        public static readonly object[] ComponentTypes =
        {
            ComponentType.ArtilleryLeft, ComponentType.ArtilleryRight,
            ComponentType.ArtilleryForward, ComponentType.ArtilleryDown,
            ComponentType.Figurehead,
            ComponentType.HullArmor,
            ComponentType.NavigationLeft, ComponentType.NavigationRight,
            ComponentType.Propulsion,
        };

        public static readonly Type[] ComponentClassTypes =
        {
            typeof(ShipArtillery), typeof(ShipArtillery),
            typeof(ShipArtillery), typeof(ShipArtillery),
            typeof(ShipFigurehead),
            typeof(ShipHullArmor),
            typeof(ShipNavigationLeft), typeof(ShipNavigationRight),
            typeof(ShipPropulsion),
        };

        public enum BrokenComponentType
        {
            Invalid = -1,
            Artillery,
            Figurehead,
            HullArmor,
            Navigation,
            Propulsion,
        }

        public static readonly BrokenComponentType[] BrokenComponentTypes =
        {
            BrokenComponentType.Artillery,
            BrokenComponentType.Figurehead,
            BrokenComponentType.HullArmor,
            BrokenComponentType.Navigation,
            BrokenComponentType.Propulsion,
        };

        //TODO: Refactor to dictionary or reflection if we ever have to change these often.
        public static readonly BrokenComponentType[] ComponentTypeToBrokenComponentType =
        {
            BrokenComponentType.Artillery, BrokenComponentType.Artillery,
            BrokenComponentType.Artillery, BrokenComponentType.Artillery,
            BrokenComponentType.Figurehead,
            BrokenComponentType.HullArmor,
            BrokenComponentType.Navigation, BrokenComponentType.Navigation,
            BrokenComponentType.Propulsion,
            BrokenComponentType.Invalid
        };
        
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