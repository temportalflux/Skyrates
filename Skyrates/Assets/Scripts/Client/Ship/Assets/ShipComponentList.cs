using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Ship Component List")]
public class ShipComponentList : ScriptableObject
{

    [Serializable]
    public struct ComponentList
    {
        public ShipComponent[] components;
    }

    /// <summary>
    /// An array keyed by <see cref="ShipBuilder.ComponentType"/> where values are arrays of ShipComponents.
    /// </summary>
    public ComponentList[] components = new ComponentList[ShipBuilder.ComponentTypes.Length];

    public bool[] editor_showComponentArray;

    /// <summary>
    /// The list of names for an array of components keyed by <see cref="ShipBuilder.ComponentType"/>.
    /// Generated from <see cref="components"/>.
    /// </summary>
    private string[][] componentNames;

    private void OnEnable()
    {
        this.GenerateNames();
    }

    public void GenerateNames()
    {
        this.componentNames = new string[ShipBuilder.ComponentTypes.Length][];
        foreach (ShipBuilder.ComponentType compType in ShipBuilder.ComponentTypes)
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

    public string[] GetNames(ShipBuilder.ComponentType compType)
    {
        return this.componentNames == null ? new string[0] : this.componentNames[(int) compType];
    }

    public T GetComponent<T>(ShipBuilder.ComponentType compType, int index) where T : ShipComponent
    {
        return (T)this.components[(int) compType].components[index];
    }

}
