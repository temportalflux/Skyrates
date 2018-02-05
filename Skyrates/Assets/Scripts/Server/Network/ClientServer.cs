using Skyrates.Common.Network;
using Skyrates.Client.Network.Event;
using Skyrates.Server.Network.Event;

namespace Skyrates.Server.Network
{
    public class ClientServer : NetworkCommon
    {

        /// <inheritdoc />
        public override void Create()
        {
            base.Create();
            NetworkEvents.HandshakeJoin += this.OnHandshakeJoin;
            NetworkEvents.HandshakeAccept += this.OnHandshakeAccept;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
            NetworkEvents.HandshakeJoin -= this.OnHandshakeJoin;
            NetworkEvents.HandshakeAccept -= this.OnHandshakeAccept;
        }

        /// <inheritdoc />
        public override void StartAndConnect(Session session)
        {
            this.StartServer(session);
        }

        /// <summary>
        /// Receives a handshake request of <see cref="EventHandshakeJoin"/>.
        /// Generates client ID and adds client to listings.
        /// Responds with <see cref="EventHandshakeClientID"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventHandshakeJoin"/></param>
        public void OnHandshakeJoin(NetworkEvent evt)
        {
            // TODO: Add client / get next client id
            // TODO: Log that a client joined
            // TODO: Create client entry
            // TODO: Send client ID back
        }

        /// <summary>
        /// Receives a handshake request of <see cref="EventHandshakeAccept"/>.
        /// Copies client's player <see cref="System.Guid"/> and marks them as valid.
        /// Does not respond, but client can now start processing <see cref="GameState"/> updates via <see cref="EventUpdateGameState"/>.
        /// </summary>
        /// <param name="evt"><see cref="EventHandshakeAccept"/></param>
        public void OnHandshakeAccept(NetworkEvent evt)
        {
            // TODO: copy over player GUID
            // TODO: Mark client as with valid player GUID
            // TODO: implement game state updates
        }

    }

}