using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class UpgradeCanvas : MonoBehaviour
    {

        public UpgradeButton[] MenuButtons;

        void Start()
		{
			if(MenuButtons.Length > 0) MenuButtons[0].Button.Select(); //Select the first button if available.
		}

	}

}