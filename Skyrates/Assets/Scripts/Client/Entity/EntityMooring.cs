using System.Collections;
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
		public float DetectionRadius = 15.0f;

		public void OnEnterEntityRadius(EntityAI source, float radius)
		{
			if (!this._active) //Only once.
			{
				EntityPlayerShip player = source as EntityPlayerShip;
				if (player)
				{
					float radiusSqr = DetectionRadius * DetectionRadius; //Avoid square root.  It is slow
					if ((player.transform.position - this.transform.position).sqrMagnitude <= radiusSqr) this.StartCoroutine(this.PlayerDistanceOverlap(player, radius));
				}
			}
		}

		private void OpenUpgradeMenu(EntityPlayerShip player)
		{
			_upgrading = true;
			CanvasObject.SetActive(false);
		}

		private void CloseUpgradeMenu(EntityPlayerShip player, bool forced = false)
		{
			_upgrading = false;
			if (!forced) CanvasObject.SetActive(true);
		}

		IEnumerator PlayerDistanceOverlap(EntityPlayerShip player, float radius)
		{
			float radiusSqr = DetectionRadius * DetectionRadius; //Avoid square root.  It is slow

			CanvasObject.SetActive(true);
			this._active = true;

			while (this && this._active && this.isActiveAndEnabled && player && (player.transform.position - this.transform.position).sqrMagnitude <= radiusSqr)
			{
				if(player.PlayerData.InputData.IsInteractingOnThisFrame)
				{
					if(!_upgrading) this.OpenUpgradeMenu(player);
					else this.CloseUpgradeMenu(player);
				}
				yield return null;
			}

			//Out of range or disabled.
			this._active = false;
			this.CloseUpgradeMenu(player, true);
			CanvasObject.SetActive(false);
		}

		public void OnExitEntityRadius(EntityAI source, float radius)
		{
		}

		public void OnOverlapWith(GameObject other, float radius)
		{
		}
	}
}