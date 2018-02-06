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

        /// <inheritdoc />
        public override void Create()
        {
            base.Create();
            NetworkEvents.Instance.ConnectionAccepted += this.OnConnectionAccepted;
            NetworkEvents.Instance.ConnectionRejected += this.OnConnectionRejected;
            NetworkEvents.Instance.Disconnect += this.OnDisconnect;
            NetworkEvents.Instance.HandshakeClientID += this.OnHandshakeClientID;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
            NetworkEvents.Instance.ConnectionAccepted -= this.OnConnectionAccepted;
            NetworkEvents.Instance.ConnectionRejected -= this.OnConnectionRejected;
            NetworkEvents.Instance.Disconnect -= this.OnDisconnect;
            NetworkEvents.Instance.HandshakeClientID -= this.OnHandshakeClientID;
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
            Debug.LogWarning("Server kicked us... do something.");
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

            UnityEngine.Debug.Log("ClientData got id " + evtClientId.client.ClientId);

            // Set the client ID
            NetworkComponent.GetSession.SetClientData(evtClientId.client);

            // Mark the client as connected to the server (it can now process updates)
            NetworkComponent.GetSession.HandshakeComplete = true;

            this.Dispatch(new EventHandshakeAccept(evtClientId.client.ClientId, Entity.NewGuid()));

            // TODO: Decouple via events
            SceneLoader.Instance.ActivateNext();
        }
        
    }
}
