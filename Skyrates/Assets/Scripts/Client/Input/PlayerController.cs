using Skyrates.Client.Data;
using Skyrates.Common.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Client.Input
{

    /// <summary>
    /// Takes unity input and puts it into the player's data object
    /// </summary>
    public class PlayerController : MonoBehaviour
    {

        /// <summary>
        /// The data which contains input information
        /// </summary>
        public LocalData PlayerData;

        void Update()
        {
            this.GetInput();
        }

        private void GetInput()
        {

            // ~~~ Movement

            // ForwardInput is left stick (up/down)
            //this.PlayerData.input.Forward.Input = UnityEngine.Input.GetAxis("xbox_stick_l_vertical");
            float fwd = UnityEngine.Input.GetButton("xbox_a") ? 1 : 0;
            float bkwd = UnityEngine.Input.GetButton("xbox_b") ? 1 : 0;
            this.PlayerData.input.Forward.Input = fwd + -bkwd;

            // Strafe is left stick (left/right)
            this.PlayerData.input.Strafe.Input = UnityEngine.Input.GetAxis("xbox_stick_l_horizontal");

            // Vertical is bumpers
            this.PlayerData.input.Vertical.Input = UnityEngine.Input.GetButton("xbox_bumper_r") ? 1 :
                UnityEngine.Input.GetButton("xbox_bumper_l") ? -1 : 0;

            // ~~~ Camera

            // Vertical is right stick (up/down)
            this.PlayerData.input.CameraVertical.Input = UnityEngine.Input.GetAxis("xbox_stick_r_vertical");

            // Horizontal is right stick (left/right)
            this.PlayerData.input.CameraHorizontal.Input = UnityEngine.Input.GetAxis("xbox_stick_r_horizontal");

            this.PlayerData.input.DPadHorizontal.Input = UnityEngine.Input.GetAxis("xbox_dpad_horizontal");
            this.PlayerData.input.DPadVertical.Input = UnityEngine.Input.GetAxis("xbox_dpad_vertical");

            // ~~~ Shooting

            // ForwardInput is left stick (up/down)
            this.PlayerData.input.ShootRight.Input = UnityEngine.Input.GetAxis("xbox_trigger_r");
            this.PlayerData.input.ShootLeft.Input = UnityEngine.Input.GetAxis("xbox_trigger_l");

            // ~~~ Menu

            this.PlayerData.input.MainMenu = UnityEngine.Input.GetButtonDown("xbox_button_menu");
            this.PlayerData.input.Back = UnityEngine.Input.GetButtonDown("xbox_button_view");

        }

    }
}