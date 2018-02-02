using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBuilder : MonoBehaviour
{

    public enum ComponentType
    {
        Hull, Propulsion, Ammunition, Sail, Figurehead, Navigation,
    }

    public ShipHull hull;
    public ShipComponent propulsion;
    public ShipComponent ammunition;
    public ShipComponent sail;
    public ShipComponent figurehead;
    public ShipComponent navigation;

}
