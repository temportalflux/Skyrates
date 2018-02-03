#ifndef CHAMPNET_SERVER_SERVER_H
#define CHAMPNET_SERVER_SERVER_H

#include "StateApplication.h"
#include "ChampNetPlugin.h"

#include <string>

class GameState;
class PerformanceTracker;

namespace ChampNet
{
	class Packet;
};

/// \addtogroup server
/// @{

enum MessageIDs
{
	ID_NONE = ChampNetPlugin::MessageIDs::ID_NONE,
	
	// Who is it sent by?
	#pragma region Server

	//! [Handshake.2] Sent with the Client's ID on connection
	HandshakeClientID,

	UpdateGamestate,

	#pragma endregion

	#pragma region Client

	//! [Handshake.1] Sent to initiate handshake and ask server to join
	HandshakeJoin,

	//! [Handshake.3] Sent to tell server that we have accepted the client ID (GameState updates can begin)
	HandshakeAccept,

	//! [Update] Sent to server to update physics data about the player
	UpdatePlayerPhysics,

	Disconnect,

	#pragma endregion

};

/// The state application for the server (the server has only one state)
class StateServer : public StateApplication
{

private:
	typedef std::string* PlayerAddress;

	/// The number of clients currently connected.
	int mClientCount;

	/// An array of peer addresses for the connected clients, keyed by <see cref="GameState::clientID"/>.
	PlayerAddress *mpClientAddresses = NULL;

	/// The game state of the client/server game
	GameState *mpGameState;

	/// A performance tracker for update shipments
	PerformanceTracker *mpTimers;

	/// The quantity of milliseconds between game update shipments
	double mMsPerUpdate;

public:

	/// Constructor
	StateServer();
	/// Destructor
	virtual ~StateServer();

	/// Called when the state is entered
	/// \author Dustin Yost
	virtual void onEnterFrom(StateApplication *previous) override;

	/// Updates the network
	 /// \author Dustin Yost
	virtual void updateNetwork() override;

	/// Handles the usage of all the different packet identifiers
	 /// \author Dustin Yost
	void handlePacket(ChampNet::Packet *packet);

	/// Updates the game every possible tick
	/// \author Dustin Yost
	virtual void updateGame() override;

	/// Renders things to the server console
	/// \author Dustin Yost
	virtual void render() override;

	/// Starts the server at whatever address and port are in the state data
	 /// \author Dustin Yost
	void start();
	/// Disconnects server and notifies clients of disconnection
	 /// \author Dustin Yost
	void disconnect();

	/// Send packet data to the network
	/// \author Dustin Yost
	void sendPacket(const char *address, char *data, int dataSize, bool broadcast);

	/// Cast some general packet type to data
	/// \author Dustin Yost
	template <typename T>
	void sendPacket(const char *address, T *packet, bool broadcast)
	{
		this->sendPacket(address, (char*)(packet), sizeof(*packet), broadcast);
	}

	/// Sends clients the notification of server disconnection
	 /// \author Dustin Yost
	void sendDisconnectPacket(const char *address, bool broadcast);
	
	/// Finds the next available address slot, returning -1 if none is found
	/// \author Dustin Yost
	bool findNextClientID(unsigned int &id);

	/// Adds a client to <see cref="mpClientAddresses"/>
	/// \author Dustin Yost
	bool addClient(const char* address, unsigned int &id);

	/// Removes a client from <see cref="mpClientAddresses"/>
	/// \author Dustin Yost
	void removeClient(unsigned int id);
	
	/// Serializes and ships <see cref="mpGameState"/> to clients.
	/// @param msgID the packetID.
	/// @param sender [optional] the address to send to, or the address to not send to.
	/// @param broadcast [optional] if the packet should be shipped to ALL connected peers (if true, sender is not shipped to).
	/// @param clientID [optional] the client ID to set in place in the GameState (see <see cref="GameState::serializeForClient"/>).
	/// \author Dustin Yost
	void sendGameState(unsigned char msgID, const char* sender = NULL, bool broadcast = true, int clientID = -1);
	
};

/// @}

#endif // CHAMPNET_SERVER_SERVER_H