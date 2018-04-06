using System;
using System.Collections.Generic;
using Cinemachine;
using Rewired;
using Skyrates.Data;
using Skyrates.Entity;
using Skyrates.Game;
using Skyrates.Game.Event;
using Skyrates.Misc;
using Skyrates.Ship;
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

            public virtual void OnEnter(PlayerController owner)
            {
            }

            public virtual void UpdatePre(Rewired.Player inputController, PlayerController owner, ref PlayerData.Input data)
            {
				if (data.BlockInputs)
				{
					//Forward
					data.MoveForward.Input = -1.0f; //Force player to slow down to a stop when inputs are blocked.
					//TODO: Do we really want this to be automatic for all blocked inputs?  Perhaps a new variable declaring whether or not to auto brake?

					// Turning
					data.TurnY.Input = 0.0f;

					// Move vertical
					data.MoveVertical.Input = 0.0f;
				}
				else
				{
					// Forward
					float fwd = inputController.GetButton("Boost") ? 1 : 0;
					float bkwd = inputController.GetButton("Brake") ? 1 : 0;
					data.MoveForward.Input = fwd - bkwd;

					// Turning
					data.TurnY.Input = inputController.GetAxis("Turn Horizontal");

					// Move vertical
					data.MoveVertical.Input = inputController.GetAxis("Move Vertical");
                }

                data.CameraHorizontal.Input = 0.0f;
                data.CameraVertical.Input = 0.0f;

                if (inputController.GetButtonDown("Interact")) //Why can't this be in OnInputInteract now?
                {
                    GameManager.Events.Dispatch(new EventEntityPlayerShip(GameEventID.PlayerInteract, owner.EntityPlayerShip));
                }
            }

            public virtual void Update(PlayerController owner, PlayerData.Input data)
            {
            }

            public virtual void OnExit() { }

        }

        protected class PlayerStateUnlocked : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                base.OnEnter(owner);
                owner.FreeLookCamera.m_XAxis.Value = 0.5f;
                owner.FreeLookCamera.m_YAxis.Value = 0.5f;
                owner.StateAnimator.SetTrigger("Unlocked");
            }

            public override void UpdatePre(Player inputController, PlayerController owner, ref PlayerData.Input data)
            {
                base.UpdatePre(inputController, owner, ref data);

                data.CameraHorizontal.Input = inputController.GetAxis("Move Camera Horizontal");
                data.CameraVertical.Input = -inputController.GetAxis("Move Camera Vertical");

                owner.FreeLookCamera.m_XAxis.Value += data.CameraHorizontal.Value;
                owner.FreeLookCamera.m_YAxis.Value += data.CameraVertical.Value;
            }
        }

        protected class PlayerStateBroadsideStar : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                base.OnEnter(owner);
                owner.StateAnimator.SetTrigger("Broadside:Star");
            }

        }

        protected class PlayerStateBroadsidePort : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                base.OnEnter(owner);
                owner.StateAnimator.SetTrigger("Broadside:Port");
            }

        }

        protected class PlayerStateBomb : PlayerState
        {

            public override void OnEnter(PlayerController owner)
            {
                base.OnEnter(owner);
                owner.StateAnimator.SetTrigger("Bomb");
            }

        }

        // maps the input string to the state
        private readonly Dictionary<string, PlayerState> PlayerStates = new Dictionary<string, PlayerState>();
		// maps the input string to the view mode
		private readonly Dictionary<string, PlayerData.CameraMode> ViewModes = new Dictionary<string, PlayerData.CameraMode>();

		private PlayerState PlayerStateCurrent;

        void Awake()
        {
            // unneccessary flag, only used to init the contorller data
            this.Controller.isPlaying = true;

            this.PlayerStates.Add("Mode:Free", this.PlayerStateCurrent = new PlayerStateUnlocked());
            this.PlayerStates.Add("Mode:Starboard", new PlayerStateBroadsideStar());
            this.PlayerStates.Add("Mode:Port", new PlayerStateBroadsidePort());
            this.PlayerStates.Add("Mode:Down", new PlayerStateBomb());

			this.ViewModes.Add("Mode:Free", this.PlayerData.ViewMode = PlayerData.CameraMode.FREE);
			this.ViewModes.Add("Mode:Starboard", PlayerData.CameraMode.LOCK_RIGHT);
			this.ViewModes.Add("Mode:Port", PlayerData.CameraMode.LOCK_LEFT);
			this.ViewModes.Add("Mode:Down", PlayerData.CameraMode.LOCK_DOWN);

			this.PlayerStateCurrent.OnEnter(this);
            this.PlayerData.Artillery.Awake();
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
            if (PlayerData.InputData.BlockInputs || !evt.GetButtonDown() || !this.PlayerStates.ContainsKey(evt.actionName)) return;

            this.PlayerStateCurrent.OnExit();
            this.PlayerStateCurrent = this.PlayerStates[evt.actionName];
			this.PlayerData.ViewMode = this.ViewModes[evt.actionName];
			this.PlayerStateCurrent.OnEnter(this);
        }

        void OnInputFire(InputActionEventData evt)
        {
            if (!(evt.GetAxis() > 0.0f)) return;
            switch (this.PlayerData.ViewMode)
            {
                case PlayerData.CameraMode.FREE:
                    if (this.EntityPlayerShip.CanFireGimbal())
                    {
                        this.PlayerData.Artillery.Gimbal = (StateActiveReload)this.ShootCooldown(
                            this.PlayerData.Artillery.Gimbal, () =>
                            {
                                this.EntityPlayerShip.Shoot(ShipData.ComponentType.ArtilleryForward, this.GetForwardCannonDirection);
                            }
                        );
                    }
                    break;
                case PlayerData.CameraMode.LOCK_LEFT:
                    this.PlayerData.Artillery.Port = (StateOverheat) this.ShootCooldown(
                        this.PlayerData.Artillery.Port, () =>
                        {
                            this.EntityPlayerShip.Shoot(ShipData.ComponentType.ArtilleryLeft);
                        }
                    );
                    break;
                case PlayerData.CameraMode.LOCK_RIGHT:
                    this.PlayerData.Artillery.Starboard = (StateOverheat) this.ShootCooldown(
                        this.PlayerData.Artillery.Starboard, () =>
                        {
                            this.EntityPlayerShip.Shoot(ShipData.ComponentType.ArtilleryRight);
                        }
                    );
                    break;
                case PlayerData.CameraMode.LOCK_DOWN:
                    this.PlayerData.Artillery.Bombs = this.ShootCooldown(
                        this.PlayerData.Artillery.Bombs, () =>
                        {
                            this.EntityPlayerShip.Shoot(ShipData.ComponentType.ArtilleryDown);
                        });
                    break;
                default:
                    break;
            }

        }

        void OnInputAim(InputActionEventData evt)
        {
            
        }

        void OnInputReload(InputActionEventData evt)
        {
            if (PlayerData.InputData.BlockInputs || !evt.GetButtonDown()) return;

            bool isMainReload = evt.actionName == "Reload:Main";

            if (isMainReload && this.PlayerData.ViewMode == PlayerData.CameraMode.FREE)
            {
                this.PlayerData.Artillery.Gimbal.TryReload();

            }
            
        }

        void OnInputInteract(InputActionEventData evt)
        {
            if (!evt.GetButtonDown()) return;
            //Debug.Log("Interact"); //Commented so it wouldn't get in the way of any error messages.
        }

        void OnInputSwitchWeapon(InputActionEventData evt)
        {
            if (PlayerData.InputData.BlockInputs || !evt.GetButtonDown()) return;
            //Debug.Log("Switch Weapon");
        }

        private void Update()
        {
            this.GetInput();
            this.PlayerStateCurrent.Update(this, this.PlayerData.InputData);
            this.PlayerData.Artillery.Update(Time.deltaTime, this.EntityPlayerShip.ShipData);
        }

        void GetInput()
        {
            this.PlayerStateCurrent.UpdatePre(this._controller, this, ref this.PlayerData.InputData);
        }

        private StateCooldown ShootCooldown(StateCooldown cooldown, Action shoot)
        {
            if (!cooldown.IsLoaded()) return cooldown;
            cooldown.Unload();
            shoot();
            return cooldown;
        }

        private Vector3 GetForwardCannonDirection(ShipArtillery artillery)
        {
            // Get total distance of projectile
            EntityProjectile projectile = artillery.Shooter.projectilePrefab as EntityProjectile;
            if (projectile == null) return artillery.transform.forward;
            float distance = projectile.SelfDestruct.Delay * artillery.DistanceModifier;

            // Get location from camera POV at distance
            Transform viewCamera = this.EntityPlayerShip.Camera.transform;
            Vector3 source = artillery.Shooter.spawn.position;

            Vector3 srcWrtCamera = source - viewCamera.position;
            Vector3 projection = Vector3.Project(srcWrtCamera, viewCamera.forward);
            Vector3 rejection = srcWrtCamera - projection;
            float cameraDistance = Mathf.Sqrt(distance * distance - rejection.sqrMagnitude);
            Vector3 destination = viewCamera.forward * cameraDistance;

            return destination;
        }

    }

}