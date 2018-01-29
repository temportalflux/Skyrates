#ifndef CHAMPNET_SERVER_SERVER_H
#define CHAMPNET_SERVER_SERVER_H

#include "StateApplication.h"

#include <string>

class GameState;
class PerformanceTracker;

namespace ChampNet
{
	class Packet;
};

/// \addtogroup server
/// @{

/// The state application for the server (the server has only one state)
class StateServer : public StateApplication
{

	/// A structure detailing a request for a player
	struct PlayerRequest
	{
		/// The <see cref="GameState::Player::lcoalID"/> of the player request
		unsigned int localID;
		/// The <see cref="GameState::Player::name"/> of the player
		std::string name;
		/// The <see cref="GameState::Player::colorR"/>
		float colorR;
		/// The <see cref="GameState::Player::colorG"/>
		float colorG;
		/// The <see cref="GameState::Player::colorB"/>
		float colorB;
	};

private:
	typedef std::string* PlayerAddress;

	/// The number of clients currently connected.
	int mClientCount;
	/// The number of players currently active (>= <see cref="mClientCount"/>.
	int mPlayerCount;
	/// An array of peer addresses for the connected clients, keyed by <see cref="GameState::clientID"/>.
	PlayerAddress *mpClientAddresses = NULL;
	/// An array of <see cref="GameState::clientID"/>s, keyed by <see cref="GameState::Player::playerID"/>
	int *mpPlayerIdToClientId = NULL;
	/// An array of <see cref="GameState::Player::playerID"/>s, keyed by <see cref="GameState::clientID"/>.
	int **mpClientIdToPlayers = NULL;

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

	/// Called when a key is marked as down this update
	/// \author Dustin Yost
	virtual void onKeyDown(int i) override;

	/// Called when some input has been entered (ENTER has been pressed)
	virtual void onInput(std::string &input);

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

	/// Deserializes packet data for player requests into an array of player requests
	/// \author Dustin Yost
	void deserializePlayerRequests(ChampNet::Packet *pPacket, PlayerRequest *&requests, int &playerRequestCount);

	/// Finds the next available address slot, returning -1 if none is found
	/// \author Dustin Yost
	bool findNextClientID(unsigned int &id);

	/// Finds the next available player slot, returning -1 if none is found
	/// \author Dustin Yost
	bool findNextPlayerID(unsigned int &id);

	/// Adds a client to <see cref="mpClientAddresses"/>
	/// \author Dustin Yost
	bool addClient(const char* address, unsigned int &id);
	/// Removes a client from <see cref="mpClientAddresses"/>
	/// \author Dustin Yost
	void removeClient(unsigned int id);
	/// Adds a player to <see cref="mpGameState"/>
	/// \author Dustin Yost
	bool addPlayer(unsigned int clientID, unsigned int localID, unsigned int &playerID,
		std::string name, float colorR, float colorG, float colorB);
	/// Serializes and ships <see cref="mpGameState"/> to clients.
	/// @param msgID the packetID.
	/// @param sender [optional] the address to send to, or the address to not send to.
	/// @param broadcast [optional] if the packet should be shipped to ALL connected peers (if true, sender is not shipped to).
	/// @param clientID [optional] the client ID to set in place in the GameState (see <see cref="GameState::serializeForClient"/>).
	/// \author Dustin Yost
	void sendGameState(unsigned char msgID, const char* sender = NULL, bool broadcast = true, int clientID = -1);

	/// Returns the client IPv4 address for some <see cref="GameState::Player::playerID"/>.
	/// \author Dustin Yost
	const char* getClientAddressFrom(unsigned int playerID);

	/// Used to put players into rank order based on wins
	void CalculateScoreBoardData();

};

/// @}

#endif // CHAMPNET_SERVER_SERVER_H