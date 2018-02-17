
using System;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Client
{
    
    public class AudioDispatcher : MonoBehaviour
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

        public AudioSource AudioArtilleryFired;
        public AudioSource AudioEntityShipHitByProjectile;
        public AudioSource AudioEntityShipHitByRam;
        public AudioSource AudioOnLootCollected;
        public AudioSource AudioOnEnemyEngage;
        public AudioSource AudioOnEnemyDisengage;

        private void CreateAudio(Vector3 position, Quaternion rotation, AudioSource prefab, Transform owner)
        {
            if (prefab != null)
            {
                AudioSource spawned = Instantiate(prefab.gameObject, position, rotation, owner).GetComponent<AudioSource>();
                Destroy(spawned.gameObject, spawned.clip.length);
            }
        }

        void OnEnable()
        {
            GameManager.Events.ArtilleryFired += this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
            GameManager.Events.LootCollected += this.OnLootCollected;
            GameManager.Events.EnemyTargetEngage += this.OnEnemyEngage;
            GameManager.Events.EnemyTargetDisengage += this.OnEnemyEngage;
        }

        void OnDisable()
        {
            GameManager.Events.ArtilleryFired -= this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
            GameManager.Events.LootCollected -= this.OnLootCollected;
            GameManager.Events.EnemyTargetEngage -= this.OnEnemyEngage;
            GameManager.Events.EnemyTargetDisengage -= this.OnEnemyEngage;
        }
        
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

        public void OnLootCollected(GameEvent evt)
        {
            EventLootCollected evtLoot = (EventLootCollected) evt;
            if (!evtLoot.PlayerShip.IsLocallyControlled) return;
            this.CreateAudio(evtLoot.Loot.transform.position, evtLoot.Loot.transform.rotation, this.AudioOnLootCollected, evtLoot.PlayerShip.transform);
        }

        public void OnEnemyEngage(GameEvent evt)
        {
            EventEnemyTargetEngage evtDEngage = (EventEnemyTargetEngage) evt;

            if (evtDEngage.Target.EntityType.EntityType != Common.Entity.Entity.Type.Player) return;
            if (!evtDEngage.Target.IsLocallyControlled) return;

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
