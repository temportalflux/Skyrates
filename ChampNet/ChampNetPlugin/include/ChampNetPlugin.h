#ifndef _CHAMPNET_PLUGIN_PLUGIN_H
#define _CHAMPNET_PLUGIN_PLUGIN_H

// defines CHAMPNET_PLUGIN_SYMTAG
#include "lib.h"

// Include raknet message identifiers
#include <RakNet\MessageIdentifiers.h>
#include <RakNet\RakNetTime.h>

/** \defgroup plugin ChampNetPlugin */

// tell compiler to link as if all function are C not C++
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	enum PacketPriority;
	enum PacketReliability;

	/// \addtogroup plugin
	/// @{
	
	/// Wraps <see cref="ChampNet"\> for usage as a Unity Plugin and static dll compilation
	/// \author Dustin Yost
	namespace ChampNetPlugin {

		/// All the Message/Packet identifiers used for this implementation
		// TODO: Not DLL general - should be moved for just server usage (irrellevant to non-Cretin usage)
		// Designations:
		// C: Client
		// S: Server
		// A: Client A
		// B: Client B
		// all: all Clients
		enum MessageIDs
		{
			//! RakNet messages (unsued for clients)
			ID_CLIENT_CONNECTION = ID_NEW_INCOMING_CONNECTION,
			//! The connection was severed
			ID_CLIENT_DISCONNECTION = ID_DISCONNECTION_NOTIFICATION,
			//! The connection was dropped
			ID_CLIENT_MISSING = ID_CONNECTION_LOST,

			//! RakNet Messages (used for clients)
			ID_CLIENT_CONNECTION_ACCEPTED = ID_CONNECTION_REQUEST_ACCEPTED,
			ID_CLIENT_CONNECTION_REJECTED = ID_CONNECTION_ATTEMPT_FAILED,

			//! placeholder - For tracking in C# script
			ID_PLACEHOLDER = ID_USER_PACKET_ENUM,

			// Client-Sent Messages
			//! C->S: ask server to join
			ID_CLIENT_JOINED,
			//! C->S: ask server to move player
			ID_PLAYER_REQUEST_MOVEMENT,
			//! C->: ask server to add monster to player
			ID_PLAYER_ADD_MONSTER, // PacketUserIDDouble

			// Battle Messages
			//! C->S: request a battle with some other player
			//! S->B: forwarded packet for request
			ID_BATTLE_REQUEST, // PacketUserIDDouble
			//! B->S: accept or deny battle with some requesting player
			//! S->A: forwarded packet for response (mainly for denial)
			ID_BATTLE_RESPONSE, // PacketBattleResponse
			//! S->AB: prompt battle actions from A and B
			ID_BATTLE_PROMPT_SELECTION, // PacketBattlePromptSelection
			//! AB->S: Respond to PROMPT_SELECTION with a selection for battle
			ID_BATTLE_SELECTION, // PacketBattleSelection
			//! S->A: Notify A that B disconnected during battle
			ID_BATTLE_OPPONENT_DISCONNECTED,
			//! A(winner)->S: tell server I won
			//! S->AB: notify clients of outcome of battle
			ID_BATTLE_RESULT, // PacketUserIDTriple
			//! AB->S: Tell server the client has acknowledged the battle result and is return to normal space
			ID_BATTLE_RESULT_RESPONSE, // PacketUserID
			//! C->S: Tell server the client has engaged in a local battle with AI
			ID_BATTLE_LOCAL_TOGGLE, // PacketUserIDBool

			// Server-Sent Messages
			//! 2) Sent to clients to notify them of the values for some spawning user
			//! Sender uses to place self, peers use to place a dummy unit
			ID_UPDATE_GAMESTATE,
			//! 3) Sent to clients to mandate their ID
			ID_CLIENT_REQUEST_PLAYER,
			//! Sent to server and forwarded to clients notifying them a user has left the server
			ID_CLIENT_LEFT,
			//! Notification to clients that the server has been disconnected
			ID_DISCONNECT,

			// Increment Client Score by 1
			ID_CLIENT_SCORE_UP,
			// recieve new rank
			ID_CLIENT_RANK_INTEGRATION,

		};

		/// Create a network to connect with
		CHAMPNET_PLUGIN_SYMTAG int Create();

		typedef void(*FuncCallBack)(const char* message, int color, int size);
		enum class Color { Red, Green, Blue, Black, White, Yellow, Orange };

		/// Registers a debug callback for usage in printing logs
		CHAMPNET_PLUGIN_SYMTAG void RegisterDebugCallback(FuncCallBack callBack);

		/// Sends a message to whatever debug callback was set by <see cref="RegisterDebugCallback"/>
		static void send_log(const char *msg, const Color &color);

		/// Destroy the network (must call Create prior) (must call when owning object is destroyed)
		CHAMPNET_PLUGIN_SYMTAG int Destroy();

		/// Start a server with the specified credentials using this object
		CHAMPNET_PLUGIN_SYMTAG int StartServer(int port, int maxClients);

		/// Start a client using this object
		CHAMPNET_PLUGIN_SYMTAG int StartClient();

		/// Connect this CLIENT to some server using the specified credentials
		CHAMPNET_PLUGIN_SYMTAG int ConnectToServer(const char* address, int port);

		/// Fetch all incoming packets. Must call prior to PollPacket. Must be called after Create and before Destroy. Returns the quantity of packets in the queue after fetch.
		CHAMPNET_PLUGIN_SYMTAG int FetchPackets();

		/// Poll the packet queue. Returns a pointer to the first packet, and removes that packet from the queue. If valid is true, then the packet can be processed, else the packet does not exist (no packets presently in the queue).
		CHAMPNET_PLUGIN_SYMTAG void* PollPacket(bool &validPacket);

		/// Returns the packet's address, given some valid packet pointer (Call after PollPacket if valid is true).
		CHAMPNET_PLUGIN_SYMTAG const char* GetPacketAddress(void* packetPtr, unsigned int &length);

		/// Returns the packet's data, given some valid packet pointer (Call after PollPacket if valid is true).
		CHAMPNET_PLUGIN_SYMTAG unsigned char* GetPacketData(void* packetPtr, unsigned int &length, unsigned long &transmitTime);

		/// Frees the memory of some packet, given some valid packet pointer (Call after PollPacket if valid is true).
		CHAMPNET_PLUGIN_SYMTAG void FreePacket(void* packetPtr);

		/// Sends a byte[] of data to some address
		CHAMPNET_PLUGIN_SYMTAG void SendByteArray(const char* address, int port, char* byteArray, int byteArraySize);

		/// Sends a byte[] of data to some address uing RakNet protocols
		CHAMPNET_PLUGIN_SYMTAG void SendData(const char* address, char* byteArray, int byteArraySize, PacketPriority *priority, PacketReliability *reliability, int channel, bool broadcast);

		/// Disconnect from the interface
		CHAMPNET_PLUGIN_SYMTAG void Disconnect();

		/// Returns the current peer interface address
		CHAMPNET_PLUGIN_SYMTAG const char* GetAddress();

	};
	/// @}

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // _CHAMPNET_PLUGIN_PLUGIN_H