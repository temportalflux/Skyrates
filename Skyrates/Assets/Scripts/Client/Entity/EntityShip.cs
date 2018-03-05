using System;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Mono;
using Skyrates.Client.Ship;
using Skyrates.Common.Entity;
using Skyrates.Common.Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.Client.Entity
{

    /// <summary>
    /// A moving entity which has ShipStats, AI, health data, and <see cref="ShipComponent"/>s.
    /// </summary>
    public class EntityShip : EntityAI
    {

        [SerializeField]
        public ShipStat StatBlock;

        [Serializable]
        public class ParticleArea
        {

            [SerializeField]
            public BoxCollider Bounds;

            [SerializeField]
            public ParticleSystem Prefab;

            [SerializeField]
            public float Scale;

            [HideInInspector]
            public ParticleSystem Generated;

        }

        [SerializeField]
        public ParticleArea SmokeData;

        [SerializeField]
        public ParticleArea FireData;

        [SerializeField]
        public GameObject ParticleOnDestruction;

        [SerializeField]
        public float LootDropRadius;

        // TODO: Attribute to DISABLE in inspector http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
        [HideInInspector]
        public float Health;

        protected override void Start()
        {
            base.Start();
            if (this.StatBlock != null)
            {
                Debug.Assert(this.StatBlock.Health > 0, string.Format(
                    "StatBlock {0} has 0 health, they will be killed on first hit, so at least make this a 1 pls.",
                    this.StatBlock.name));
                this.Health = this.StatBlock.Health;
            }
            this.InitParticle(ref this.SmokeData);
            this.InitParticle(ref this.FireData);
        }
        
        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            this.UpdateHealthParticles();
        }

        private void InitParticle(ref ParticleArea area)
        {
            if (area == null || area.Bounds == null || area.Prefab == null)
                return;

            area.Generated = Instantiate(area.Prefab.gameObject,
                area.Bounds.transform.parent
            ).GetComponent<ParticleSystem>();
            area.Generated.transform.localPosition = area.Bounds.transform.localPosition;
            area.Generated.transform.localRotation = area.Bounds.transform.localRotation;
            
            ParticleSystem.ShapeModule shape = area.Generated.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = Vector3.Scale(area.Bounds.size, area.Bounds.transform.lossyScale);
            
            ParticleSystem.MainModule main = area.Generated.main;
            ParticleSystem.MinMaxCurve size = main.startSize;
            size.constant = area.Scale;
            main.startSize = size;
        }

        // Called when some non-trigger collider with a rigidbody enters
        /// <inheritdoc />
        public virtual void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Projectile-Enemy") && this.CompareTag("Enemy")) || (other.CompareTag("Projectile-Player") && this.CompareTag("Player")))
            {
                return;
            }

            EntityProjectile entityProjectile = other.GetComponent<EntityProjectile>();
            if (entityProjectile != null)
            {
                this.TakeDamage(entityProjectile, entityProjectile.GetDamage());

                GameManager.Events.Dispatch(new EventEntityShipHitByProjectile(this, entityProjectile));

                // collider is a projectile
                // TODO: Owner should destroy based on networking
                Destroy(entityProjectile.gameObject);
            }

            ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
            if (ram != null)
            {
                // If ram, and still have health, tell source that ram attack wasn't fully successful
                if (this.TakeDamage(ram.Ship, ram.GetDamage()) > 0)
                {
                    ram.Ship.OnRamUnsucessful(this);
                }
                GameManager.Events.Dispatch(new EventEntityShipHitByRam(this, ram));
            }

        }

        public virtual void OnTriggerExit(Collider other)
        {
            
        }

        /// <summary>
        /// Returns the ramming component of the ship, null if none exists.
        /// </summary>
        /// <returns></returns>
        public virtual ShipFigurehead GetFigurehead()
        {
            return null;
        }

        /// <summary>
        /// Called when the target still has health after ram.
        /// Causes the owner to take some amount of damage (currently 2).
        /// </summary>
        /// <param name="target"></param>
        protected virtual void OnRamUnsucessful(EntityShip target)
        {
            // TODO: Calculate the damage to take when ramming is unsuccesful.
            this.TakeDamage(target, 2);
        }

        /// <summary>
        /// Called to cause damage to the ship.
        /// </summary>
        /// <param name="source">The entity which is causing the damage.</param>
        /// <param name="damage">The amount of damage to take.</param>
        /// <returns>The amount of health remaining after the attack</returns>
        public virtual float TakeDamage(Common.Entity.Entity source, float damage)
        {
            // Remove the damage from the health
            this.Health -= damage;

            // Update particle effects
            this.UpdateHealthParticles();
            
            // If we still have health, stop execution here
            if (this.Health > 0) return this.Health;
            
            // No more health, so try to destroy

            // Get whether or not this object should be destroyed
            if (this.OnPreDestroy())
            {
                // Destroy em
                Destroy(this.gameObject);
            }

            // Return that there is no health left - we are dead
            return 0;
        }

        /// <summary>
        /// Called right before a ship is destroyed via <see cref="TakeDamage(Entity, float)"/>
        /// </summary>
        /// <returns>True if the object can be destroyed</returns>
        protected virtual bool OnPreDestroy()
        {
            // Spawn particles for the object being destroyed
            this.SpawnDestructionParticles();

            // Spawn loot
            // NOTE: Since this function is called by the owning client, then loot is spawned on owning client
            // TODO: Sync loot - make loot entity static
            this.SpawnLoot(this.transform.position);

            return true;
        }

        /// <summary>
        /// Spawns an orphan prefab which contains a particle system for an explosion when this ship has been destoryed.
        /// The orphan is destroyed when it is done playing its particles.
        /// </summary>
        protected virtual void SpawnDestructionParticles()
        {
            if (this.ParticleOnDestruction == null) return;

            // Spawn the prefab WITH NO OWNER (so it isnt destroyed when the object is)
            GameObject particles = Instantiate(this.ParticleOnDestruction,
                this.transform.position, this.transform.rotation);
            particles.transform.localScale = Vector3.Scale(particles.transform.localScale, this.transform.localScale);
            // Destory the particle system when it is done playing
            Destroy(particles.gameObject, particles.GetComponentInChildren<ParticleSystem>().main.duration);
        }

        /// <summary>
        /// Generates/Spawns loot according to the loot table in <see cref="StatBlock"/>.
        /// </summary>
        /// <param name="position"></param>
        protected virtual void SpawnLoot(Vector3 position)
        {
            if (this.StatBlock == null || this.StatBlock.Loot == null) return;

            // Generate the loot to spawn
            KeyValuePair<ShipData.BrokenComponentType, Loot.Loot>[] loots = this.StatBlock.Loot.Generate();
            // Spawn each loot in turn
            foreach (KeyValuePair<ShipData.BrokenComponentType, Loot.Loot> lootItem in loots)
            {
                // If the loot is invalid in some way, discard
                if (lootItem.Key == ShipData.BrokenComponentType.Invalid || lootItem.Value == null)
                {
                    continue;
                }

                // Get a random position to spawn it
                Vector3 pos = position + Vector3.Scale(Random.insideUnitSphere, Vector3.one * this.LootDropRadius);

                // Create the prefab instance for the loot
                Loot.Loot loot = Instantiate(lootItem.Value.gameObject, pos, Quaternion.identity).GetComponent<Loot.Loot>();
                // Set the item the loot contains
                loot.Item = lootItem.Key;
                // TODO: Loot event, loot should be static entity for networking
            }
        }

        /// <summary>
        /// Returns a list of shooters for some artillery component. Null if none exist.
        /// </summary>
        /// <param name="artillery"></param>
        /// <returns></returns>
        protected virtual Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return null;
        }

        /// <summary>
        /// Causes all the shooters from <see cref="GetArtilleryShooters"/> to fire for some artillery component.
        /// </summary>
        /// <param name="artillery"></param>
        public void Shoot(ShipData.ComponentType artillery)
        {
            this.Shoot(artillery, (shooter => shooter.GetProjectileDirection().normalized));
        }

        /// <summary>
        /// Causes all the shooters from <see cref="GetArtilleryShooters"/> to fire for some artillery component.
        /// </summary>
        /// <param name="artillery"></param>
        public void Shoot(ShipData.ComponentType artillery, Vector3 allDirection)
        {
            allDirection.Normalize();
            this.Shoot(artillery, (shooter => allDirection));
        }

        /// <summary>
        /// Causes all the shooters from <see cref="GetArtilleryShooters"/> to fire for some artillery component.
        /// </summary>
        /// <param name="artillery"></param>
        public void Shoot(ShipData.ComponentType artillery, Func<Shooter, Vector3> getDirection)
        {
            // TODO: Optimize this
            Shooter[] shooters = this.GetArtilleryShooters(artillery);

            if (shooters == null || shooters.Length <= 0)
                return;

            // Tell each shooter to fire
            foreach (Shooter shooter in shooters)
            {
                shooter.FireProjectile(getDirection(shooter), this.Physics.LinearVelocity);
            }

            // Dispatch event for the shooters as a whole.
            // TODO: Change event name to only be for the player
            GameManager.Events.Dispatch(new EventArtilleryFired(this, shooters, artillery));
        }

        /// <summary>
        /// Updates the diagetic particles based on current health.
        /// </summary>
        protected virtual void UpdateHealthParticles()
        {
            if (this.StatBlock == null) return;

            // get the amount of damage currently taken (diff in health vs max health)
            float damageTaken = this.StatBlock.Health - this.Health;

            // Update smoke particles
            if (this.SmokeData.Generated != null)
            {
                float emittedAmountSmoke = 0;
                // if the damage taken is in the range, when set emittedAmountSmoke
                if (damageTaken >= this.StatBlock.HealthFeedbackData.SmokeDamage.x &&
                    damageTaken <= this.StatBlock.HealthFeedbackData.SmokeDamage.y)
                {
                    // lots o math
                    float scaled = (damageTaken - this.StatBlock.HealthFeedbackData.SmokeDamage.x) /
                                   (this.StatBlock.HealthFeedbackData.SmokeDamage.y - this.StatBlock.HealthFeedbackData.SmokeDamage.x);
                    emittedAmountSmoke =
                        scaled * (this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.y -
                                  this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.x) +
                        this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.x;
                }

                // set the emission rate
                ParticleSystem.EmissionModule emissionSmoke = this.SmokeData.Generated.emission;
                emissionSmoke.rateOverTime = emittedAmountSmoke;
            }

            // Update fire particles
            if (this.FireData.Generated != null)
            {
                float emiitedAmountFire = 0;
                // if the damage taken is in the range, when set emiitedAmountFire
                if (damageTaken >= this.StatBlock.HealthFeedbackData.FireDamage.x &&
                    damageTaken <= this.StatBlock.HealthFeedbackData.FireDamage.y)
                {
                    // lots o math II
                    float scaled = (damageTaken - this.StatBlock.HealthFeedbackData.FireDamage.x) /
                                   (this.StatBlock.HealthFeedbackData.FireDamage.y - this.StatBlock.HealthFeedbackData.FireDamage.x);
                    emiitedAmountFire =
                        scaled * (this.StatBlock.HealthFeedbackData.FireEmissionAmount.y -
                                  this.StatBlock.HealthFeedbackData.FireEmissionAmount.x) +
                        this.StatBlock.HealthFeedbackData.FireEmissionAmount.x;
                }

                // set the emission rate
                ParticleSystem.EmissionModule emission = this.FireData.Generated.emission;
                emission.rateOverTime = emiitedAmountFire;
            }

        }
    }
}
