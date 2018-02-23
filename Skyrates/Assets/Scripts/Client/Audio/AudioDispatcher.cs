
using System;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Client
{
 
    /// <summary>
    /// Observer which spawns audio sources based on game events
    /// </summary>
    public class AudioDispatcher : UnityEngine.MonoBehaviour
    {

        // TODO: Turn this into an editor thing with dictionaries
        /*
        [Serializable]
        public class AudioEvent
        {
            [SerializeField]
            public GameEventID EventID;
            [SerializeField]
            public AudioSource AudioPrefab;
        }

        [SerializeField]
        public AudioEvent[] AudioEvents;

        public readonly Dictionary<GameEventID, AudioSource> AudioEventPrefabs = new Dictionary<GameEventID, AudioSource>();

        void Awake()
        {
            foreach (AudioEvent audioEvent in AudioEvents)
            {
                this.AudioEventPrefabs[audioEvent.EventID] = audioEvent.AudioPrefab;
            }
        }
        */

        /// <summary>
        /// Audio played when artillery is fired
        /// </summary>
        public AudioSource AudioArtilleryFired;
        /// <summary>
        /// Audio played when a ship is hit by a projectile
        /// </summary>
        public AudioSource AudioEntityShipHitByProjectile;
        /// <summary>
        /// Audio played when a ship is hit by another ship's ram
        /// </summary>
        public AudioSource AudioEntityShipHitByRam;
        /// <summary>
        /// Audio played when a player collects loot
        /// </summary>
        public AudioSource AudioOnLootCollected;
        /// <summary>
        /// Audio played when enemies engage a player
        /// </summary>
        [Deprecated]
        public AudioSource AudioOnEnemyEngage;
        /// <summary>
        /// Audio played when enemies disengage a player
        /// </summary>
        [Deprecated]
        public AudioSource AudioOnEnemyDisengage;

        /// <summary>
        /// Plays a specific audio at a location.
        /// </summary>
        /// <param name="position">The position to spawn the prefab at</param>
        /// <param name="rotation">The rotation of the prefab</param>
        /// <param name="prefab">The prefab to instantiate</param>
        /// <param name="owner">The transform to which the prefab audio source is attatched to</param>
        private void CreateAudio(Vector3 position, Quaternion rotation, AudioSource prefab, Transform owner)
        {
            if (prefab != null)
            {
                // Create the thing
                AudioSource spawned = Instantiate(prefab.gameObject, position, rotation, owner).GetComponent<AudioSource>();
                // Kill it when it is done playing
                Destroy(spawned.gameObject, spawned.clip.length);
            }
        }
        
        /// <summary>
        /// Unity event - used to subscribe to game events
        /// </summary>
        void OnEnable()
        {
            GameManager.Events.ArtilleryFired += this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
            GameManager.Events.LootCollected += this.OnLootCollected;
            GameManager.Events.EnemyTargetEngage += this.OnEnemyEngage;
            GameManager.Events.EnemyTargetDisengage += this.OnEnemyEngage;
        }

        /// <summary>
        /// Unity event - used to unsubscribe to game events
        /// </summary>
        void OnDisable()
        {
            GameManager.Events.ArtilleryFired -= this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
            GameManager.Events.LootCollected -= this.OnLootCollected;
            GameManager.Events.EnemyTargetEngage -= this.OnEnemyEngage;
            GameManager.Events.EnemyTargetDisengage -= this.OnEnemyEngage;
        }
        
        /// <summary>
        /// Called when an artillery is fired. Plays <see cref="AudioArtilleryFired"/>.
        /// </summary>
        /// <param name="evt"></param>
        public void OnArtilleryFired(GameEvent evt)
        {
            EventArtilleryFired eventArtillery = (EventArtilleryFired) evt;

            Vector3 averagePosition;
            Quaternion averageRotation;
            if (eventArtillery.GetAverageTransform(out averagePosition, out averageRotation) > 0)
            {
                this.CreateAudio(averagePosition, averageRotation, this.AudioArtilleryFired, eventArtillery.Entity.transform);
            }

        }

        /// <summary>
        /// Called when a ship is hit by a projectile or ram. Plays <see cref="AudioEntityShipHitByProjectile"/> or <see cref="AudioEntityShipHitByRam"/>, repsectively.
        /// </summary>
        /// <param name="evt"></param>
        public void OnEntityShipHitBy(GameEvent evt)
        {
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            AudioSource prefab = null;

            switch (evt.EventID)
            {
                case GameEventID.EntityShipHitByProjectile:
                {
                    EventEntityShipHitByProjectile evt2 = (EventEntityShipHitByProjectile) evt;
                    position = evt2.Projectile.transform.position;
                    rotation = evt2.Projectile.transform.rotation;
                    prefab = this.AudioEntityShipHitByProjectile;
                }
                    break;
                case GameEventID.EntityShipHitByRam:
                {
                    EventEntityShipHitByRam evt2 = (EventEntityShipHitByRam) evt;
                    position = evt2.Figurehead.transform.position;
                    rotation = evt2.Figurehead.transform.rotation;
                    prefab = this.AudioEntityShipHitByRam;
                }
                    break;
                default:
                    break;
            }

            if (prefab != null)
            {
                this.CreateAudio(position, rotation, prefab, this.transform);
            }
        }

        /// <summary>
        /// Called when a player picks up loot. Plays <see cref="AudioOnLootCollected"/>.
        /// </summary>
        /// <param name="evt"></param>
        public void OnLootCollected(GameEvent evt)
        {
            EventLootCollected evtLoot = (EventLootCollected) evt;
            this.CreateAudio(evtLoot.Loot.transform.position, evtLoot.Loot.transform.rotation, this.AudioOnLootCollected, evtLoot.PlayerShip.transform);
        }

        [Deprecated]
        public void OnEnemyEngage(GameEvent evt)
        {
            EventEnemyTargetEngage evtDEngage = (EventEnemyTargetEngage) evt;
            
            //switch (evtDEngage.EventID)
            //{
            //    case GameEventID.EnemyTargetEngage:
            //        this.CreateAudio(
            //            evtDEngage.Target.transform.position,
            //            evtDEngage.Target.transform.rotation,
            //            this.AudioOnEnemyEngage, evtDEngage.Target.transform);
            //        break;
            //    case GameEventID.EnemyTargetDisengage:
            //        this.CreateAudio(
            //            evtDEngage.Target.transform.position,
            //            evtDEngage.Target.transform.rotation,
            //            this.AudioOnEnemyDisengage, evtDEngage.Target.transform);
            //        break;
            //    default:
            //        break;
            //}
        }

    }

}
