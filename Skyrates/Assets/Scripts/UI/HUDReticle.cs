using System.Collections;
using System.Collections.Generic;
using Skyrates.Game;
using Skyrates.Mono;
using Skyrates.Ship;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    [RequireComponent(typeof(Image))]
    public class HUDReticle : MonoBehaviour
    {

        public float OverlapDelay = 0.1f;
        private float _timeElapsed;

        private Image Image;

        public LayerMask MaskEnemy;
        public Color ColorEnemy;
        public LayerMask MaskFriendly;
        public Color ColorFriendly;
        public Color ColorDisabled;

        private Color _colorDefault;
        private Transform _camera;

        void Start()
        {
            this.Image = this.GetComponent<Image>();
            this._colorDefault = this.Image.color;
            this._timeElapsed = 0.0f;
        }

        void Update()
        {
            if (this._camera == null)
                this._camera = GameObject.FindGameObjectWithTag("MainCamera").transform;

            this._timeElapsed += Time.deltaTime;
            if (this._timeElapsed >= this.OverlapDelay)
            {
                this._timeElapsed = 0.0f;
                this.Image.color = this.RaycastReturnColor();
            }

        }

        private Color RaycastReturnColor()
        {
            if (!GameManager.Instance.PlayerInstance.CanFireGimbal())
            {
                return this.ColorDisabled;
            }
            else if (this.Raycast(this.MaskEnemy))
            {
                return this.ColorEnemy;
            }
            else if (this.Raycast(this.MaskFriendly))
            {
                return this.ColorFriendly;
            }
            else
            {
                return this._colorDefault;
            }
        }

        private bool Raycast(LayerMask mask)
        {
            // TODO: Make this less coupled and move into functions
            float distance = GameManager.Instance.PlayerInstance.ShipData.GetStat<ShipArtillery>(
                GameManager.Instance.PlayerInstance.ShipGeneratorRoot.Blueprint.ShipComponentList,
                ShipData.ComponentType.ArtilleryForward, artillery => artillery.DistanceModifier * artillery.Shooter.projectilePrefab.GetComponent<SelfDestruct>().Delay, false);
            return UnityEngine.Physics.Raycast(this._camera.position, this._camera.forward, distance, mask);
        }

    }

}
