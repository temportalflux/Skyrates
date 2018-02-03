#include "StateServer.h"

#include "Packet.h"

#include "StateData.h"
#include "Win.h"
#include <iostream>
#include <RakNet\PacketPriority.h>
#include "Packets.h"
#include "GameState.h"
#include "PerformanceTracker.h"

#include <stdlib.h> // rand
#include <sstream>
#include <time.h>

StateServer::StateServer() : StateApplication()
{
	ChampNetPlugin::Create();

	this->mpGameState = new GameState(-1);
	this->mpTimers = new PerformanceTracker();
	this->mMsPerUpdate =
		//100;//1 / (10 * 0.001); // (10 updates / 1 second) * (1 seconds / 1000 ms) = (1 updates / 100 ms) => (u/ms)^(-1) = (100 ms / 1 u)
		1000;// 1 / (1 * 0.001); // (1 updates / 1 second) * ( 1 seconds / 1000 ms) = (1 updates / 1000 ms) => (u/ms)^(-1) = (1000 ms / 1 u)
}

StateServer::~StateServer()
{
	if (this->mpClientAddresses != NULL)
	{
		for (int i = 0; i < mClientCount; i++)
			if (mpClientAddresses[i] != NULL)
			{
				delete mpClientAddresses[i];
				mpClientAddresses[i] = NULL;
			}
		delete[] this->mpClientAddresses;
		this->mpClientAddresses = NULL;
	}

	if (mpGameState != NULL)
	{
		delete mpGameState;
		mpGameState = NULL;
	}

	if (this->mpTimers != NULL)
	{
		delete mpTimers;
		this->mpTimers = NULL;
	}

	this->disconnect();
	ChampNetPlugin::Destroy();
}

void StateServer::onEnterFrom(StateApplication *previous)
{
	StateApplication::onEnterFrom(previous);
	this->start();
}

/**
 * Starts the server at whatever address and port are in the state data
 */
void StateServer::start()
{
	std::cout << "Starting server...\n";

	this->mClientCount = this->mpState->mNetwork.maxClients;
	this->mpClientAddresses = new PlayerAddress[mClientCount];
	for (int i = 0; i < mClientCount; i++)
		this->mpClientAddresses[i] = NULL;
	
	ChampNetPlugin::StartServer(this->mpState->mNetwork.port, this->mpState->mNetwork.maxClients);

	this->mpTimers->startTracking("update");
}

/**
 * Disconnects server and notifies clients of disconnection
 */
void StateServer::disconnect()
{
	this->sendDisconnectPacket(NULL, true);
	ChampNetPlugin::Disconnect();
}

/**
 * Sends clients the notification of server disconnection
 */
void StateServer::sendDisconnectPacket(const char *address, bool broadcast)
{
	PacketGeneral packet[1];
	packet->id = MessageIDs::Disconnect;
	this->sendPacket(address == NULL ? ChampNetPlugin::GetAddress() : address, packet, broadcast);
}

/**
 * Updates the network
 */
void StateServer::updateNetwork()
{
	// Fetch all packets and put them in a queue - aka pull all packets from RakNet and put them in a queue in the plugin
	ChampNetPlugin::FetchPackets();
	// Iterate over all packets in the plugin queue
	void* pPacket = NULL;
	bool foundValidPacket = false;
	do
	{
		// Get the next available packet
		pPacket = ChampNetPlugin::PollPacket(foundValidPacket);
		// Check if that packet is valid (there was a packet in the queue)
		if (foundValidPacket)
		{
			// A packet was found, handle it
			this->handlePacket((ChampNet::Packet*)pPacket);
			// Free the packet from memory (delete it)
			ChampNetPlugin::FreePacket(pPacket);
		}
		// Only operate while there are packets left in this fetch (more may have come in since fetch, but they can be taken care of next call)
	} while (foundValidPacket);
	
}

/**
 * Handles the usage of all the different packet identifiers
 */
void StateServer::handlePacket(ChampNet::Packet *packet)
{
	// Check which packet type it is
	switch (packet->getID())
	{
		// Some client is connecting (expect a ID_USER_JOINED shortly)
		case ChampNetPlugin::ID_CLIENT_CONNECTION:
			{
				std::cout << "User connected\n";
				this->mpState->mNetwork.peersConnected++;
			}
			break;
			// Some client has disconnected
		case ChampNetPlugin::ID_CLIENT_DISCONNECTION:
			{
				std::cout << "User disconnected\n";
				this->mpState->mNetwork.peersConnected--;
			}
			break;
			// A client is joining
		case MessageIDs::HandshakeJoin:
			{

				unsigned int pPacketLength = 0;
				PacketGeneral* pPacket = packet->getPacketAs<PacketGeneral>(pPacketLength);

				std::string addressSender = packet->getAddress();

				unsigned int clientID;
				if (!this->addClient(addressSender.c_str(), clientID))
				{
					this->sendDisconnectPacket(addressSender.c_str(), false);
					return;
				}

				// Print out that a user exists
				std::cout << "Client " << clientID << " has joined from " << addressSender << '\n';
				std::cout << "There are " << this->mpGameState->clients.size() << " clients\n";
				
				this->mpGameState->clients[clientID]->passedHandshake = false;

				PacketID pPacketClientID[1];
				pPacketClientID->id = MessageIDs::HandshakeClientID;
				pPacketClientID->dataID = clientID;
				this->sendPacket(addressSender.c_str(), pPacketClientID, false);
				
			}
			break;
		case MessageIDs::HandshakeAccept:
			{
				//unsigned int pPacketLength = 0;
				//PacketID* pPacket = packet->getPacketAs<PacketID>(pPacketLength);
				
				// Ok, they are good, the client side will start processing game updates
			}
			break;
		case ChampNetPlugin::ID_CLIENT_MISSING:
			std::cout << "Connection lost\n";
			// TODO: Do something here, else the client will remain in game state
			break;
		case MessageIDs::Disconnect:
			{
				unsigned int pPacketLength = 0;
				PacketID* pPacket = packet->getPacketAs<PacketID>(pPacketLength);
				std::cout << "Client " << pPacket->dataID << " has disconnected\n";
				this->removeClient(pPacket->dataID);
				std::cout << "There are " << this->mpGameState->clients.size() << " clients\n";
			}
			break;
			/*
		case ChampNetPlugin::ID_CLIENT_LEFT:
			// A user is leaving / has left the server
			{
				unsigned int pPacketLength = 0;
				PacketUserID* pPacket = packet->getPacketAs<PacketUserID>(pPacketLength);
				unsigned int clientID = pPacket->dataID;

				// Report out that the user left
				std::cout << "Client " << clientID << " has left\n";
								
				this->removeClient(clientID);

				// Notify all other clients that the user left
				this->sendPacket(packet->getAddress().c_str(), pPacket, true);
			}
			break;
		case ChampNetPlugin::ID_PLAYER_REQUEST_MOVEMENT:
			// A user's position/rotation is being updated
			{
				unsigned int pPacketLength = 0;
				PacketPlayerPosition* pPacket = packet->getPacketAs<PacketPlayerPosition>(pPacketLength);
				unsigned int clientID = pPacket->clientID;
				unsigned int playerID = pPacket->playerID;

				// Integrate the position change into the gamestate
				this->mpGameState->players[playerID].posX = pPacket->posX;
				this->mpGameState->players[playerID].posY = pPacket->posY;
				this->mpGameState->players[playerID].velY = pPacket->velY;
				this->mpGameState->players[playerID].velX = pPacket->velX;

				//std::cout << "Integrated " << this->mpGameState->players[playerID].posX << " " << this->mpGameState->players[playerID].posY << '\n';

				// ship gamestate back to all clients
				//this->sendGameState(ChampNetPlugin::ID_UPDATE_GAMESTATE);
			}
			break;
		case ChampNetPlugin::ID_CLIENT_REQUEST_PLAYER:
			{
				std::string addressSender = packet->getAddress();

				// The client wants to spawn another player
				unsigned int pPacketLength = 0;
				PacketClientPlayerID* pPacket = packet->getPacketAs<PacketClientPlayerID>(pPacketLength);
				unsigned int clientID = pPacket->clientID;
				unsigned int localID = pPacket->playerID;

				std::cout << "Client " << clientID <<
					" has requested another player for localID " << localID << '\n';

				// Get the next playerID
				unsigned int playerID;
				if (!this->addPlayer(clientID, localID, playerID, "", 0, 0, 0))
				{
					std::cout << "Could not provide another player to client|local="
						<< clientID << "|" << localID << '\n';
					return;
				}

				//this->sendGameState(ChampNetPlugin::ID_UPDATE_GAMESTATE);
			}
			break;
		case ChampNetPlugin::ID_PLAYER_ADD_MONSTER:
			{
				unsigned int pPacketLength = 0;
				PacketUserIDDouble* pPacket = packet->getPacketAs<PacketUserIDDouble>(pPacketLength);
				unsigned int playerID = pPacket->playerIdSender;
				unsigned int monsterID = pPacket->playerIdReceiver;
				// Add monsterID to the monsters for playerID
				{
					// get the old stats
					int oldMonstersCount = mpGameState->players[playerID].monstersCount;
					unsigned int *oldMonsters = mpGameState->players[playerID].monsters;
					// increment the count (adding 1)
					mpGameState->players[playerID].monstersCount++;
					// create new stats
					unsigned int *newMonsters = new unsigned int[mpGameState->players[playerID].monstersCount];
					// copy old stats
					for (int iMonster = 0; iMonster < oldMonstersCount; iMonster++)
						newMonsters[iMonster] = oldMonsters[iMonster];
					// add new id
					newMonsters[oldMonstersCount] = monsterID;
					// set new stats
					mpGameState->players[playerID].monsters = newMonsters;
					// delete old stats
					delete[] oldMonsters;
				}
			}
			break;
			//*/
			/*
		case ChampNetPlugin::ID_BATTLE_REQUEST:
			// User (playerIdSender) is requesting User (playerIdReceiver) to battle
			{
				unsigned int pPacketLength = 0;
				PacketUserIDDouble* pPacket = packet->getPacketAs<PacketUserIDDouble>(pPacketLength);

				// prevents users from moving until battle request is accepted
				this->mpGameState->players[pPacket->playerIdSender].inBattle = true;
				this->mpGameState->players[pPacket->playerIdReceiver].inBattle = true;

				// Get the address of the sender (the challenger/requester) for reporting purposes
				std::string addressSender = packet->getAddress();
				// Get the address of the receiver
				std::string *addressReceiver = this->mpClientAddresses[this->mpPlayerIdToClientId[pPacket->playerIdReceiver]];
				// Report out the request
				std::cout << "Forwarding battle request from "
					<< pPacket->playerIdSender << " at " << addressSender << " to "
					<< pPacket->playerIdReceiver << " at " << addressReceiver << '\n';
				// Send the request to the receiver
				this->sendPacket(addressReceiver->c_str(), pPacket, false);
			}
			break;
		case ChampNetPlugin::ID_BATTLE_RESPONSE:
			// User (playerIdSender) is responding to User (playerIdReceiver) to battle
			{
				unsigned int pPacketLength = 0;
				PacketBattleResponse* pPacket = packet->getPacketAs<PacketBattleResponse>(pPacketLength);

				// Get the address of the receiver (the challenger/requester)
				const char *addressSender = this->getClientAddressFrom(pPacket->playerIdSender);

				bool noReceiver = this->mpPlayerIdToClientId[pPacket->playerIdReceiver] < 0;

				if (noReceiver)
				{
					pPacket->accepted = false;
					this->sendPacket(addressSender, pPacket, false);
					return;
				}

				const char *addressReceiver = this->getClientAddressFrom(pPacket->playerIdReceiver);
				// Forward the packet - forward the response to the one who requested the battle
				this->sendPacket(addressReceiver, pPacket, false);

				if (pPacket->accepted)
				{
					// Update gamestate for in battle
					this->mpGameState->players[pPacket->playerIdSender].inBattle = true;
					this->mpGameState->players[pPacket->playerIdReceiver].inBattle = true;
					this->mpGameState->players[pPacket->playerIdSender].battleOpponentId = pPacket->playerIdReceiver;
					this->mpGameState->players[pPacket->playerIdReceiver].battleOpponentId = pPacket->playerIdSender;

					PacketBattlePromptSelection promptSelection[1];
					promptSelection->id = ChampNetPlugin::ID_BATTLE_PROMPT_SELECTION;
					promptSelection->playerAId = pPacket->playerIdReceiver;
					promptSelection->playerASelection = -1;
					promptSelection->playerAChoice = -1;
					promptSelection->playerBId = pPacket->playerIdSender;
					promptSelection->playerBSelection = -1;
					promptSelection->playerBChoice = -1;

					// Update battlers' gamestates then send battle packet
					this->sendGameState(MessageIDs::ID_UPDATE_GAMESTATE, addressSender, false);
					this->sendGameState(MessageIDs::ID_UPDATE_GAMESTATE, addressReceiver, false);
					this->sendPacket(addressSender, promptSelection, false);
					this->sendPacket(addressReceiver, promptSelection, false);
				}
				// if battle is rejected then allow both players to move again
				else
				{
					this->mpGameState->players[pPacket->playerIdSender].inBattle = false;
					this->mpGameState->players[pPacket->playerIdReceiver].inBattle = false;
				}

			}
			break;
		case ChampNetPlugin::ID_BATTLE_LOCAL_TOGGLE:
			{
				// Get Packet
				unsigned int pPacketLength = 0;
				PacketUserIDBool* pPacket = packet->getPacketAs<PacketUserIDBool>(pPacketLength);
				// Get the playerID
				unsigned int playerID = pPacket->playerID;
				// Player is telling us they have entered a local AI battle, so in battle w/o player opponent
				this->mpGameState->players[playerID].inBattle = pPacket->toggle;
				this->mpGameState->players[playerID].battleOpponentId = -1;
			}
			break;
		case ChampNetPlugin::ID_BATTLE_PROMPT_SELECTION:
			// INVALID
			break;
		case ChampNetPlugin::ID_BATTLE_SELECTION:
			{
				// Get Packet
				unsigned int pPacketLength = 0;
				PacketBattleSelection* pPacket = packet->getPacketAs<PacketBattleSelection>(pPacketLength);
				unsigned int playerID = pPacket->playerId;

				// Cache response in server-side only data
				this->mpGameState->players[playerID].lastBattleSelection = pPacket->selection;
				this->mpGameState->players[playerID].lastBattleChoice = pPacket->choice;

				// Check if other player has responded
				int opponentIDOptional = this->mpGameState->players[playerID].battleOpponentId;
				unsigned int opponentID = (unsigned int)opponentIDOptional;
				if (opponentIDOptional >= 0 &&
					// assign opponentID to the unsigned version of opponentIDOptional (which can be negative)
					// and then check if it's selection is valid (they have already submitted)
					this->mpGameState->players[opponentID].lastBattleSelection >= 0)
				{

					GameState::Player incoming = this->mpGameState->players[playerID];
					GameState::Player opponent = this->mpGameState->players[opponentID];
					const char *addressIncoming = this->getClientAddressFrom(playerID);
					const char *addressOpponent = this->getClientAddressFrom(opponentID);

					// Construct prompt selection packet with updated cache data
					PacketBattlePromptSelection promptSelection[1];
					promptSelection->id = ChampNetPlugin::ID_BATTLE_PROMPT_SELECTION;

					promptSelection->playerAId = opponent.playerID;
					promptSelection->playerASelection = opponent.lastBattleSelection;
					promptSelection->playerAChoice = opponent.lastBattleChoice;

					promptSelection->playerBId = incoming.playerID;
					promptSelection->playerBSelection = incoming.lastBattleSelection;
					promptSelection->playerBChoice = incoming.lastBattleChoice;

					// Send prompt
					this->sendPacket(addressIncoming, promptSelection, false);
					this->sendPacket(addressOpponent, promptSelection, false);

					// Invalid cached selection info
					this->mpGameState->players[playerID].lastBattleSelection = -1;
					this->mpGameState->players[playerID].lastBattleChoice = -1;
					this->mpGameState->players[opponentID].lastBattleSelection = -1;
					this->mpGameState->players[opponentID].lastBattleChoice = -1;

				}

			}
			break;
		case ChampNetPlugin::ID_BATTLE_RESULT:
			{
				// Winner is telling us they won
				unsigned int pPacketLength = 0;
				PacketUserIDTriple* pPacket = packet->getPacketAs<PacketUserIDTriple>(pPacketLength);

				// Get the address of the sender (the challenger/requester) for reporting purposes
				std::string addressSender = packet->getAddress();
				// Get the address of the receiver
				int receiverClientID = this->mpPlayerIdToClientId[pPacket->playerIdReceiver];

				// the playerID of the winner
				unsigned int winnerID = pPacket->playerIdThird;
				// will be the same if player battled AI
				if (winnerID != receiverClientID)
				{
					this->mpGameState->players[winnerID].wins++;

					// update scoreboards
					CalculateScoreBoardData();

					// NULL if they disconnected during battle
					std::string *addressReceiver = receiverClientID >= 0 ? this->mpClientAddresses[receiverClientID] : NULL;
					if (addressReceiver != NULL)
					{
						this->sendPacket(addressReceiver->c_str(), pPacket, false);
					}
				}

				// Send the request to the receiver
				this->sendPacket(addressSender.c_str(), pPacket, false);
			}
			break;
		case ChampNetPlugin::ID_BATTLE_RESULT_RESPONSE:
			{
				// Get Packet
				unsigned int pPacketLength = 0;
				PacketUserID* pPacket = packet->getPacketAs<PacketUserID>(pPacketLength);
				// Get the playerID
				unsigned int playerID = pPacket->dataID;
				// Player is telling us they have left the result dialog, so they are out of the battle
				this->mpGameState->players[playerID].inBattle = false;
				this->mpGameState->players[playerID].battleOpponentId = -1;
			}
			break;
		case ChampNetPlugin::ID_CLIENT_SCORE_UP: // used to increment a client score by 1 // proof that scoreboard works
			{

				std::string addressSender = packet->getAddress();

				unsigned int pPacketLength = 0;

				PacketPlayerScoreboardInfo* pPacket = packet->getPacketAs<PacketPlayerScoreboardInfo>(pPacketLength);
				unsigned int clientID = pPacket->clientID;
				unsigned int playerID = pPacket->playerID;

				// Integrate the win and increment by 1 into the gamestate
				this->mpGameState->players[playerID].wins = (pPacket->score + 1);

				//this->sendGameState(ChampNetPlugin::ID_UPDATE_GAMESTATE, addressSender.c_str(), true, clientID);
			}
		break;
		case ChampNetPlugin::ID_CLIENT_RANK_INTEGRATION:
		{
			std::string addressSender = packet->getAddress();

			unsigned int pPacketLength = 0;

			PacketPlayerScoreboardInfo* pPacket = packet->getPacketAs<PacketPlayerScoreboardInfo>(pPacketLength);
			unsigned int clientID = pPacket->clientID;
			unsigned int playerID = pPacket->playerID;

			// Integrate the win and increment by 1 into the gamestate
			this->mpGameState->players[playerID].rank = (pPacket->score);

			//this->sendGameState(ChampNetPlugin::ID_UPDATE_GAMESTATE, addressSender.c_str(), true, clientID);
		}
		break;
		//*/
		default:
			std::cout << "Received packet with id " << packet->getID() << " with length " << packet->getDataLength() << '\n';
			break;
	}
}

void StateServer::updateGame()
{
	StateApplication::updateGame();

	// get update time in MS
	if (this->mpTimers->getElapsedTime("update") >= this->mMsPerUpdate)
	{
		this->sendGameState(MessageIDs::UpdateGamestate);
		//std::cout << "Update " << time(NULL) << '\n';
		this->mpTimers->stopTracking("update");
		this->mpTimers->clearTracker("update");
	}

}

void StateServer::render()
{

	// The current line being typed
	std::string input = this->mpState->mInput.currentLine;
	// The previous size of the line being typed
	// will be the same as input.length() when no input has changed
	// will be -1 if the line was inputted this update
	// will be input.length() - 1 if there was a letter added this update
	// will be input.length() + 1 if there was a letter removed this update
	unsigned int inputSizePrev = this->mpState->mInput.lineSizePrevious;

	// The current position of the cursor (the next letter will be written here)
	int cursorPosX = this->mpState->mConsole.cursorPosX;
	int cursorPosY = this->mpState->mConsole.cursorPosY;

}

/**
 * Send packet data to the network
 */
void StateServer::sendPacket(const char *address, char *data, int dataSize, bool broadcast)
{
	PacketPriority priority = HIGH_PRIORITY;
	PacketReliability reliability = RELIABLE;
	ChampNetPlugin::SendData(address, data, dataSize, &priority, &reliability, 0, broadcast);
}

/**
* Finds the next available address slot, returning -1 if none is found
*/
bool StateServer::findNextClientID(unsigned int &id)
{
	for (unsigned int i = 0; i < this->mpState->mNetwork.maxClients; i++)
	{
		if (this->mpClientAddresses[i] == NULL)
		{
			id = i;
			return true;
		}
	}
	return false;
}

bool StateServer::addClient(const char* address, unsigned int &id)
{
	// Generate ID for user
	if (!this->findNextClientID(id))
	{
		// Some invalid ID was found
		std::cout << "ERROR: Server is full, but another user has connected - disconnecting new user\n";
		return false;
	}
	
	// Set the id in the list with the new address
	this->mpClientAddresses[id] = new std::string(address);

	this->mpGameState->addClient(id);

	return true;
}

void StateServer::removeClient(unsigned int clientID)
{
	this->mpGameState->removeClient(clientID);

	if (this->mpClientAddresses[clientID] != NULL)
	{
		delete this->mpClientAddresses[clientID];
		this->mpClientAddresses[clientID] = NULL;
	}
}

void StateServer::sendGameState(unsigned char msgID, const char* sender, bool broadcast, int clientID)
{
	int dataLength;
	char *data = this->mpGameState->serializeForClient(msgID, dataLength, this->mClientCount);
	this->sendPacket(sender, data, dataLength, broadcast);
	delete data;
}
