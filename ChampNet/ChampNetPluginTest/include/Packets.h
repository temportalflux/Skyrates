#pragma once

// All the possible packet configurations

/// \addtogroup server
/// @{

#pragma pack(push, 1)

/// A general packet with the packetID
struct PacketGeneral
{
	/// The packetID
	unsigned char id;
};

/// A paccket with an unknown identifier
struct PacketUserID
{
	/// The packetID
	unsigned char id;
	/// Some identifier
	unsigned int dataID;
};

struct PacketUserIDBool
{
	/// The packetID
	unsigned char id;
	/// The <see cref="GameState::Player::playerID"/> of the player.
	unsigned int playerID;
	bool toggle;
};

/// A packet with a clientID and playerID
struct PacketClientPlayerID
{
	/// The packetID
	unsigned char id;
	/// The ID of the client, akin to <see cref="GameState::clientID"/>
	unsigned int clientID;
	/// The ID of the player, akin to <see cref="GameState::Player::playerID"/>
	unsigned int playerID;
};

/// A packet with a clientID, playerID, and score of the player
// TODO: temporary? This is a bad packet because it mandates the player's score to the server
struct PacketPlayerScoreboardInfo // created to just increment score/set new rank
{
	/// The packetID
	unsigned char id;
	/// The ID of the client, akin to <see cref="GameState::clientID"/>
	unsigned int clientID;
	/// The ID of the player, akin to <see cref="GameState::Player::playerID"/>
	unsigned int playerID;
	/// The new score of the player
	unsigned int score;
};

/// A packet with a clientID, playerID, and dead reckoning variables
struct PacketPlayerPosition
{
	/// The packetID
	unsigned char id;
	/// The ID of the client, akin to <see cref="GameState::clientID"/>
	unsigned int clientID;
	/// The ID of the player, akin to <see cref="GameState::Player::playerID"/>
	unsigned int playerID;
	/// The x-coord of the player's position, <see cref="GameState::Player::posX"/>
	float posX;
	/// The y-coord of the player's position, <see cref="GameState::Player::posY"/>
	float posY;
	/// The x-coord of the player's velocity, <see cref="GameState::Player::velX"/>
	float velX;
	/// The y-coord of the player's velocity, <see cref="GameState::Player::velX"/>
	float velY;
};

/// A packet with two playerIDs
struct PacketUserIDDouble
{
	/// The packetID
	unsigned char id;
	/// A <see cref="GameState::Player::playerID"/>
	unsigned int playerIdSender;
	/// A <see cref="GameState::Player::playerID"/>
	unsigned int playerIdReceiver;
};

/// A packet with three playerIDs
struct PacketUserIDTriple
{
	/// The packetID
	unsigned char id;
	/// A <see cref="GameState::Player::playerID"/>
	unsigned int playerIdSender;
	/// A <see cref="GameState::Player::playerID"/>
	unsigned int playerIdReceiver;
	/// A <see cref="GameState::Player::playerID"/>
	unsigned int playerIdThird;
};

/// A packet responding to a battle request (<see cref="PacketUserIDDouble"/>), and includes an acceptance flag.
struct PacketBattleResponse
{
	/// The packetID
	unsigned char id;
	/// A <see cref="GameState::Player::playerID"/>. The sender of the response (and the receiver of a battle request).
	unsigned int playerIdSender;
	/// A <see cref="GameState::Player::playerID"/>. The receiver of the response (and the sender of the battle request).
	unsigned int playerIdReceiver;
	/// A flag indicating if the <see cref="PacketBattleResponse::playerIdSender"/> has accepted the battle.
	bool accepted;
};

/// A packet with with user selected data from the previous round. Expects a response back from both player's for their next move in the form of <see cref="PacketBattleSelection"/>.
struct PacketBattlePromptSelection
{
	/// The packetID
	unsigned char id;

	/// A <see cref="GameState::Player::playerID"/> of the first player in the previous round.
	unsigned int playerAId;
	/// The <see cref="BattleSelection"/> of the first player, -1 if invalid
	int playerASelection;
	/// The <see cref="BattleSelection"/> choice of the first player, -1 if invalid
	int playerAChoice;

	/// A <see cref="GameState::Player::playerID"/> of the second player in the previous round.
	unsigned int playerBId;
	/// The <see cref="BattleSelection"/> of the second player, -1 if invalid
	int playerBSelection;
	/// The <see cref="BattleSelection"/> choice of the second player, -1 if invalid
	int playerBChoice;

};

/// A packet with a playerID, their opponent's ID, and the player's selections for battle
struct PacketBattleSelection
{
	/// The packetID
	unsigned char id;
	/// The <see cref="GameState::Player::playerID"/> of the player whose selection this is
	unsigned int playerId;
	/// The <see cref="GameState::Player::playerID"/> of the opponent
	unsigned int playerIdOpponent;
	/// The player's battle option <see cref="BattleSelection"/>
	unsigned int selection;
	/// The player's battle option <see cref="BattleSelection"/> choice
	unsigned int choice;
};

#pragma pack(pop)

/// The selections for battles
enum BattleSelection : unsigned int
{
	ATTACK, SWAP, FLEE,
};

/// @}
