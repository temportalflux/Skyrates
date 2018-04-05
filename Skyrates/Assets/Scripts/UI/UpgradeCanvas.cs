using Skyrates.Entity;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.UI
{

    public class UpgradeCanvas : MonoBehaviour
    {

        public UpgradeButton[] MenuButtons;

        public StatPanel[] Panels;

        private EntityPlayerShip _player;
        private ShipComponentList _componentList;

        void Start()
		{
			if(MenuButtons.Length > 0) MenuButtons[0].Button.Select(); //Select the first button if available.
		}

        public void SetPlayer(EntityPlayerShip player)
        {
            this._player = player;
            this._componentList = player.ShipGeneratorRoot.Blueprint.ShipComponentList;
        }

        void Update()
        {
            foreach (StatPanel panel in this.Panels)
            {
                panel.UpdateStats(this._componentList, this._player.ShipData);
            }
        }

    }

}