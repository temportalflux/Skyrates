#include "GameState.h"

GameState::GameState(int id)
{
	this->clients = std::map<unsigned int, GameState::Client*>();
}

GameState::~GameState()
{
	for (auto it = this->clients.begin(); it != this->clients.end(); ++it)
	{
		Client *pClient = it->second;
		delete pClient;
	}
	this->clients.clear();
}

void GameState::addClient(unsigned int clientID)
{
	Client *pClient = new Client();
	pClient->clientID = clientID;

	this->clients[clientID] = pClient;
}

void GameState::removeClient(unsigned int clientID)
{
	if (this->clients.find(clientID) != this->clients.end())
	{
		Client *pClient = this->clients[clientID];
		delete pClient;
		this->clients.erase(clientID);
	}
}

int GameState::Client::getSize() const
{
	return 0
		// clientID
		+ sizeof(unsigned int)
		// if guid is valid
		+ 1
		// playerEntityGuid
		+ sizeof(int) + 16
		;
	;
}

int GameState::getSize() const
{
	int totalSize = 0
		// packetID
		+ sizeof(unsigned char)
		;

	// max clients
	//totalSize += sizeof(int);

	// number of clients
	int sizeClients = this->clients.size();
	totalSize += sizeof(int);
	
	// amount of space required for clients
	for (auto const &entry : this->clients)
	{
		totalSize += entry.second->getSize();
	}

	return totalSize;
}

char* GameState::serializeForClient(unsigned char packetID, int &dataLength, int maxClients)
{
	const int size = this->getSize();
	dataLength = size;
	char *data = new char[size];
	char *pos = data;

	// write the packetID
	*((char *)pos) = packetID; pos += sizeof(packetID);

	// write the total possible clients
	//*((int *)pos) = maxClients; pos += sizeof(maxClients);

	// write the # of clients
	int sizeClients = this->clients.size();
	*((int *)pos) = sizeClients; pos += sizeof(sizeClients);

	for (auto const &entry : this->clients)
	{
		Client *pClient = entry.second;

		// write clientID
		*((unsigned int *)pos) = pClient->clientID;
		pos += sizeof(unsigned int);

		*((unsigned char*) pos) = pClient->playerEntityGuidValid ? 1 : 0; pos++;

		int guidSize = 16;
		*((int *) pos) = guidSize;
		pos += sizeof(guidSize);
		memcpy(pos, pClient->playerEntityGuid, guidSize);

	}

	// data is now filled with the gamestate data
	return data;
}
