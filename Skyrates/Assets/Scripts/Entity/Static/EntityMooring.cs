using System.Collections;
using Skyrates.Client.UI;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

	/// <summary>
	/// A static entity where you can upgrade your components.
	/// </summary>
	public class EntityMooring : Common.Entity.Entity, DistanceCollidable
	{
		//TODO: doxygen
		private bool _active;
		private bool _upgrading;
		public GameObject CanvasObject;

	    public UpgradeCanvas UpgradeMenu;

        public void OnEnterEntityRadius(EntityAI source, float radius)
		{
			if (!this._active) //Only once.
			{
				EntityPlayerShip player = source as EntityPlayerShip;
				if (player)
				{
					this.StartCoroutine(this.PlayerDistanceOverlap(player, radius));
				}
			}
		}

		private void OpenUpgradeMenu(EntityPlayerShip player)
		{
			this._upgrading = true;
			this.CanvasObject.SetActive(false);
			foreach (UpgradeButton button in this.UpgradeMenu.MenuButtons)
			{
				button.AddUpgradeListener(player);
			}
		    this.UpgradeMenu.gameObject.SetActive(true);
		}

		private void CloseUpgradeMenu(EntityPlayerShip player, bool forced = false)
		{
			this._upgrading = false;
			if (!forced) CanvasObject.SetActive(true);
			foreach (UpgradeButton button in this.UpgradeMenu.MenuButtons)
			{
				button.RemoveUpgradeListener();
			}
			this.UpgradeMenu.gameObject.SetActive(false);
		}

		IEnumerator PlayerDistanceOverlap(EntityPlayerShip player, float radius)
		{
			this.CanvasObject.SetActive(true);
			this._active = true;

			while (this && this._active && this.isActiveAndEnabled && player && (player.transform.position - this.transform.position).sqrMagnitude <= radius)
			{
				if(player.PlayerData.InputData.IsInteractingOnThisFrame)
				{
					if(!this._upgrading) this.OpenUpgradeMenu(player);
					else this.CloseUpgradeMenu(player);
				}
				yield return null;
			}

			//Out of range or disabled.
			this._active = false;
			this.CloseUpgradeMenu(player, true);
			this.CanvasObject.SetActive(false);
		}

		public void OnExitEntityRadius(EntityAI source, float radius)
		{
		}

		public void OnOverlapWith(GameObject other, float radius)
		{
		}
	}
}