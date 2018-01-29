
#include <iostream>

#include "Game.h"
#include "StateServer.h"
#include "StateData.h"

#include <stdlib.h>
#include <time.h>

using namespace std;

/// \defgroup server Server: Cretins

int main() {
	cout << "Starting ChampNet server...\n";
	srand(time(NULL));

	// Get server information
	unsigned int port, maxClients;
	// Port
	std::cout << "Enter port: ";
	std::cin >> port;
	// Max Clients
	std::cout << "Enter max clients: ";
	std::cin >> maxClients;

	// Create a server
	Game *pGame = new Game(new StateServer());
	pGame->getData()->mNetwork.port = port;
	pGame->getData()->mNetwork.maxClients = maxClients;

	// Run the loop for the game
	while (pGame->isRunning())
	{
		pGame->update();
	}

	// Delete the server
	delete pGame;
	pGame = NULL;

	system("pause");
	return 0;
}
