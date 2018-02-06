using Skyrates.Client.Ship;
using UnityEditor;

using ComponentType = ShipData.ComponentType;

[CustomEditor(typeof(ShipComponentList))]
public class ShipComponentListEditor : PrefabListEditor<ComponentType, ShipComponent>
{

    private static bool[] _keysEnabled;

    protected override bool[] GetStaticKeyToggleArray()
    {
        return _keysEnabled;
    }

    protected override void SetStaticKeyToggleArray(bool[] keyBlocksToggled)
    {
        _keysEnabled = keyBlocksToggled;
    }

}
