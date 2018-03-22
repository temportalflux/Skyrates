using System.Collections;
using Skyrates.Data;
using Skyrates.Game;
using Skyrates.Game.Event;
using Skyrates.Physics;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// The ship that a player controls.
    /// </summary>
    public class EntityPlayerShip : EntityShip
    {

        /// <summary>
        /// The network safe data which has information about the modular-ship.
        /// </summary>
        [HideInInspector]
        public ShipData ShipData;

        /// <summary>
        /// The non-networked data local to the player.
        /// </summary>
        [Header("Player: Data")]
        public PlayerData PlayerData;

        /// <summary>
        /// The Ship component which creates the modular ship.
        /// </summary>
        public ShipGenerator ShipGeneratorRoot;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            this.ShipGeneratorRoot.Destroy();
            this.ShipData = this.ShipGeneratorRoot.Generate();
            this.PlayerData.Init();
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.PlayerData.Init();
        }

        /// <inheritdoc />
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            GameManager.Events.Dispatch(new EventEntityPlayerShip(GameEventID.PlayerMoved, this));
        }

        /// <inheritdoc />
        protected override void ApplyRotations(PhysicsData physics, float deltaTime)
        {
            this.Physics.MoveRotation(physics.RotationPosition);
            this.GetRender().transform.localRotation = physics.RotationAesteticPosition;
        }

        #region Loot

        /// <inheritdoc />
        public override void OnOverlapWith(GameObject other, float radius)
        {
            Loot.Loot lootObject = other.GetComponent<Loot.Loot>();
            if (lootObject != null)
            {
                this.OnLootCollided(lootObject);
            }
        }

        /// <summary>
        /// Called when this object collides with loot.
        /// Adds loot to the <see cref="PlayerData"/> and updates UI/UX information.
        /// </summary>
        /// <param name="loot"></param>
        public void OnLootCollided(Loot.Loot loot)
        {
            this.PlayerData.Inventory.Add(loot.Item); //Must be before generating loot.

            // TODO: Change AddLoot to OnLootChanged
            this.GetHull().AddLoot(loot.LootPrefabWithoutSail, loot.Item);

            GameManager.Events.Dispatch(new EventLootCollected(this, loot));

            // destroy the loot
            Destroy(loot.gameObject);
        }

        #endregion

        #region Destruction

        /// <inheritdoc />
        protected override bool OnPreDestroy()
        {
            // TODO: Do base, and return to menu (always wait for x seconds, so level loads and the animation can play)

            this.Health = this.Hull.MaxHealth;

            Transform spawn = GameManager.Instance.PlayerSpawn;
            this.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            this.PhysicsData.RotationPosition = spawn.rotation;
            this.PhysicsData.LinearPosition = spawn.position;

            return false;
        }

        /// <inheritdoc />
        protected override void SpawnLoot(Vector3 position)
        {
        }
        
        #endregion

    }

}
