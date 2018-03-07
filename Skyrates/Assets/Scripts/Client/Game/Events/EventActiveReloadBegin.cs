using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Entity;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Game.Event
{
    
    public class EventActiveReloadBegin : EventEntityPlayerShip
    {

        public bool IsStarboard;

        public float ActiveReloadStart;
        public float ActiveReloadEnd;

        public EventActiveReloadBegin(EntityPlayerShip playerShip,
            bool isStarboard, float percentStart, float percentEnd)
            : base(GameEventID.ActiveReloadBegin, playerShip)
        {
            this.IsStarboard = isStarboard;
            this.ActiveReloadStart = percentStart;
            this.ActiveReloadEnd = percentEnd;
        }

        public float GetPercentUpdate()
        {
            return this.IsStarboard
                ? this.PlayerShip.PlayerData.StateData.ShootingDataStarboardPercentReloaded
                : this.PlayerShip.PlayerData.StateData.ShootingDataPortPercentReloaded;
        }

    }

}
