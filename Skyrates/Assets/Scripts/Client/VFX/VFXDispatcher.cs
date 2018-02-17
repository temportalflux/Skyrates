using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using UnityEngine;

public class VFXDispatcher : MonoBehaviour
{

    [Serializable]
    public enum DispatchID
    {
        LootPickup,
        ProjectileLaunch,
        ProjectileHit,
        RamHit,
    }

    [Serializable]
    public class DispatchEntry
    {

        [SerializeField]
        public DispatchID ID;

        [SerializeField]
        public ParticleSystem Prefab;

    }

    [SerializeField]
    public DispatchEntry[] DispatchEntries;

    private readonly Dictionary<DispatchID, List<ParticleSystem>> _dispatchObjects = new Dictionary<DispatchID, List<ParticleSystem>>();

    private void Awake()
    {
        foreach (DispatchEntry entry in DispatchEntries)
        {
            if (!this._dispatchObjects.ContainsKey(entry.ID))
                this._dispatchObjects.Add(entry.ID, new List<ParticleSystem>());
            this._dispatchObjects[entry.ID].Add(entry.Prefab);
        }
    }

    private List<ParticleSystem> Dispatch(DispatchID id, Vector3 position, Quaternion rotation)
    {
        List<ParticleSystem> spawned = new List<ParticleSystem>();
        if (!this._dispatchObjects.ContainsKey(id)) return spawned;

        foreach (ParticleSystem prefab in _dispatchObjects[id])
        {
            if (prefab == null)
            {
                Debug.Log(string.Format("Found null prefab for dispatcher {0} at ID {1}", this.name, id));
                continue;
            }
            spawned.Add(this.SpawnPrefab(prefab, position, rotation));
        }

        return spawned;
    }

    protected virtual ParticleSystem SpawnPrefab(ParticleSystem prefab, Vector3 position, Quaternion rotation)
    {

        ParticleSystem obj = Instantiate(prefab.gameObject, position, rotation).GetComponent<ParticleSystem>();

        float lifetime;
        if (this.GetDuration(obj, out lifetime))
        {
            Destroy(obj.gameObject, lifetime);
        }

        return obj;
    }

    protected virtual bool GetDuration(ParticleSystem obj, out float lifetime)
    {
        lifetime = obj.main.duration;
        return true;
    }

    protected virtual void OnEnable()
    {
        GameManager.Events.ArtilleryFired += this.OnArtilleryFired;
        GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
        GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
        GameManager.Events.LootCollected += this.OnLootCollected;
    }

    protected virtual void OnDisable()
    {
        GameManager.Events.ArtilleryFired -= this.OnArtilleryFired;
        GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
        GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
        GameManager.Events.LootCollected -= this.OnLootCollected;
    }

    private void OnArtilleryFired(GameEvent evt)
    {
        EventArtilleryFired eventArtillery = (EventArtilleryFired) evt;

        Vector3 averagePosition;
        Quaternion averageRotation;
        if (eventArtillery.GetAverageTransform(out averagePosition, out averageRotation) > 0)
        {
            List<ParticleSystem> systems = this.Dispatch(DispatchID.ProjectileLaunch, averagePosition, averageRotation);
            foreach (ParticleSystem system in systems)
            {
                system.transform.parent = eventArtillery.Entity.transform;
            }
        }

    }

    private void OnEntityShipHitBy(GameEvent evt)
    {
        Vector3 position;
        Quaternion rotation;
        DispatchID id;

        switch (evt.EventID)
        {
            case GameEventID.EntityShipHitByProjectile:
            {
                EventEntityShipHitByProjectile evt2 = (EventEntityShipHitByProjectile) evt;
                position = evt2.Projectile.transform.position;
                rotation = evt2.Projectile.transform.rotation;
                id = DispatchID.ProjectileHit;
            }
                break;
            case GameEventID.EntityShipHitByRam:
            {
                EventEntityShipHitByRam evt2 = (EventEntityShipHitByRam) evt;
                position = evt2.Figurehead.transform.position;
                rotation = evt2.Figurehead.transform.rotation;
                id = DispatchID.RamHit;
                }
                break;
            default:
                return;
        }
        rotation *= Quaternion.Euler(0, 180, 0);
        foreach (ParticleSystem system in this.Dispatch(id, position, rotation))
        {
            system.transform.parent = ((EventEntityShipDamaged) evt).Entity.transform;
        }
    }

    private void OnLootCollected(GameEvent evt)
    {
        EventLootCollected evtLoot = (EventLootCollected) evt;
        if (!evtLoot.PlayerShip.IsLocallyControlled)
            return;
        foreach (ParticleSystem system in this.Dispatch(DispatchID.LootPickup, evtLoot.Entity.transform.position, evtLoot.Entity.transform.rotation))
        {
            system.transform.parent = evtLoot.Entity.transform;
        }
    }

}
