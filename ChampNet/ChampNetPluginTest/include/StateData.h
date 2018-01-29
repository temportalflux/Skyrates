#ifndef CHAMPNET_SERVER_STATE_DATA_H
#define CHAMPNET_SERVER_STATE_DATA_H

#include <string>

/// \addtogroup server
/// @{

/// The state of the input keys
struct StateInput
{
	/// The max size of the keyboard arrays
	static const unsigned int SIZE_KEYBOARD = 256;

	/// The current keyboard state
	char keyboard[SIZE_KEYBOARD];
	/// The previous keyboard state
	char previous[SIZE_KEYBOARD];

	/// If the input is currently treated as the CAPS lock being on
	bool isCaps;
	/// If empty lines are enabled on ENTER
	bool allowEmptyLines;

	/// The current line of text (before pressing ENTER)
	std::string currentLine;
	/// The previous size of the currentLine (-1 when the line is cleared)
	unsigned int lineSizePrevious;

};

/// The state of the console window
struct StateConsole
{

	/// A pointer to the console window
	void *consoleWindow;

	/// The current cursor position in the console
	int cursorPosX, cursorPosY;

};

/// The state of the network data
struct StateNetwork
{

	/// The port the network is connected to
	unsigned int port;
	/// The maximum number of clients for the server
	unsigned int maxClients;
	/// The number of peers connected to the server
	unsigned int peersConnected;
	/// The maximum number of players allowed to connected client peer
	const unsigned int maxPlayersPerClient = 4;

};

/// The data state of all local data
struct StateData
{
	StateInput mInput;
	StateConsole mConsole;
	StateNetwork mNetwork;
};

/// @}

#endif // CHAMPNET_SERVER_STATE_DATA_H