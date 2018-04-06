using System;
using System.Collections;
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
        public float Health;

        #region Parts of the ship

        /// <summary>
        /// The root of the render object (must be a child/decendent of this root).
        /// </summary>
        [Header("Ship: Objects")]
        [Tooltip("The root of the render object (must be a child/decendent of this root).")]
        public Transform Render;

        public ShipHull Hull;

        #endregion
        
        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            if (this.Hull != null)
            {
                Debug.Assert(this.Hull.HP > 0, string.Format(
                    "Hull {0} has 0 health, they will be killed on first hit, so at least make this a 1 pls.",
                    this.Hull.name));
                this.Health = this.Hull.HP;
            }
            StartCoroutine(this.AutoHeal());
        }
        
        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            this.Hull.UpdateHealthParticles(this.Health);
        }

        /// <inheritdoc />
        public override Transform GetRender()
        {
            return this.Render;
        }

        protected override void UpdateBehaviorData()
        {
            base.UpdateBehaviorData();
            this.DataBehavior.ThrustMultiplier = this.GetHull().GetMultiplierThrust();
            this.DataBehavior.TurnSpeedMultiplier = this.GetHull().GetMultiplierTurnSpeed();
        }

        /// <inheritdoc />
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
        
        #region Health
        
        /// <summary>
        /// Auto heals the player ship every 5 secodns while the health is less than max health.
        /// </summary>
        /// <returns></returns>
        IEnumerator AutoHeal()
        {
            if (this.Hull == null || this.Hull.HealthRegenAmount <= 0.0f) yield break;
            while (true)
            {
                yield return new WaitUntil((() => this.Health < this.Hull.HP));
                while (this.Health < this.Hull.HP)
                {
                    this.Health += this.Hull.HealthRegenAmount;
                    yield return new WaitForSeconds(this.Hull.HealthRegenDelay);
                }
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
                    Entity source = entityProjectile.Shooter.Owner != null ? entityProjectile.Shooter.Owner.Ship : null;
                    this.TakeDamage(source ?? entityProjectile, damage);

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
        public virtual float TakeDamage(Entity source, float damage)
        {
            // Remove the damage from the health
            this.Health -= damage;

            // Update particle effects
            this.Hull.UpdateHealthParticles(this.Health);

            if (source != this && source is EntityAI)
            {
                this.OnDamagedBy((EntityAI)source);
            }

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

        protected virtual void OnDamagedBy(EntityAI source)
        {
            if (this.DataBehavior.Formation != null)
            {
                this.DataBehavior.Formation.OnDamagedBy(source);
            }
            if (this.FormationOwner != null)
            {
                this.FormationOwner.OnDamagedBy(null, source);
            }
        }

        /// <summary>
        /// Called when the target still has health after ram.
        /// Causes the owner to take some amount of damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage">The amount of damage that was caused to the target</param>
        public virtual void OnRamUnsucessful(Entity target, float damage)
        {
            // Calculate the damage to take when ramming is unsuccesful.
            this.TakeDamage(target, CaclulateRecoil(damage));

            this.ForceStop();
        }

        protected virtual void ForceStop()
        {
            this.PhysicsData.LinearVelocity = Vector3.zero;
            this.PhysicsData.LinearAccelleration = Vector3.zero;
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
            this.Hull.SpawnDestructionParticles();

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
        protected virtual ShipArtillery[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            // TODO: Optimize this
            ShipComponent[] components = this.GetHull().GetComponent(artillery);
            if (components == null) return new ShipArtillery[0];
            List<ShipArtillery> evtArtillery = new List<ShipArtillery>();
            foreach (ShipComponent component in components)
            {
                evtArtillery.Add((ShipArtillery)component);
            }
            return evtArtillery.ToArray();
        }

        /// <summary>
        /// Causes all the shooters from <see cref="GetArtilleryShooters"/> to fire for some artillery component.
        /// </summary>
        /// <param name="artillery"></param>
        public void Shoot(ShipData.ComponentType artillery)
        {
            this.Shoot(artillery, (shooter => shooter.Shooter.GetProjectileDirection().normalized));
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
        public void Shoot(ShipData.ComponentType artillery, Func<ShipArtillery, Vector3> getDirection)
        {
            // TODO: Optimize this
            ShipArtillery[] shooters = this.GetArtilleryShooters(artillery);

            if (shooters == null || shooters.Length <= 0)
                return;

            // Tell each shooter to fire
            foreach (ShipArtillery shooter in shooters)
            {
                shooter.Shoot(getDirection, this.PhysicsData.LinearVelocity);
            }

            // Dispatch event for the shooters as a whole.
            // TODO: Change event name to only be for the player
            GameManager.Events.Dispatch(new EventArtilleryFired(this, shooters, artillery));
        }

        #endregion

    }
}
