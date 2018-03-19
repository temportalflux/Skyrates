using UnityEditor;

using ComponentType = Skyrates.Ship.ShipData.ComponentType;

namespace Skyrates.Ship
{
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
}