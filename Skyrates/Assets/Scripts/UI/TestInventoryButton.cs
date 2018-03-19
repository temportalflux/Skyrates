using Skyrates.Data;
using Skyrates.Ship;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{
    public enum DebugType
    {
        //For testing purposes only.  No Doxygen.
        None, //Currently removes by default
        Add
    }

    // TODO: Remove, this is a test class
    public class TestInventoryButton : MonoBehaviour
    {
        //For testing purposes only.  No Doxygen.
        public PlayerData PlayerData;
        public Button Button;
        public Text Text;
        public ShipData.BrokenComponentType Type; //Type of the button, shows what item type it aligns to.

        public DebugType DebugType; //Remove or add, for debugging purposes.

        //Adds item to local data.
        void AddItem()
        {
            Debug.Log(string.Format("Added {0} unit(s) to {1}", PlayerData.Inventory.Add(Type), GetName(Type)));
        }

        //Removes item from local data.
        void RemoveItem()
        {
            Debug.Log(string.Format("Removed {0} unit(s) from {1}", PlayerData.Inventory.Remove(Type), GetName(Type)));
        }

        //Helper method to shorten the code a bit.
        private string GetName(ShipData.BrokenComponentType brokenComponent)
        {
            return Inventory.GetName(Type);
        }

        void Start()
        {
            PlayerData.Init(); //For testing purposes, reset the player data.
            switch (DebugType)
            {
                default:
                case DebugType.None:
                    Button.onClick.AddListener(RemoveItem);
                    break;
                case DebugType.Add:
                    Button.onClick.AddListener(AddItem);
                    break;
            }
        }


        void Update()
        {
            switch (DebugType)
            {
                default:
                case DebugType.None:
                    //Text.text = string.Format("{0}: {1} Units", type, PlayerData.Inventory.GetAmount(type)); //For debugging.
                    Text.text = PlayerData.Inventory.GetAmount(Type).ToString(); //For ingame.
                    break;
                case DebugType.Add:
                    Text.text = string.Format("Add {0} Unit", GetName(Type));
                    break;
            }
        }
    }
}