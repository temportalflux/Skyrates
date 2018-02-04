#ifndef CHAMPNET_SERVER_GAMESTATE_H
#define CHAMPNET_SERVER_GAMESTATE_H

#include <map>

/// \addtogroup server
/// @{

/// The state of the game across server/client boundaries.
/// NEVER serialized in the client, NEVER deserialized for the server
class GameState
{

public:
	
	/// A structure to hold information about each player
	struct Client
	{

		/// The maximum length of the Player when serialized into a byte[]
		int getSize() const;

		// NOT SERIALIZED

		bool passedHandshake;

		// SERIALIZED

		/// The clientID of the controlling client. This is unique across all peers in a network.
		unsigned int clientID;

		bool playerEntityGuidValid;

		unsigned char playerEntityGuid[16];

		/// Position X
		float physicsPositionX;
		/// Position Y
		float physicsPositionY;
		/// Position Z
		float physicsPositionZ;

		/// VelocityRotationEuler X
		float physicsPositionRotationalEulerX;
		/// VelocityRotationEuler Y
		float physicsPositionRotationalEulerY;
		/// VelocityRotationEuler Z
		float physicsPositionRotationalEulerZ;
		
	};

	/// Clients
	std::map<unsigned int, Client*> clients;

	/// Constructor
	/// @param id the <see cref="clientID"/>
	GameState(int id);

	/// Destructor
	~GameState();

	int getSize() const;

	/// Add a player to the game state
	/// @param clientID the client which owns the player
	void addClient(unsigned int clientID);

	/// Remove a player from the game state
	/// @param clientID the unique <see cref="Client::clientID"/> of the client
	void removeClient(unsigned int clientID);

	/// Serializes the game state into a byte[]
	/// @param packetID the packet ID to include
	/// @param clientID the clientID to emulate this packet as (replaces the game state <see cref="GameState::clientID"/> with the value in the byte[], but doesn't set the value in the local game state).
	/// @param dataLength the length of the byte[] returned
	/// @return byte[]
	char* serializeForClient(unsigned char packetID, int &dataLength, int maxClients);

};
/// @}

#endif