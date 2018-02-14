using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network;
using Skyrates.Client.Network.Event;
using Skyrates.Common.Entity;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network;
using Skyrates.Server.Network.Event;
using UnityEngine;

namespace Skyrates.Client.Network
{

    /// <summary>
    /// Base class implementation for all ClientsData.
    /// </summary>
    /// <inheritdoc/>
    public class Client : NetworkCommon
    {

        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
            NetworkEvents.Instance.ConnectionAccepted += this.OnConnectionAccepted;
            NetworkEvents.Instance.ConnectionRejected += this.OnConnectionRejected;
            NetworkEvents.Instance.Disconnect += this.OnDisconnect;
            NetworkEvents.Instance.HandshakeClientID += this.OnHandshakeClientID;

            GameManager.Events.PlayerMoved += this.OnPlayerMoved;
            GameManager.Events.SpawnEntityProjectile += this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile += this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam += this.OnEntityShipHitBy;
            GameManager.Events.LootCollided += this.OnLootCollided;
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            NetworkEvents.Instance.ConnectionAccepted -= this.OnConnectionAccepted;
            NetworkEvents.Instance.ConnectionRejected -= this.OnConnectionRejected;
            NetworkEvents.Instance.Disconnect -= this.OnDisconnect;
            NetworkEvents.Instance.HandshakeClientID -= this.OnHandshakeClientID;

            GameManager.Events.PlayerMoved -= this.OnPlayerMoved;
            GameManager.Events.SpawnEntityProjectile -= this.OnRequestSpawnEntityProjectile;
            GameManager.Events.EntityShipHitByProjectile -= this.OnEntityShipHitBy;
            GameManager.Events.EntityShipHitByRam -= this.OnEntityShipHitBy;
            GameManager.Events.LootCollided -= this.OnLootCollided;
        }

        /// <inheritdoc />
        public override void StartAndConnect(Session session)
        {
            this.EntityTracker = new EntityReceiver();
            
            this.StartClient(session);
            this.Connect(session);
        }

        /// <summary>
        /// Receives connection acceptance by RakNet server (implicit).
        /// Initates the handshake with <see cref="EventHandshakeJoin"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventRakNet"/></param>
        public void OnConnectionAccepted(NetworkEvent evt)
        {
            UnityEngine.Debug.Log("Joining server");
            // Initiates handshake after connecting
            this.Dispatch(new EventHandshakeJoin(),
                NetworkComponent.GetSession.TargetAddress,
                NetworkComponent.GetSession.Port
            );
        }

        /// <summary>
        /// Receives connection rejection by RakNet server (implicit).
        /// </summary>
        /// <param name="evt"><see cref="EventRakNet"/></param>
        public void OnConnectionRejected(NetworkEvent evt)
        {
            UnityEngine.Debug.LogWarning("Connection was rejected... :'(");
        }

        public void OnDisconnect(NetworkEvent evt)
        {
            // TODO: Boot user back to main menu
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        /// <summary>
        /// Receives client ID from <see cref="EventHandshakeClientID"/>.
        /// Responds with <see cref="EventHandshakeAccept"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventHandshakeClientID"/></param>
        public void OnHandshakeClientID(NetworkEvent evt)
        {
            EventHandshakeClientID evtClientId = evt as EventHandshakeClientID;
            UnityEngine.Debug.Assert(evtClientId != null, "evtClientId != null");

            UnityEngine.Debug.Log(string.Format("ClientData got id {0} {1}", evtClientId.client.ClientId, evtClientId.client.PlayerGuid));

            // Set the client ID
            NetworkComponent.GetSession.SetClientData(evtClientId.client);

            // Mark the client as connected to the server (it can now process updates)
            NetworkComponent.GetSession.HandshakeComplete = true;

            this.Dispatch(new EventHandshakeAccept(evtClientId.client.ClientId));

            // TODO: Decouple via events
            SceneLoader.Instance.ActivateNext();
        }

        public void DeserializeGameState(byte[] data, ref int lastIndex)
        {
            // Entities
            EntityTracker tracker = NetworkComponent.GetNetwork().GetEntityTracker();
            tracker.Deserialize(data, ref lastIndex);
        }

        public void OnPlayerMoved(GameEvent evt)
        {
            // On player move, tell server
            // TODO: Reconsider frequency
            NetworkComponent.GetNetwork().Dispatch(new EventRequestSetPlayerPhysics(((EventEntityPlayerShip) evt).PlayerShip.Physics));
        }

        public virtual void OnRequestSpawnEntityProjectile(GameEvent evt)
        {
            EventSpawnEntityProjectile evtSpawn = (EventSpawnEntityProjectile) evt;
            NetworkComponent.GetNetwork().Dispatch(new EventRequestSpawnEntityProjectile(evtSpawn.TypeData, evtSpawn.Spawn, evtSpawn.Velocity, evtSpawn.ImpluseForce));
        }

        // when any entity is suppossed to be damaged
        public virtual void OnEntityShipHitBy(GameEvent evt)
        {
            EventEntityShipDamaged evtDamaged = (EventEntityShipDamaged) evt;
            // If we own the target, then we tell server that one of our entities is damaged
            if (evtDamaged.Ship.IsLocallyControlled)
            {
                NetworkComponent.GetNetwork().Dispatch(new EventRequestEntityShipDamaged(evtDamaged.Ship, evtDamaged.Damage));
            }
        }

        public virtual void OnLootCollided(GameEvent evt)
        {
            EventLootCollided evtLoot = (EventLootCollided)evt;
            if (evtLoot.PlayerShip.IsLocallyControlled)
            {
                // pass back to player for handling locally
                evtLoot.PlayerShip.OnLootCollided(evtLoot.Loot);
            }
        }

    }
}
