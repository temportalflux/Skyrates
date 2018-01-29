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
	struct Player
	{
		/// The maximum length of any player's name
		const static int SIZE_MAX_NAME = 10;

		/// The clientID of the controlling client. This is unique across all peers in a network.
		unsigned int clientID;
		/// The playerID of the character. This is unique across all players in a network.
		unsigned int playerID;
		/// The localID of the character. This is unqiue to all players with the same <see cref="Player::clientID"/>.
		unsigned int localID;
		/// the name of the character
		std::string name;
		/// the highlight color of the character
		float colorR, colorG, colorB, colorA;
		/// the position
		float posX, posY, posZ;
		/// velocity
		float velX, velY, velZ;
		/// and acceleration
		float accX, accY, accZ;
		/// if the player is in battle
		bool inBattle;
		/// Number of wins the player has
		unsigned int wins;
		/// Rank of player on the scoreboard
		int rank;

		int monstersCount;
		unsigned int *monsters;

		// Not serialized
		/// the playerId of the opponent, -1 if invalid, otherwise >= 0
		int battleOpponentId;
		/// the last selection in battles, -1 if invalid, otherwise >= 0
		int lastBattleSelection;
		/// the last selection choice in battle, -1 if invalid, otherwise >= 0
		int lastBattleChoice;

		/// The maximum length of the Player when serialized into a byte[]
		int getSize() const;

	};

	/// The ID of the client this gamestate is in
	int clientID;

	/// A map of all players, from their global unqiue <see cref="Player::playerID"\> to the <see cref="Player"/> object.
	std::map<unsigned int, Player> players;

	/// Constructor
	/// @param id the <see cref="clientID"/>
	GameState(int id);
	/// Destructor
	~GameState();

	/// Add a player to the game state
	/// @param clientID the client which owns the player
	/// @param playerID the unique ID of the player
	/// @param localID the unique ID local to the client
	/// @param name the name of the player
	/// @param colorR the red channel of the overlay color
	/// @param colorG the green channel of the overlay color
	/// @param colorB the blue channel of the overlay color
	void addPlayer(unsigned int clientID, unsigned int playerID, unsigned int localID,
		std::string name, float colorR, float colorG, float colorB);

	/// Remove a player from the game state
	/// @param playerID the unique <see cref="Player::playerID"/> of the player
	void removePlayer(unsigned int playerID);

	int getSize() const;

	/// Serializes the game state into a byte[]
	/// @param packetID the packet ID to include
	/// @param clientID the clientID to emulate this packet as (replaces the game state <see cref="GameState::clientID"/> with the value in the byte[], but doesn't set the value in the local game state).
	/// @param dataLength the length of the byte[] returned
	/// @return byte[]
	char* serializeForClient(unsigned char packetID, int clientID, int &dataLength);

};
/// @}

#endif