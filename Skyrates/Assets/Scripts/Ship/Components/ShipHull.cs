using System;
using System.Collections.Generic;
using Skyrates.Entity;
using UnityEngine;

namespace Skyrates.Ship
{

    /// <summary>
    /// Subclass of <see cref="ShipComponent"/> dedicated to
    /// components of type <see cref="ShipData.ComponentType.Hull"/>.
    /// </summary>
    public class ShipHull : ShipComponent
    {

        [Serializable]
        public class ComponentList
        {
            public ShipComponent[] Value;
        }

        [Tooltip("The amount of damage the ship can take")]
        public int MaxHealth;

        public float HealthRegenAmount;
        public float HealthRegenDelay;

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

            // TODO: https://docs.unity3d.com/ScriptReference/EditorGUILayout.CurveField.html

            // amount of damage taken at which the smoke starts at
            [SerializeField]
            public Vector2Int DamageRange;

            [SerializeField]
            public Vector2Int EmissionAmountRange;

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

        [Header("Components")]
        [SerializeField]
        public ComponentList[] Components = new ComponentList[ShipData.ComponentTypes.Length];

        [Header("Loot on Ship")]
        [SerializeField]
        public Transform[] LootMounts;

        /// <summary>
        /// All the loot on the deck.
        /// TODO: Generate loot onto ships which are carrying (loot is random gen on spawn not death).
        /// </summary>
        private readonly List<GameObject> GeneratedLoot = new List<GameObject>();

        void Start()
        {
            this.InitParticle(ref this.SmokeData);
            this.InitParticle(ref this.FireData);
        }

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
        public void SpawnDestructionParticles()
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
        public void UpdateHealthParticles(float health)
        {
            // get the amount of damage currently taken (diff in health vs max health)
            float damageTaken = this.MaxHealth - health;

            // Update smoke particles
            if (this.SmokeData.Generated != null)
            {
                float emittedAmountSmoke = 0;
                // if the damage taken is in the range, when set emittedAmountSmoke
                if (damageTaken >= this.SmokeData.DamageRange.x &&
                    damageTaken <= this.SmokeData.DamageRange.y)
                {
                    // lots o math
                    float scaled = (damageTaken - this.SmokeData.DamageRange.x) /
                                   (this.SmokeData.DamageRange.y - this.SmokeData.DamageRange.x);
                    emittedAmountSmoke =
                        scaled * (this.SmokeData.EmissionAmountRange.y -
                                  this.SmokeData.EmissionAmountRange.x) +
                        this.SmokeData.EmissionAmountRange.x;
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
                if (damageTaken >= this.FireData.DamageRange.x &&
                    damageTaken <= this.FireData.DamageRange.y)
                {
                    // lots o math II
                    float scaled = (damageTaken - this.FireData.DamageRange.x) /
                                   (this.FireData.DamageRange.y - this.FireData.DamageRange.x);
                    emiitedAmountFire =
                        scaled * (this.FireData.EmissionAmountRange.y -
                                  this.FireData.EmissionAmountRange.x) +
                        this.FireData.EmissionAmountRange.x;
                }

                // set the emission rate
                ParticleSystem.EmissionModule emission = this.FireData.Generated.emission;
                emission.rateOverTime = emiitedAmountFire;
            }

        }

        #endregion

        private void ForEeach<T>(ShipData.ComponentType componentType, Action<T> loop) where T: ShipComponent
        {
            foreach (ShipComponent comp in this.GetComponent(ShipData.ComponentType.HullArmor))
            {
                T compT = comp as T;
                if (compT != null)
                {
                    loop(compT);
                }
            }
        }

        public float GetMultiplierThrust()
        {
            float value = 0.0f;
            this.ForEeach<ShipPropulsion>(ShipData.ComponentType.Propulsion, (comp) =>
            {
                value *= comp.Thrust;
            });
            return value;
        }

        public float GetMultiplierTurnSpeed()
        {
            float value = 0.0f;
            this.ForEeach<ShipNavigation>(ShipData.ComponentType.NavigationLeft, (nav) =>
            {
                value *= nav.Maneuverability;
            });
            this.ForEeach<ShipNavigation>(ShipData.ComponentType.NavigationRight, (nav) =>
            {
                value *= nav.Maneuverability;
            });
            return value;
        }

        /// <summary>
        /// Gets the amount of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The amount of damage subtracted from damage taken</returns>
        public float GetDefense()
        {
            float value = 0.0f;
            this.ForEeach<ShipHullArmor>(ShipData.ComponentType.HullArmor, (hullArmor) =>
            {
                value += hullArmor.GetDefense();
            });
            return value;
        }

        /// <summary>
        /// Gets the percentage of damage subtracted from damage taken.
        /// </summary>
        /// <returns>The percentage of damage subtracted from damage taken</returns>
        public float GetProtection()
        {
            float value = 0.0f;
            this.ForEeach<ShipHullArmor>(ShipData.ComponentType.HullArmor, (hullArmor) =>
            {
                value *= hullArmor.GetProtection();
            });
            return value;
        }

        public void AddLoot(GameObject lootObjectPrefab, ShipData.BrokenComponentType item, bool forced = false)
        {
            int nextIndex = this.GeneratedLoot.Count;
            if (nextIndex < this.LootMounts.Length)
            {
                Transform mount = this.LootMounts[nextIndex];
                GameObject generated = Instantiate(lootObjectPrefab, this.transform);
                generated.transform.SetPositionAndRotation(mount.position, mount.rotation);
                generated.transform.localScale = mount.localScale;
                this.GeneratedLoot.Add(generated);
                EntityPlayerShip playerShip = this.Ship as EntityPlayerShip;
                if (playerShip)
                {
                    playerShip.PlayerData.Inventory.MapGeneratedLoot(item, generated, forced);
                }
            }
        }

        protected int GetComponentIndex(ShipData.ComponentType type)
        {
            return (int)type;
        }

        /// <summary>
        /// Returns the generated component list for a specific type.
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public ShipComponent[] GetComponent(ShipData.ComponentType compType)
        {
            return this.Components[this.GetComponentIndex(compType)].Value;
        }

    }

}
