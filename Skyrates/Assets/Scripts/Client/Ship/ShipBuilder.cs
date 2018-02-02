using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Ship Builder")]
public class ShipBuilder : ScriptableObject
{
    public enum ComponentType
    {
        Hull, Propulsion, Ammunition, Sail, Figurehead, Navigation,
    }

    public static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.Hull,
        ComponentType.Propulsion,
        ComponentType.Ammunition,
        ComponentType.Sail,
        ComponentType.Figurehead,
        ComponentType.Navigation,
    };

    public static readonly Type[] ComponentClassTypes =
    {
        typeof(ShipHull),
        typeof(ShipComponent),
        typeof(ShipComponent),
        typeof(ShipComponent),
        typeof(ShipComponent),
        typeof(ShipComponent),
    };

    public ShipComponentList shipComponentList;

    public int[] components = new int[ComponentTypes.Length];

    public ShipHull GetHull()
    {
        return this.shipComponentList.GetComponent<ShipHull>(ComponentType.Hull, this.components[(int)ComponentType.Hull]);
    }

    public ShipComponent GetPropulsion()
    {
        return this.shipComponentList.GetComponent<ShipComponent>(ComponentType.Propulsion, this.components[(int)ComponentType.Propulsion]);
    }

    public ShipComponent GetAmmunition()
    {
        return this.shipComponentList.GetComponent<ShipComponent>(ComponentType.Ammunition, this.components[(int)ComponentType.Ammunition]);
    }

    public ShipComponent GetSail()
    {
        return this.shipComponentList.GetComponent<ShipComponent>(ComponentType.Sail, this.components[(int)ComponentType.Sail]);
    }

    public ShipComponent GetFigurehead()
    {
        return this.shipComponentList.GetComponent<ShipComponent>(ComponentType.Figurehead, this.components[(int)ComponentType.Figurehead]);
    }

    public ShipComponent GetNavigation()
    {
        return this.shipComponentList.GetComponent<ShipComponent>(ComponentType.Navigation, this.components[(int)ComponentType.Navigation]);
    }

    public void BuildTo(ref GameObject root)
    {
        ShipHull hullPrefab = this.GetHull();
        ShipHull hullBuilt = Instantiate(hullPrefab.gameObject, root.transform).GetComponent<ShipHull>();

        foreach (ComponentType compType in ComponentTypes)
        {
            if (compType == ComponentType.Hull) continue;

            GameObject prefab = this.shipComponentList.GetComponent<ShipComponent>(
                compType, this.components[(int) compType]).gameObject;

            Transform[] targets = hullPrefab.GetRoots(compType);

            foreach (Transform target in targets)
            {
                GameObject built = Instantiate(prefab, target.position, target.rotation, root.transform);
                hullBuilt.AddShipComponent(compType, built.GetComponent<ShipComponent>());
            }
            
        }

    }

}
