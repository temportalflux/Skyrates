using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

[CreateAssetMenu(menuName = "Stats/Ship Component List")]
public class ShipComponentList : ScriptableObject
{

    [Serializable]
    public struct ComponentList
    {
        public ShipComponent[] components;
    }

    /// <summary>
    /// An array keyed by <see cref="ComponentType"/> where values are arrays of ShipComponents.
    /// </summary>
    public ComponentList[] components = new ComponentList[ShipData.ComponentTypes.Length];

    public bool[] editor_showComponentArray;

    /// <summary>
    /// The list of names for an array of components keyed by <see cref="ComponentType"/>.
    /// Generated from <see cref="components"/>.
    /// </summary>
    private string[][] componentNames;

    private void OnEnable()
    {
        this.GenerateNames();
    }

    public void GenerateNames()
    {
        this.componentNames = new string[ShipData.ComponentTypes.Length][];
        foreach (ComponentType compType in ShipData.ComponentTypes)
        {
            ComponentList componentArray = this.components[(int) compType];
            this.componentNames[(int)compType] = new string[componentArray.components.Length];
            for (int iComponent = 0; iComponent < componentArray.components.Length; iComponent++)
            {
                ShipComponent comp = this.components[(int) compType].components[iComponent];
                this.componentNames[(int) compType][iComponent] = comp != null ? comp.name : "Null " + iComponent;
            }
        }
    }

    public string[] GetNames(ComponentType compType)
    {
        return this.componentNames == null ? new string[0] : this.componentNames[(int) compType];
    }

    public ShipComponent GetRawComponent(ComponentType compType, int index)
    {
        return this.components[(int) compType].components[index];
    }

    public T GetComponent<T>(ComponentType compType, int index) where T : ShipComponent
    {
        return (T)this.GetRawComponent(compType, index);
    }

}
