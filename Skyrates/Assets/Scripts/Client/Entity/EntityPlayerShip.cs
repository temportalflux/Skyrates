﻿using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Data;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Mono;
using Skyrates.Client.Scene;
using Skyrates.Client.Ship;
using Skyrates.Common.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Skyrates.Client.Entity
{

    /// <summary>
    /// The ship that a player controls.
    /// </summary>
    public class EntityPlayerShip : EntityShip
    {

        /// <summary>
        /// The network safe data which has information about the modular-ship.
        /// </summary>
        [BitSerialize(5)]
        [HideInInspector]
        public ShipData ShipData;

        /// <summary>
        /// The non-networked data local to the player.
        /// </summary>
        public LocalData PlayerData;

        /// <summary>
        /// The transform which points towards where the forward direction is.
        /// </summary>
        [Tooltip("The transform which points towards where the forward direction is.")]
        public Transform View;

        /// <summary>
        /// The root of the render object (must be a child/decendent of this root).
        /// </summary>
        [Tooltip("The root of the render object (must be a child/decendent of this root).")]
        public Transform Render;

        /// <summary>
        /// The Ship component which creates the modular ship.
        /// </summary>
        public Ship.Ship ShipRoot;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            this.ShipRoot.Destroy();
            this.ShipData = this.ShipRoot.Generate(this);
            this.PlayerData.Init();
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            StartCoroutine(this.AutoHeal());
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.PlayerData.Init();
        }

        /// <inheritdoc />
        protected override Transform GetView()
        {
            return this.View;
        }

        /// <inheritdoc />
        public override Transform GetRender()
        {
            return this.Render;
        }

        /// <summary>
        /// Auto heals the player ship every 5 secodns while the health is less than max health.
        /// </summary>
        /// <returns></returns>
        IEnumerator AutoHeal()
        {
            while (true)
            {
                yield return new WaitUntil((() => this.Health < this.StatBlock.Health));
                while (this.Health < this.StatBlock.Health)
                {
                    this.Health++;
                    yield return new WaitForSeconds(5.0f);
                }
            }
        }

        /// <inheritdoc />
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            GameManager.Events.Dispatch(new EventEntityPlayerShip(GameEventID.PlayerMoved, this));

            // TODO: Make events for these

            if (this.PlayerData.input.MainMenu)
            {
                // Go back to main menu                
                SceneLoader.Instance.Enqueue(SceneData.SceneKey.MenuMain);
                SceneLoader.Instance.ActivateNext();
            }

            if (this.PlayerData.input.Back)
            {
                // Exit the game
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();          
#endif
            }
        }

        public override void OnOverlapWith(GameObject other, float radius)
        {
            Loot.Loot lootObject = other.GetComponent<Loot.Loot>();
            if (lootObject != null)
            {
                this.OnLootCollided(lootObject);
            }
        }

        /// <inheritdoc />
        protected override Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            // TODO: Optimize this
            ShipComponent[] components = this.ShipRoot.Hull.GetGeneratedComponent(artillery);
            List<Shooter> evtArtillery = new List<Shooter>();
            foreach (ShipComponent component in components)
            {
                evtArtillery.Add(((ShipArtillery)component).Shooter);
            }
            return evtArtillery.ToArray();
        }

        /// <inheritdoc />
        public override ShipFigurehead GetFigurehead()
        {
            ShipComponent[] figureheads = this.ShipRoot.Hull.GetGeneratedComponent(ShipData.ComponentType.Figurehead);
            return figureheads.Length > 0 ? (ShipFigurehead)figureheads[0] : null;
        }

        /// <inheritdoc />
        protected override bool OnPreDestroy()
        {
            // TODO: Do base, and return to menu (always wait for x seconds, so level loads and the animation can play)

            this.Health = this.StatBlock.Health;

            Transform spawn = GameManager.Instance.PlayerSpawn;
            this.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            this.Physics.RotationPosition = spawn.rotation;
            this.Physics.LinearPosition = spawn.position;

            return false;
        }

        /// <inheritdoc />
        protected override void SpawnLoot(Vector3 position)
        {
        }

        /// <summary>
        /// Called when this object collides with loot.
        /// Adds loot to the <see cref="PlayerData"/> and updates UI/UX information.
        /// </summary>
        /// <param name="loot"></param>
        public void OnLootCollided(Loot.Loot loot)
        {
            this.PlayerData.Inventory.Add(loot.Item);

            // TODO: do this through event?
            this.ShipRoot.Hull.GenerateLoot(loot.LootPrefabWithoutSail);

            GameManager.Events.Dispatch(new EventLootCollected(this, loot));
            
            // destroy the loot
            Destroy(loot.gameObject);
        }

    }

}
