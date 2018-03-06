using Skyrates.Client.Data;
using Skyrates.Common.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Skyrates.Client.Entity;
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
            this.Controller.AddInputEventDelegate(this.OnInputMenu, UpdateLoopType.Update, "Menu");
            this.Controller.AddInputEventDelegate(this.OnInputExit, UpdateLoopType.Update, "Exit");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Free");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Starboard");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Port");
            this.Controller.AddInputEventDelegate(this.OnInputCameraMode, UpdateLoopType.Update, "Mode:Down");
            this.Controller.AddInputEventDelegate(this.OnInputFire, UpdateLoopType.Update, "Fire");
            this.Controller.AddInputEventDelegate(this.OnInputAim, UpdateLoopType.Update, "Aim");
            this.Controller.AddInputEventDelegate(this.OnInputReload, UpdateLoopType.Update, "Reload:Main");
            this.Controller.AddInputEventDelegate(this.OnInputReload, UpdateLoopType.Update, "Reload:Alt");
        }

        void OnDisable()
        {
            this.Controller.RemoveInputEventDelegate(this.OnInputMenu);
            this.Controller.RemoveInputEventDelegate(this.OnInputExit);
            this.Controller.RemoveInputEventDelegate(this.OnInputCameraMode);
        }

        void OnInputMenu(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;
            
            // Go back to main menu                
            SceneLoader.Instance.Enqueue(SceneData.SceneKey.MenuMain);
            SceneLoader.Instance.ActivateNext();
        }

        void OnInputExit(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;
            
            // Exit the game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();          
#endif
        }

        void OnInputCameraMode(InputActionEventData evt)
        {
            switch (evt.actionName)
            {
                case "Mode:Free":
                    this.PlayerData.ViewMode = LocalData.CameraMode.FREE;
                    break;
                case "Mode:Starboard":
                    this.PlayerData.ViewMode = LocalData.CameraMode.LOCK_RIGHT;
                    break;
                case "Mode:Port":
                    this.PlayerData.ViewMode = LocalData.CameraMode.LOCK_LEFT;
                    break;
                case "Mode:Down":
                    this.PlayerData.ViewMode = LocalData.CameraMode.LOCK_DOWN;
                    break;
            }
            // TODO: Snap camera
        }

        void OnInputFire(InputActionEventData evt)
        {
            ShipData.ComponentType compType;
            switch (this.PlayerData.ViewMode)
            {
                case LocalData.CameraMode.LOCK_RIGHT:
                case LocalData.CameraMode.FREE:
                    compType = ShipData.ComponentType.ArtilleryRight;
                    break;
                case LocalData.CameraMode.LOCK_LEFT:
                    compType = ShipData.ComponentType.ArtilleryLeft;
                    break;
                default:
                    return;
            }
            this.ToggleShooting(evt.GetAxis() > 0.0f, compType);
        }

        void OnInputAim(InputActionEventData evt)
        {
            switch (this.PlayerData.ViewMode)
            {
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
            if (evt.actionName == "Reload:Alt")
            {
                if (this.PlayerData.ViewMode == LocalData.CameraMode.FREE)
                {
                    // TODO: Fire reload event (main false, alt true)
                }
            }
            else
            {
                // TODO: Fire reload event (main true, alt false)
            }
        }

        private void Update()
        {
            this.GetInput();
            this.ProcessInput();
        }

        void GetInput()
        {
            // Forward
            float fwd = this._controller.GetButton("Boost") ? 1 : 0;
            float bkwd = this._controller.GetButton("Brake") ? 1 : 0;
            this.PlayerData.input.MoveForward.Input = fwd - bkwd;

            // Turning
            this.PlayerData.input.TurnY.Input = this._controller.GetAxis("Turn Horizontal");

            // Move vertical
            this.PlayerData.input.MoveVertical.Input = this._controller.GetAxis("Move MoveVertical");

            // Camera
            if (this.PlayerData.ViewMode == LocalData.CameraMode.FREE)
            {
                this.PlayerData.input.CameraVertical.Input = -this._controller.GetAxis("Move Camera MoveVertical");
                this.PlayerData.input.CameraHorizontal.Input = this._controller.GetAxis("Move Camera Horizontal");
            }
            else
            {
                this.PlayerData.input.CameraVertical.Input =
                    this.PlayerData.input.CameraHorizontal.Input = 0.0f;
            }
        }

        void ProcessInput()
        {
            this.ProcessCamera();
        }

        void ProcessCamera()
        {
            // Grab the vertical axis (up)
            Vector3 dirVertical = this.Camera.up;
            dirVertical.x = dirVertical.z = 0;

            // Rotate around the vertical axis
            this.Camera.RotateAround(this.CameraPivot.position, dirVertical, this.PlayerData.input.CameraHorizontal.Value);

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
            this.Camera.RotateAround(this.CameraPivot.position, dirHorizontal, this.PlayerData.input.CameraVertical.Value);
        }

        private void ToggleShooting(bool isShooting, ShipData.ComponentType artillery)
        {
            bool wasShooting = this.PlayerData.input.IsShooting;
            this.PlayerData.input.IsShooting = isShooting;

            if (!wasShooting && isShooting)
            {
                // start shooting
            }
            else if (wasShooting && !isShooting)
            {
                // stop shooting
            }
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