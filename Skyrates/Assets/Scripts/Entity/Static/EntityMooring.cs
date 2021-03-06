﻿using System.Collections;
using Skyrates.Game;
using Skyrates.Game.Event;
using Skyrates.Mono;
using Skyrates.UI;
using UnityEngine;

namespace Skyrates.Entity
{

	/// <summary>
	/// A static entity where you can upgrade your components.
	/// </summary>
	public class EntityMooring : Entity, IDistanceCollidable
	{

        /// <summary>
        /// The menu the player opens on interaction.
        /// </summary>
	    public UpgradeCanvas UpgradeMenu;

	    // TODO: Use state machine?

        /// <summary>
        /// True while the player is within radius of this object.
        /// </summary>
		private bool _playerNear;

        /// <summary>
        /// True while the upgrade menu is open.
        /// </summary>
        private bool _myMenuIsOpen;

	    private void OnEnable()
	    {
	        GameManager.Events.PlayerInteract += this.OnPlayerInteract;
	    }

	    private void OnDisable()
	    {
	        GameManager.Events.PlayerInteract -= this.OnPlayerInteract;
        }

	    /// <inheritdoc />
        public void OnEnterEntityRadius(EntityAI source, float radius)
		{
            // Only run when this is sent the first time
		    if (this._playerNear) return;

            // Attempt to show the player they can interact
            EntityPlayerShip player = source as EntityPlayerShip;
		    if (player)
		    {
		        this.StartCoroutine(this.PlayerDistanceOverlap(player, radius * radius));
		    }
		}

        /// <inheritdoc />
	    public void OnOverlapWith(GameObject other, float radius) { }

        /// <summary>
        /// While the player is near, this checks their distances and deactivates if they are too far away.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radiusSq"></param>
        /// <returns></returns>
	    IEnumerator PlayerDistanceOverlap(EntityPlayerShip player, float radiusSq)
	    {
            // The player is nearby, start running
	        this._playerNear = true;
            //GameManager.Events.Dispatch(EventMenu.Open(EventMenu.CanvasType.Upgrades));

            // Continue until this object is no longer active OR the player is too far
            while (this && this._playerNear && this.isActiveAndEnabled && player && (player.transform.position - this.transform.position).sqrMagnitude <= radiusSq)
	        {
                // wait till next frame
	            yield return null;
	        }

            // Out of range or this object is disabled
            // Double check the menu is closed - in case the player left the area without closing the menu
	        if (this._myMenuIsOpen)
	        {
	            this.CloseMenu(player);
            }

            // Player is no longer nearby, stop running
            this._playerNear = false;
	    }

        /// <summary>
        /// Event called when a player presses the interact button.
        /// </summary>
        /// <param name="evt"></param>
	    private void OnPlayerInteract(GameEvent evt)
	    {
            // If the player is not near this object, disregard
	        if (!this._playerNear) return;

	        EventEntityPlayerShip evtPlayer = (EventEntityPlayerShip) evt;

            // If the menu is not currently open, then open it
	        if (!this._myMenuIsOpen)
	        {
	            this.OpenMenu(evtPlayer.PlayerShip);
            }
            // Otherwise, the menu is open, so close it
	        else
	        {
	            this.CloseMenu(evtPlayer.PlayerShip);
            }
	    }

        /// <summary>
        /// Open the interaction menu.
        /// </summary>
        /// <param name="player"></param>
        private void OpenMenu(EntityPlayerShip player)
		{
            // Cache state
			this._myMenuIsOpen = true;

            // Open the menu
            GameManager.Events.Dispatch(EventMenu.Open(EventMenu.CanvasType.Upgrades));

            // Init the menu
            // TODO: Move this into an UpgradeMenu function
            this.UpgradeMenu.SetPlayer(player);
			foreach (UpgradeButton button in this.UpgradeMenu.MenuButtons)
			{
				button.AddUpgradeListener(player);
                button.RefreshPending();
			}
		}

	    /// <summary>
	    /// Close the interaction menu.
	    /// </summary>
	    /// <param name="player"></param>
	    /// <param name="forced"></param>
	    private void CloseMenu(EntityPlayerShip player)
	    {
	        // Cache state
            this._myMenuIsOpen = false;

	        GameManager.Events.Dispatch(EventMenu.Close(EventMenu.CanvasType.Upgrades));
            
            foreach (UpgradeButton button in this.UpgradeMenu.MenuButtons)
			{
				button.RemoveUpgradeListener();
			}
		}
        
	}

}