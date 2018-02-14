using System;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Loot;
using Skyrates.Client.Ship;
using Skyrates.Common.Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skyrates.Common.Entity
{

    public class EntityShip : EntityDynamic
    {

        [SerializeField]
        public ShipStat StatBlock;

        [SerializeField]
        public ParticleSystem ParticleSmoke;

        [SerializeField]
        public ParticleSystem ParticleFire;

        [SerializeField]
        public ParticleSystem ParticleOnDestruction;

        // TODO: Attribute to DISABLE in inspector http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
        [BitSerialize(0)]
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
        }

        protected override void Update()
        {
            base.Update();
            this.UpdateHealthParticles();
        }

        // Called when some non-trigger collider with a rigidbody enters
        protected virtual void OnTriggerEnter(Collider other)
        {
            EntityProjectile entityProjectile = other.GetComponent<EntityProjectile>();
            if (entityProjectile != null)
            {
                GameManager.Events.Dispatch(new EventEntityShipHitByProjectile(this, entityProjectile));
                // collider is a projectile
                // TODO: Owner should destroy based on networking
                Destroy(entityProjectile.gameObject);
            }

            ShipFigurehead ram = other.GetComponent<ShipFigurehead>();
            if (ram != null)
            {
                GameManager.Events.Dispatch(new EventEntityShipHitByRam(this, ram));
            }

        }

        // called by network interface events which originate from OnTriggerEnter
        public virtual void TakeDamage(float damage)
        {
            this.Health -= damage;

            GameManager.Events.Dispatch(new EventEntityShipDamaged(this, damage));

            if (this.Health > 0) return;

            this.UpdateHealthParticles();

            if (this.OnPreDestroy())
            {
                // Destroy em
                Destroy(this.gameObject);
            }

        }

        protected virtual bool OnPreDestroy()
        {

            this.SpawnDestructionParticles();

            // Spawn loot
            // NOTE: Since this function is called by the owning client, then loot is spawned on owning client
            // TODO: Sync loot - make loot entity static
            this.SpawnLoot(this.transform.position);

            return true;
        }

        protected virtual void SpawnDestructionParticles()
        {
            if (this.ParticleOnDestruction != null)
            {
                ParticleSystem particles = Instantiate(this.ParticleOnDestruction.gameObject,
                    this.transform.position, this.transform.rotation).GetComponent<ParticleSystem>();
                Destroy(particles.gameObject, particles.main.duration);
            }
        }

        protected virtual void SpawnLoot(Vector3 position)
        {
            ShipComponent[] loots = this.StatBlock.Loot.Generate();
            foreach (ShipComponent lootItem in loots)
            {
                if (lootItem == null)
                {
                    continue;
                }

                Vector3 pos = position + Random.insideUnitSphere * this.StatBlock.LootRadius;
                Loot loot = Instantiate(
                    this.StatBlock.LootPrefab.gameObject, pos, Quaternion.identity).GetComponent<Loot>();
                loot.Item = lootItem;
                // TODO: Loot event, loot should be static entity
            }
        }

        protected virtual Shooter[] GetArtilleryShooters(ShipData.ComponentType artillery)
        {
            return null;
        }

        public void Shoot(ShipData.ComponentType artillery)
        {
            // TODO: Optimize this
            Shooter[] shooters = this.GetArtilleryShooters(artillery);

            if (shooters == null || shooters.Length <= 0)
                return;

            foreach (Shooter shooter in shooters)
            {
                shooter.FireProjectile(shooter.ProjectileDirection().normalized, this.Physics.LinearVelocity);
            }

            GameManager.Events.Dispatch(new EventArtilleryFired(this, shooters));
        }

        protected virtual void UpdateHealthParticles()
        {
            if (this.StatBlock == null) return;

            float damageTaken = this.StatBlock.Health - this.Health;

            if (this.ParticleSmoke != null)
            {
                float emiitedAmountSmoke = 0;
                if (damageTaken >= this.StatBlock.HealthFeedbackData.SmokeDamage.x &&
                    damageTaken <= this.StatBlock.HealthFeedbackData.SmokeDamage.y)
                {
                    float scaled = (damageTaken - this.StatBlock.HealthFeedbackData.SmokeDamage.x) /
                                   (this.StatBlock.HealthFeedbackData.SmokeDamage.y - this.StatBlock.HealthFeedbackData.SmokeDamage.x);
                    emiitedAmountSmoke =
                        scaled * (this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.y -
                                  this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.x) +
                        this.StatBlock.HealthFeedbackData.SmokeEmissionAmount.x;
                }

                ParticleSystem.EmissionModule emissionSmoke = this.ParticleSmoke.emission;
                emissionSmoke.rateOverTime = emiitedAmountSmoke;
            }

            if (this.ParticleFire != null)
            {
                float emiitedAmountFire = 0;
                if (damageTaken >= this.StatBlock.HealthFeedbackData.FireDamage.x &&
                    damageTaken <= this.StatBlock.HealthFeedbackData.FireDamage.y)
                {
                    float scaled = (damageTaken - this.StatBlock.HealthFeedbackData.FireDamage.x) /
                                   (this.StatBlock.HealthFeedbackData.FireDamage.y - this.StatBlock.HealthFeedbackData.FireDamage.x);
                    emiitedAmountFire =
                        scaled * (this.StatBlock.HealthFeedbackData.FireEmissionAmount.y -
                                  this.StatBlock.HealthFeedbackData.FireEmissionAmount.x) +
                        this.StatBlock.HealthFeedbackData.FireEmissionAmount.x;
                }

                ParticleSystem.EmissionModule emission = this.ParticleFire.emission;
                emission.rateOverTime = emiitedAmountFire;
            }

        }
    }
}
