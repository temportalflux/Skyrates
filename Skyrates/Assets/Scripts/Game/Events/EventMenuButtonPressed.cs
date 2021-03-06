﻿
namespace Skyrates.Game.Event
{

    /// <summary>
    /// Dispatched when a game starts (regardless of if the network 
    /// </summary>
    public class EventMenuButtonPressed : GameEvent
    {

        public enum MenuButton
        {
            Menu, Back,
        }

        public MenuButton Button;

        public EventMenuButtonPressed(MenuButton button) : base(GameEventID.MenuButtonPressed)
        {
            this.Button = button;
        }

    }

}
