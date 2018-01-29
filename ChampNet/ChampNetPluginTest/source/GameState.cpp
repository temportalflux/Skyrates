/*
Names and ID: Christopher Brennan: 1028443, Dustin Yost: 0984932, Jacob Ruth: 0890406
Course Info: EGP-405-01
Project Name: Project 3: Synchronized Networking
Due: 11/22/17
Certificate of Authenticity (standard practice): “We certify that this work is entirely our own.
The assessor of this project may reproduce this project and provide copies to other academic staff,
and/or communicate a copy of this project to a plagiarism-checking service, which may retain a copy of the project on its database.”
*/
#include "GameState.h"

GameState::GameState(int id)
{
	this->clientID = id;
	this->players = std::map<unsigned int, GameState::Player>();
}

GameState::~GameState()
{

}

void GameState::addPlayer(unsigned int clientID, unsigned int playerID, unsigned int localID,
	std::string name, float colorR, float colorG, float colorB)
{
	Player player;
	player.clientID = clientID;
	player.playerID = playerID;
	player.localID = localID;
	player.name = name;
	player.colorR = colorR;
	player.colorG = colorG;
	player.colorB = colorB;
	player.colorA = 1.0f;
	player.posX = player.posY = player.posZ = 0;
	player.velX = player.velY = player.velZ = 0;
	player.accX = player.accY = player.accZ = 0;
	player.inBattle = false;
	player.wins = 0;
	player.rank = -1;
	player.monstersCount = 0;
	player.monsters = new unsigned int[0];

	player.battleOpponentId = -1;
	player.lastBattleSelection = -1;
	player.lastBattleChoice = -1;

	this->players[playerID] = player;
}

void GameState::removePlayer(unsigned int playerID)
{
	if (this->players.find(playerID) != this->players.end())
	{
		this->players.erase(playerID);
	}
}

int GameState::Player::getSize() const
{
	return 0
		// clientID
		+ sizeof(unsigned int)
		// playerID
		+ sizeof(unsigned int)
		// localID
		+ sizeof(unsigned int)
		// name
		+ sizeof(int) + (this->name.length() * (sizeof(char) * 2))
		// color
		+ (sizeof(float) * 4)
		// position
		+ (sizeof(float) * 3)
		// velocity
		+ (sizeof(float) * 3)
		// acceleration
		+ (sizeof(float) * 3)
		// inBattle
		+ sizeof(bool)
		// Wins
		+ sizeof(unsigned int)
		// Rank
		+ sizeof(int)
		// number of monsters
		+ sizeof(int)
		// size of monster
		+ (this->monstersCount * sizeof(unsigned int));
	;
}

int GameState::getSize() const
{
	int totalSize = 0
		// packetID
		+ sizeof(unsigned char)
		// clientID
		+ sizeof(int)
		;
	// number of players
	int sizePlayers = this->players.size();
	totalSize += sizeof(int);
	// amount of space required for players
	for (auto const &entry : this->players)
	{
		totalSize += entry.second.getSize();
	}
	return totalSize;
}

char* GameState::serializeForClient(unsigned char packetID, int clientID, int &dataLength)
{
	const int size = this->getSize();
	dataLength = size;
	char *data = new char[size];
	char *pos = data;

	// write the packetID
	*((char *)pos) = packetID; pos += sizeof(packetID);
	// write the client ID
	*((int *)pos) = clientID; pos += sizeof(int);
	// write player count
	*((int *)pos) = this->players.size(); pos += sizeof(int);

	// write players
	for (auto const &entry : this->players)
	{
		Player player = entry.second;

		// write clientID
		*((int *)pos) = player.clientID; pos += sizeof(unsigned int);
		// write playerID
		*((int *)pos) = player.playerID; pos += sizeof(unsigned int);
		// write localID
		*((int *)pos) = player.localID; pos += sizeof(unsigned int);

		// write name
		// NOTE: this does not use ALL of the name byte, if the name is < SIZE_MAX_NAME characters
		int nameLength = player.name.length();
		//nameLength = nameLength < Player::SIZE_MAX_NAME ? nameLength : Player::SIZE_MAX_NAME;
		*((int *)pos) = nameLength; pos += sizeof(int);
		for (int i = 0; i < nameLength; i++)
		{
			*((char *)pos) = player.name[i];
			*((char *)(pos + 1)) = 0;
			// move head twice to account for UTF-16 encoding
			pos += sizeof(char) * 2;
		}

		// write color
		*((float *)pos) = player.colorR; pos += sizeof(float);
		*((float *)pos) = player.colorG; pos += sizeof(float);
		*((float *)pos) = player.colorB; pos += sizeof(float);
		*((float *)pos) = player.colorA; pos += sizeof(float);
		// write position
		*((float *)pos) = player.posX; pos += sizeof(float);
		*((float *)pos) = player.posY; pos += sizeof(float);
		*((float *)pos) = player.posZ; pos += sizeof(float);
		// write velocity
		*((float *)pos) = player.velX; pos += sizeof(float);
		*((float *)pos) = player.velY; pos += sizeof(float);
		*((float *)pos) = player.velZ; pos += sizeof(float);
		// write acceleration
		*((float *)pos) = player.accX; pos += sizeof(float);
		*((float *)pos) = player.accY; pos += sizeof(float);
		*((float *)pos) = player.accZ; pos += sizeof(float);
		// write inBattle
		*((int *)pos) = player.inBattle; pos += sizeof(bool);
		// write Wins
		*((unsigned int *)pos) = player.wins; pos += sizeof(unsigned int);
		// write Rank
		*((int *)pos) = player.rank; pos += sizeof(int);
		// write # of monsters
		*((int *)pos) = player.monstersCount; pos += sizeof(int);
		// write monsters
		for (int iMonster = 0; iMonster < player.monstersCount; iMonster++)
		{
			*((unsigned int *)pos) = player.monsters[iMonster]; pos += sizeof(unsigned int);
		}
	}

	// data is now filled with the gamestate data
	return data;
}
