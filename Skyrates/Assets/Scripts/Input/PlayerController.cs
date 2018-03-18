﻿using Skyrates.Client.Data;
using Skyrates.Common.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Rewired;
using Skyrates.Client.Entity;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Scene;
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
        public PlayerData PlayerData;

        public EntityPlayerShip EntityPlayerShip;

        public Animator StateAnimator;

        public CinemachineFreeLook FreeLookCamera;

        private Rewired.Player _controller;

        private Rewired.Player Controller
        {
            get
            {
                if (this._controller == null)
                    this._controller = ReInput.players.GetPlayer(0);
                return this._controller;
            }
        }

        protected class PlayerState
        {

            public virtual void OnEnter(PlayerController owner) { }

            public virtual void UpdatePre(Rewired.Player inputController, PlayerController owner, ref PlayerData.Input data)
            {
                // Forward
                float fwd = inputController.GetButton("Boost") ? 1 : 0;
                float bkwd = inputController.GetButton("Brake") ? 1 : 0;
                data.MoveForward.Input = fwd - bkwd;

                // Turning
                data.TurnY.Input = inputController.GetAxis("Turn Horizontal");

                // Move vertical
                data.MoveVertical.Input = inputController.GetAxis("Move Vertical");

                data.CameraHorizontal.Input = 0.0f;
                data.CameraVertical.Input = 0.0f;

                if (inputController.GetButtonDown("Interact"))
                {
                    GameManager.Events.Dispatch(new EventEntityPlayerShip(GameEventID.PlayerInteract, owner.EntityPlayerShip));
                }
            }

            public virtual void Update(PlayerController owner, PlayerData.Input data)
            {
                owner.FreeLookCamera.m_XAxis.Value += data.CameraHorizontal.Value;
                owner.FreeLookCamera.m_YAxis.Value += data.CameraVertical.Value;
            }

            public virtual void OnExit() { }

        }

        protected class PlayerStateUnlocked : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                owner.StateAnimator.SetTrigger("Unlocked");
            }

            public override void UpdatePre(Player inputController, PlayerController owner, ref PlayerData.Input data)
            {
                base.UpdatePre(inputController, owner, ref data);

                data.CameraHorizontal.Input = inputController.GetAxis("Move Camera Horizontal");
                data.CameraVertical.Input = -inputController.GetAxis("Move Camera Vertical");
            }
        }

        protected class PlayerStateBroadsideStar : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                owner.StateAnimator.SetTrigger("Broadside:Star");
            }

        }

        protected class PlayerStateBroadsidePort : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                owner.StateAnimator.SetTrigger("Broadside:Port");
            }

        }

        protected class PlayerStateBomb : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                owner.StateAnimator.SetTrigger("Bomb");
            }

        }

        // maps the input string to the state
        private readonly Dictionary<string, PlayerState> PlayerStates = new Dictionary<string, PlayerState>();

        private PlayerState PlayerStateCurrent;

        void Awake()
        {
            // unneccessary flag, only used to init the contorller data
            this.Controller.isPlaying = true;

            this.PlayerStates.Add("Mode:Free", this.PlayerStateCurrent = new PlayerStateUnlocked());
            this.PlayerStates.Add("Mode:Starboard", new PlayerStateBroadsideStar());
            this.PlayerStates.Add("Mode:Port", new PlayerStateBroadsidePort());
            this.PlayerStates.Add("Mode:Down", new PlayerStateBomb());
            this.PlayerStateCurrent.OnEnter(this);
        }

        void OnEnable()
        {
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Free");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Starboard");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Port");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Down");

            this.Controller.AddInputEventDelegate(this.OnInputFire, UpdateLoopType.Update, "Fire");
            this.Controller.AddInputEventDelegate(this.OnInputAim, UpdateLoopType.Update, "Aim");
            this.Controller.AddInputEventDelegate(this.OnInputReload, UpdateLoopType.Update, "Reload:Main");
            this.Controller.AddInputEventDelegate(this.OnInputReload, UpdateLoopType.Update, "Reload:Alt");
            this.Controller.AddInputEventDelegate(this.OnInputInteract, UpdateLoopType.Update, "Interact");
            this.Controller.AddInputEventDelegate(this.OnInputSwitchWeapon, UpdateLoopType.Update, "Switch Weapon");
        }

        void OnDisable()
        {
            this.Controller.RemoveInputEventDelegate(this.OnInputCameraMode);
            this.Controller.RemoveInputEventDelegate(this.OnInputFire);
            this.Controller.RemoveInputEventDelegate(this.OnInputAim);
            this.Controller.RemoveInputEventDelegate(this.OnInputReload);
            this.Controller.RemoveInputEventDelegate(this.OnInputInteract);
            this.Controller.RemoveInputEventDelegate(this.OnInputSwitchWeapon);
        }

        void OnInputCameraMode(InputActionEventData evt)
        {
            if (!evt.GetButtonDown() || !this.PlayerStates.ContainsKey(evt.actionName)) return;

            this.PlayerStateCurrent.OnExit();
            this.PlayerStateCurrent = this.PlayerStates[evt.actionName];
            this.PlayerStateCurrent.OnEnter(this);

            //Transform cameraState = this.Camera.transform;
            //switch (evt.actionName)
            //{
            //    case "Mode:Free":
            //        this.PlayerData.StateData.ViewMode = PlayerData.CameraMode.FREE;
            //        cameraState = this.CameraModeFree;
            //        this.StateAnimator.SetTrigger("Unlocked");
            //        break;
            //    case "Mode:Starboard":
            //        this.PlayerData.StateData.ViewMode = PlayerData.CameraMode.LOCK_RIGHT;
            //        cameraState = this.CameraModeStarboard;
            //        this.StateAnimator.SetTrigger("Broadside:Star");
            //        break;
            //    case "Mode:Port":
            //        this.PlayerData.StateData.ViewMode = PlayerData.CameraMode.LOCK_LEFT;
            //        cameraState = this.CameraModePort;
            //        this.StateAnimator.SetTrigger("Broadside:Port");
            //        break;
            //    case "Mode:Down":
            //        this.PlayerData.StateData.ViewMode = PlayerData.CameraMode.LOCK_DOWN;
            //        cameraState = this.CameraModeDown;
            //        this.StateAnimator.SetTrigger("Bomb");
            //        break;
            //}
            
            //// TODO: Slerp camera
            //this.Camera.SetPositionAndRotation(cameraState.position, cameraState.rotation);

        }

        void OnInputFire(InputActionEventData evt)
        {
            ShipData.ComponentType compType;
            switch (this.PlayerData.StateData.ViewMode)
            {
                case PlayerData.CameraMode.LOCK_RIGHT:
                case PlayerData.CameraMode.FREE:
                    compType = ShipData.ComponentType.ArtilleryRight;
                    break;
                default:
                    return;
            }
            this.ToggleShooting(evt.GetAxis() > 0.0f, compType);
        }

        void OnInputAim(InputActionEventData evt)
        {
            switch (this.PlayerData.StateData.ViewMode)
            {
                case PlayerData.CameraMode.LOCK_LEFT:
                case PlayerData.CameraMode.FREE:
                    //this.PlayerData.input.AimScale = evt.GetAxis();
                    this.ToggleShooting(evt.GetAxis() > 0.0f, ShipData.ComponentType.ArtilleryLeft);
                    break;
                case PlayerData.CameraMode.LOCK_DOWN:
                    this.ToggleBombing(evt.GetAxis() > 0.0f);
                    break;
                default:
                    break;
            }
        }

        void OnInputReload(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;

            bool found;
            ShipData.ComponentType reloadTarget = this.GetActiveReloadTarget(evt.actionName == "Reload:Main", out found);
            if (found)
            {
                switch (reloadTarget)
                {
                    case ShipData.ComponentType.ArtilleryRight:
                        if (!this.PlayerData.StateData.ShootingDataStarboardIsReloading)
                        {
                            this.PlayerData.StateData.ShootingDataStarboardIsReloading = true;
                            this.PlayerData.StateData.ShootingDataStarboardPercentReloaded = 0.0f;
                        }
                        else
                        {
                            if (this.PlayerData.StateData.ShootingDataStarboardCanReload)
                            {
                                if (this.PlayerData.StateData.ShootingDataStarboardPercentReloaded >=
                                    this.PlayerData.StateData.ShootDelayActiveReloadStart &&
                                    this.PlayerData.StateData.ShootingDataStarboardPercentReloaded <=
                                    this.PlayerData.StateData.ShootDelayActiveReloadEnd)
                                {
                                    this.PlayerData.StateData.ShootingDataStarboardPercentReloaded = 1.0f;
                                }
                                else
                                {
                                    this.PlayerData.StateData.ShootingDataStarboardCanReload = false;
                                }
                            }
                        }
                        break;
                    case ShipData.ComponentType.ArtilleryLeft:
                        if (!this.PlayerData.StateData.ShootingDataPortIsReloading)
                        {
                            this.PlayerData.StateData.ShootingDataPortIsReloading = true;
                            this.PlayerData.StateData.ShootingDataPortPercentReloaded = 0.0f;
                        }
                        else
                        {
                            if (this.PlayerData.StateData.ShootingDataPortCanReload)
                            {
                                if (this.PlayerData.StateData.ShootingDataPortPercentReloaded >=
                                    this.PlayerData.StateData.ShootDelayActiveReloadStart &&
                                    this.PlayerData.StateData.ShootingDataPortPercentReloaded <=
                                    this.PlayerData.StateData.ShootDelayActiveReloadEnd)
                                {
                                    this.PlayerData.StateData.ShootingDataPortPercentReloaded = 1.0f;
                                }
                                else
                                {
                                    this.PlayerData.StateData.ShootingDataPortCanReload = false;
                                }

                            }
                        }
                        break;
                }
            }
        }

        ShipData.ComponentType GetActiveReloadTarget(bool main, out bool found)
        {
            found = true;
            return main ? ShipData.ComponentType.ArtilleryRight : ShipData.ComponentType.ArtilleryLeft;
        }

        void OnInputInteract(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;
            //Debug.Log("Interact"); //Commented so it wouldn't get in the way of any error messages.
        }

        void OnInputSwitchWeapon(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;
            Debug.Log("Switch Weapon");
        }

        private void Update()
        {
            this.GetInput();
            this.ProcessInput(Time.deltaTime);
        }

        void GetInput()
        {
            this.PlayerStateCurrent.UpdatePre(this._controller, this, ref this.PlayerData.InputData);
        }

        void ProcessInput(float deltaTime)
        {
            this.PlayerStateCurrent.Update(this, this.PlayerData.InputData);
            this.ProcessActiveReload(deltaTime);
        }

        void ProcessActiveReload(float deltaTime)
        {
            float deltaAmt = deltaTime / this.PlayerData.StateData.ShootDelay;

            if (this.PlayerData.StateData.ShootingDataStarboardIsReloading)
            {
                this.PlayerData.StateData.ShootingDataStarboardPercentReloaded =
                    Mathf.Min(1.0f, this.PlayerData.StateData.ShootingDataStarboardPercentReloaded + deltaAmt);
                if (this.PlayerData.StateData.ShootingDataStarboardPercentReloaded >= 1.0f)
                {
                    this.PlayerData.StateData.ShootingDataStarboardCanReload = false;
                    this.PlayerData.StateData.ShootingDataStarboardIsReloading = false;
                }
            }

            if (this.PlayerData.StateData.ShootingDataPortIsReloading)
            {
                this.PlayerData.StateData.ShootingDataPortPercentReloaded =
                    Mathf.Min(1.0f, this.PlayerData.StateData.ShootingDataPortPercentReloaded + deltaAmt);
                if (this.PlayerData.StateData.ShootingDataPortPercentReloaded >= 1.0f)
                {
                    this.PlayerData.StateData.ShootingDataPortCanReload = false;
                    this.PlayerData.StateData.ShootingDataPortIsReloading = false;
                }
            }

        }

        private void ToggleShooting(bool isShooting, ShipData.ComponentType artillery)
        {
            if (!isShooting) return;

            float percentReloaded = 0.0f;
            switch (artillery)
            {
                case ShipData.ComponentType.ArtilleryRight:
                    percentReloaded = this.PlayerData.StateData.ShootingDataStarboardPercentReloaded;
                    break;
                case ShipData.ComponentType.ArtilleryLeft:
                    percentReloaded = this.PlayerData.StateData.ShootingDataPortPercentReloaded;
                    break;
                default:
                    return;
            }
            
            if (percentReloaded < 1.0f) return;
            percentReloaded = 0.0f;

            this.Shoot(artillery);

            // TODO: Do I need this to update the values?
            switch (artillery)
            {
                case ShipData.ComponentType.ArtilleryRight:
                    this.PlayerData.StateData.ShootingDataStarboardCanReload = true;
                    this.PlayerData.StateData.ShootingDataStarboardIsReloading = false;
                    this.PlayerData.StateData.ShootingDataStarboardPercentReloaded = percentReloaded;
                    break;
                case ShipData.ComponentType.ArtilleryLeft:
                    this.PlayerData.StateData.ShootingDataPortCanReload = true;
                    this.PlayerData.StateData.ShootingDataPortIsReloading = false;
                    this.PlayerData.StateData.ShootingDataPortPercentReloaded = percentReloaded;
                    break;
            }

            GameManager.Events.Dispatch(new EventActiveReloadBegin(
                this.EntityPlayerShip,
                artillery == ShipData.ComponentType.ArtilleryRight,
                this.PlayerData.StateData.ShootDelayActiveReloadStart,
                this.PlayerData.StateData.ShootDelayActiveReloadEnd
            ));

        }

        private void Shoot(ShipData.ComponentType artillery)
        {
            this.EntityPlayerShip.Shoot(artillery);
        }

        private void ToggleBombing(bool isBombing)
        {
            // TODO: Implement bombing
        }

    }
}