
using System;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Client.Ship;
using UnityEngine;

namespace Skyrates.Client
{

    [RequireComponent(typeof(GameManager))]
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

        private void CreateAudio(Vector3 position, Quaternion rotation, AudioSource prefab)
        {
            AudioSource spawned = Instantiate(prefab, position, rotation, this.transform);
            Destroy(spawned, prefab.time);
        }

        void OnEnable()
        {
            GameManager.Events.ArtilleryFired += this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
        }

        void OnDisable()
        {
            GameManager.Events.ArtilleryFired -= this.OnArtilleryFired;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
        }
        
        public void OnArtilleryFired(GameEvent evt)
        {
            EventArtilleryFired eventArtillery = (EventArtilleryFired) evt;

            // Position average is easy
            Vector3 averagePosition = Vector3.zero;
            // Quaternion average taken from https://answers.unity.com/questions/815266/find-and-average-rotations-together.html
            Quaternion averageRotation = new Quaternion(0, 0, 0, 0);
            int artilleryCount = eventArtillery.Artillery.Length;
            foreach (ShipArtillery artillery in eventArtillery.Artillery)
            {
                averagePosition += artillery.transform.position;
                averageRotation = Quaternion.Slerp(averageRotation, artillery.transform.rotation, 1 / artilleryCount);
            }
            averagePosition /= artilleryCount;

            this.CreateAudio(averagePosition, averageRotation, this.AudioArtilleryFired);
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
                this.CreateAudio(position, rotation, prefab);
            }
        }

    }

}
