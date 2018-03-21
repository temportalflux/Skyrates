using System;
using System.Collections.Generic;
using Skyrates.Game;
using Skyrates.Game.Event;
using Skyrates.Mono;
using Skyrates.Physics;
using Skyrates.Ship;
using UnityEngine;

namespace Skyrates.Entity
{

    /// <summary>
    /// A moving entity which has ShipStats, AI, health data, and <see cref="ShipComponent"/>s.
    /// </summary>
    public abstract class EntityShip : EntityAI
    {
        
        // TODO: Attribute to DISABLE in inspector http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
        [HideInInspector]
        public float Health;

        #region Parts of the ship

        /// <summary>
        /// The root of the render object (must be a child/decendent of this root).
        /// </summary>
        [Header("Objects")]
        [Tooltip("The root of the render object (must be a child/decendent of this root).")]
        public Transform Render;

        public ShipHull Hull;

        #endregion

        #region Particles

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

        [Header("Particles")]
        [SerializeField]
        public ParticleArea SmokeData;

        [SerializeField]
        public ParticleArea FireData;

        [SerializeField]
        public GameObject ParticleOnDestruction;

        #endregion
        
        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            if (this.Hull != null)
            {
                Debug.Assert(this.Hull.MaxHealth > 0, string.Format(
                    "Hull {0} has 0 health, they will be killed on first hit, so at least make this a 1 pls.",
                    this.Hull.name));
                this.Health = this.Hull.MaxHealth;
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

        /// <inheritdoc />
        public override Transform GetRender()
        {
            return this.Render;
        }

        protected override void ApplyRotations(PhysicsData physics, float deltaTime)
        {
            this.Physics.MoveRotation(physics.RotationPosition);
            this.GetRender().localRotation = physics.RotationAesteticPosition;
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            this.TryCalculateDamage(other);
        }

        public virtual void OnTriggerExit(Collider other)
        {
        }

        #region Components

        /// <summary>
        /// Returns the hull of the ship
        /// </summary>
        /// <returns></returns>
        public ShipHull GetHull()
        {
            return this.Hull;
        }
        
        /// <summary>
        /// Returns a list of components in the ship's hull of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ShipComponent[] GetShipComponentsOfType(ShipData.ComponentType type)
        {
            return this.GetHull() != null ? this.GetHull().GetComponent(type) : new ShipComponent[0];
        }

        /// <summary>
        /// Returns the ramming component of the ship, null if none exists.
        /// </summary>
        /// <returns></returns>
        public virtual ShipFigurehead GetFigurehead()
        {
            ShipHull hull = this.GetHull();
            ShipComponent[] comps = hull != null ? hull.GetComponent(ShipData.ComponentType.Figurehead) : null;
            return comps != null && comps.Length > 0 ? comps[0] as ShipFigurehead : null;
        }
        
        #endregion

        #region Particles

        /// <summary>
        /// Sets up a particle area to follow the size of this ship
        /// </summary>
        /// <param name="area"></param>
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
        /// Updates the diagetic particles based on current health.
        /// </summary>
        protected virtual void UpdateHealthParticles()
        {
            // TODO: Move this to hull
            if (this.Hull == null) return;

            // get the amount of damage currently taken (diff in health vs max health)
            float damageTaken = this.Hull.MaxHealth - this.Health;

            // Update smoke particles
            if (this.SmokeData.Generated != null)
            {
                float emittedAmountSmoke = 0;
                // if the damage taken is in the range, when set emittedAmountSmoke
                if (damageTaken >= this.Hull.HealthFeedbackData.SmokeDamage.x &&
                    damageTaken <= this.Hull.HealthFeedbackData.SmokeDamage.y)
                {
                    // lots o math
                    float scaled = (damageTaken - this.Hull.HealthFeedbackData.SmokeDamage.x) /
                                   (this.Hull.HealthFeedbackData.SmokeDamage.y - this.Hull.HealthFeedbackData.SmokeDamage.x);
                    emittedAmountSmoke =
                        scaled * (this.Hull.HealthFeedbackData.SmokeEmissionAmount.y -
                                  this.Hull.HealthFeedbackData.SmokeEmissionAmount.x) +
                        this.Hull.HealthFeedbackData.SmokeEmissionAmount.x;
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
                if (damageTaken >= this.Hull.HealthFeedbackData.FireDamage.x &&
                    damageTaken <= this.Hull.HealthFeedbackData.FireDamage.y)
                {
                    // lots o math II
                    float scaled = (damageTaken - this.Hull.HealthFeedbackData.FireDamage.x) /
                                   (this.Hull.HealthFeedbackData.FireDamage.y - this.Hull.HealthFeedbackData.FireDamage.x);
                    emiitedAmountFire =
                        scaled * (this.Hull.HealthFeedbackData.FireEmissionAmount.y -
                                  this.Hull.HealthFeedbackData.FireEmissionAmount.x) +
                        this.Hull.HealthFeedbackData.FireEmissionAmount.x;
                }

                // set the emission rate
                ParticleSystem.EmissionModule emission = this.FireData.Generated.emission;
                emission.rateOverTime = emiitedAmountFire;
            }

        }

        #endregion
        
        #region Damage

        /// <summary>
        /// Calculates how much damage should be dealt.
        /// </summary>
        /// <param name="baseAtk">The amount of base damage the source says it should deal</param>
        /// <param name="defense">How much initial damage is taken away from the attack (damage = baseAtk - defense)</param>
        /// <param name="protection">How much of the damage should actually be taken (damage = protection * damage). Must be a number between 0.0 and 100.0</param>
        /// <returns></returns>
        private static float CalculateDamage(float baseAtk, float defense, float protection)
        {
            // TODO: Add penetration (attack multiplier, the opposite of protection) if wanted.
            // TODO: Add critical hit if wanted.

            // The more protection there is, the lower the percentage of total damage should be.
            // Therefore protection must be serialized from range of [0, 100] to [1, 0]
            // Clamp protection to range [0, 100] - ensure protection is in the expected range
            protection = Mathf.Clamp(protection, 0.0f, 100.0f);
            // Serialize the range from [0, 100] to [0, 1]
            protection /= 100.0f;
            // Reverse the range from [0, 1] to [1, 0]
            protection = 1.0f - protection;

            // Subtract defense from attack
            float damage = (baseAtk - defense);
            // Can't take less than 1 damage.
            damage = Mathf.Max(damage, 1.0f);
            // and multiply the total by the protection formula.
            damage *= protection;

            return damage;
        }

        /// <summary>
        /// Calculates how much damage should be taken if an entity has dealt x damage without killing its target.
        /// </summary>
        /// <param name="damage">The amount of damage dealt</param>
        /// <returns></returns>
        public static float CaclulateRecoil(float damage)
        {
            // MAGIC: Constant modifier indicating that the damage we take is 1% of the damage we dealt
            float recoilMultiplier = 0.01f;
            // minimum damage is 1hp
            return Mathf.Max(damage * recoilMultiplier, 1.0f);
        }

        /// <summary>
        /// Will attempt to take damage from the collider's scripts.
        /// Based on if its a projectile or figurehead.
        /// </summary>
        /// <param name="other"></param>
        private void TryCalculateDamage(Collider other)
        {
            // Check to ensure that sources and their projectiles don't collide
            // downside is that enemies cannot be hit by any projectile that came from an enemy
            if ((other.CompareTag("Projectile-Enemy") && this.CompareTag("Enemy")) ||
                (other.CompareTag("Projectile-Player") && this.CompareTag("Player")))
            {
                return;
            }

            float damage = 0;

            // Projectile calculations
            {
                EntityProjectile entityProjectile = other.GetComponent<EntityProjectile>();
                if (entityProjectile != null)
                {
                    // Calculate the total damage
                    damage = CalculateDamage(
                        entityProjectile.GetAttack(),
                        this.GetHull() ? this.GetHull().GetDefense() : 0.0f,
                        this.GetHull() ? this.GetHull().GetProtection() : 0.0f
                    );

                    // Take the necessary damage
                    this.TakeDamage(entityProjectile, damage);

                    // Dispatch event for hit by projectile
                    GameManager.Events.Dispatch(new EventEntityShipHitByProjectile(this, entityProjectile, damage));

                    // collider is a projectile
                    Destroy(entityProjectile.gameObject);
                }
            }

            // Figurehead calculations
            {
                ShipFigurehead figurehead = other.GetComponent<ShipFigurehead>();
                if (figurehead != null)
                {
                    // Calculate the total damage
                    damage = CalculateDamage(
                        figurehead.GetAttack(),
                        this.GetHull() ? this.GetHull().GetDefense() : 0.0f,
                        this.GetHull() ? this.GetHull().GetProtection() : 0.0f
                    );

                    // If ram, and still have health, tell source that ram attack wasn't fully successful
                    if (this.TakeDamage(figurehead.Ship, damage) > 0)
                    {
                        // We know that the health taken was our full amount because our health is not 0, so we don't need to recalculate the amount of damage we actually took.
                        figurehead.Ship.OnRamUnsucessful(this, damage);
                    }
                    GameManager.Events.Dispatch(new EventEntityShipHitByRam(this, figurehead, damage));
                }
            }
        }

        /// <summary>
        /// Called to cause damage to the ship.
        /// </summary>
        /// <param name="source">The entity which is causing the damage.</param>
        /// <param name="damage">The amount of damage to take.</param>
        /// <returns>The amount of health remaining after the attack</returns>
        public virtual float TakeDamage(Skyrates.Entity.Entity source, float damage)
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
        /// Called when the target still has health after ram.
        /// Causes the owner to take some amount of damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage">The amount of damage that was caused to the target</param>
        protected virtual void OnRamUnsucessful(EntityShip target, float damage)
        {
            // Calculate the damage to take when ramming is unsuccesful.
            this.TakeDamage(target, CaclulateRecoil(damage));
        }

        #endregion

        #region Destruction

        /// <summary>
        /// Called right before a ship is destroyed via <see cref="TakeDamage(Entity, float)"/>
        /// </summary>
        /// <returns>True if the object can be destroyed</returns>
        protected virtual bool OnPreDestroy()
        {
            // Spawn particles for the object being destroyed
            this.SpawnDestructionParticles();

            // Spawn loot
            this.SpawnLoot(this.transform.position);

            return true;
        }

        /// <summary>
        /// Generates/Spawns loot according to the loot table in <see cref="StatBlock"/>.
        /// </summary>
        /// <param name="position"></param>
        protected virtual void SpawnLoot(Vector3 position)
        {

        }

        #endregion

        #region Shooting
        
        /// <summary>
        /// Returns a list of shooters for some artillery component. Null if none exist.
        /// </summary>
        /// <param name="artillery"></param>
        /// <returns></returns>
        protected virtual Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            // TODO: Optimize this
            ShipComponent[] components = this.GetHull().GetComponent(artillery);
            if (components == null) return new Shooter[0];
            List<Shooter> evtArtillery = new List<Shooter>();
            foreach (ShipComponent component in components)
            {
                evtArtillery.Add(((ShipArtillery)component).Shooter);
            }
            return evtArtillery.ToArray();
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
                shooter.FireProjectile(getDirection(shooter), this.PhysicsData.LinearVelocity);
            }

            // Dispatch event for the shooters as a whole.
            // TODO: Change event name to only be for the player
            GameManager.Events.Dispatch(new EventArtilleryFired(this, shooters, artillery));
        }

        #endregion

    }
}
