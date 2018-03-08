using Skyrates.Client.Data;
using Skyrates.Common.AI;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public LocalData PlayerData;

        public EntityPlayerShip EntityPlayerShip;

        public Transform Camera;

        public Transform CameraPivot;

        public Transform CameraModeFree;
        public Transform CameraModeStarboard;
        public Transform CameraModePort;
        public Transform CameraModeDown;

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

        void Awake()
        {
            // unneccessary flag, only used to init the contorller data
            this.Controller.isPlaying = true;
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
        }

        void OnInputCameraMode(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;

            Transform cameraState = this.Camera.transform;
            switch (evt.actionName)
            {
                case "Mode:Free":
                    this.PlayerData.StateData.ViewMode = LocalData.CameraMode.FREE;
                    cameraState = this.CameraModeFree;
                    break;
                case "Mode:Starboard":
                    this.PlayerData.StateData.ViewMode = LocalData.CameraMode.LOCK_RIGHT;
                    cameraState = this.CameraModeStarboard;
                    break;
                case "Mode:Port":
                    this.PlayerData.StateData.ViewMode = LocalData.CameraMode.LOCK_LEFT;
                    cameraState = this.CameraModePort;
                    break;
                case "Mode:Down":
                    this.PlayerData.StateData.ViewMode = LocalData.CameraMode.LOCK_DOWN;
                    cameraState = this.CameraModeDown;
                    break;
            }
            
            // TODO: Slerp camera
            this.Camera.SetPositionAndRotation(cameraState.position, cameraState.rotation);

        }

        void OnInputFire(InputActionEventData evt)
        {
            ShipData.ComponentType compType;
            switch (this.PlayerData.StateData.ViewMode)
            {
                case LocalData.CameraMode.LOCK_RIGHT:
                case LocalData.CameraMode.FREE:
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
                case LocalData.CameraMode.LOCK_LEFT:
                case LocalData.CameraMode.FREE:
                    //this.PlayerData.input.AimScale = evt.GetAxis();
                    this.ToggleShooting(evt.GetAxis() > 0.0f, ShipData.ComponentType.ArtilleryLeft);
                    break;
                case LocalData.CameraMode.LOCK_DOWN:
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
                        break;
                    case ShipData.ComponentType.ArtilleryLeft:
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
            // Forward
            float fwd = this._controller.GetButton("Boost") ? 1 : 0;
            float bkwd = this._controller.GetButton("Brake") ? 1 : 0;
            this.PlayerData.InputData.MoveForward.Input = fwd - bkwd;

            // Turning
            this.PlayerData.InputData.TurnY.Input = this._controller.GetAxis("Turn Horizontal");

            // Move vertical
            this.PlayerData.InputData.MoveVertical.Input = this._controller.GetAxis("Move Vertical");

            // Camera
            if (this.PlayerData.StateData.ViewMode == LocalData.CameraMode.FREE)
            {
                this.PlayerData.InputData.CameraVertical.Input = -this._controller.GetAxis("Move Camera Vertical");
                this.PlayerData.InputData.CameraHorizontal.Input = this._controller.GetAxis("Move Camera Horizontal");
            }
            else
            {
                this.PlayerData.InputData.CameraVertical.Input =
                    this.PlayerData.InputData.CameraHorizontal.Input = 0.0f;
            }
			this.PlayerData.InputData.IsInteractingOnThisFrame = this._controller.GetButtonDown("Interact");
        }

        void ProcessInput(float deltaTime)
        {
            this.ProcessCamera();
            this.ProcessActiveReload(deltaTime);
        }

        void ProcessCamera()
        {
            // Grab the vertical axis (up)
            Vector3 dirVertical = this.Camera.up;
            dirVertical.x = dirVertical.z = 0;

            // Rotate around the vertical axis
            this.Camera.RotateAround(this.CameraPivot.position, dirVertical, this.PlayerData.InputData.CameraHorizontal.Value);

            // Grab the x axis (right)
            Vector3 dirHorizontal = this.Camera.right;
            dirVertical.y = dirVertical.z = 0;


            /*
            float rotateDegrees = this.playerInput.MoveVertical;
            Vector3 currentVector = transform.position - this.pivot.position;
            currentVector.y = 0;
            Vector3 iVec = this.transform.forward.Flatten(Vector3.up);
            float angleBetween = Vector3.Angle(iVec, currentVector) * (Vector3.Cross(iVec, currentVector).y > 0 ? 1 : -1);
            float newAngle = Mathf.Clamp(angleBetween + rotateDegrees, -90, 40);
            rotateDegrees = newAngle - angleBetween;
            */

            // Pivot around the local left/right axis (right of the facing direction)
            this.Camera.RotateAround(this.CameraPivot.position, dirHorizontal, this.PlayerData.InputData.CameraVertical.Value);
        }

        void ProcessActiveReload(float deltaTime)
        {
            float deltaAmt = deltaTime / this.PlayerData.StateData.ShootDelay;
            this.PlayerData.StateData.ShootingDataStarboardPercentReloaded =
                Mathf.Min(1.0f, this.PlayerData.StateData.ShootingDataStarboardPercentReloaded + deltaAmt);
            this.PlayerData.StateData.ShootingDataPortPercentReloaded =
                Mathf.Min(1.0f, this.PlayerData.StateData.ShootingDataPortPercentReloaded + deltaAmt);
        }

        private void ToggleShooting(bool isShooting, ShipData.ComponentType artillery)
        {
            if (!isShooting) return;

            float percentReloaded;
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

            this.Shoot(artillery);
            percentReloaded = 0.0f;

            // TODO: Do I need this to update the values?
            switch (artillery)
            {
                case ShipData.ComponentType.ArtilleryRight:
                    this.PlayerData.StateData.ShootingDataStarboardPercentReloaded = percentReloaded;
                    this.PlayerData.StateData.ShootingDataStarboardCanReload = true;
                    break;
                case ShipData.ComponentType.ArtilleryLeft:
                    this.PlayerData.StateData.ShootingDataPortPercentReloaded = percentReloaded;
                    this.PlayerData.StateData.ShootingDataPortCanReload = true;
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