using System;
using Skyrates.Client.Data;
using Skyrates.Client.Entity;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Scene;
using Skyrates.Client.UI;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;

namespace Skyrates.Client.Game
{

    /// <inheritdoc />
    /// <summary>
    /// Singleton which encapsulates <see cref="T:Skyrates.Client.Game.Event.GameEvents" />, local data, and win state.
    /// </summary>
    public class UpgradeManager : MonoBehaviour //Not a singleton, destroys on load.
    {
		//TODO: Doxygen.
        /// <summary>
        /// The <see cref="Singleton{UpgradeManager}"/> instance.
        /// </summary>
        public static UpgradeManager Instance;

        public GameObject UpgradeMenu; //The sole purpose for this class.  Reference once for use in all moorings.

		[HideInInspector]
		public UpgradeButton[] UpgradeMenuButtons;

		void Awake()
		{
			GameManager.Instance.UpgradeManager = this;
			UpgradeMenuButtons = UpgradeMenu.GetComponentsInChildren<UpgradeButton>();
		}
	}

}
