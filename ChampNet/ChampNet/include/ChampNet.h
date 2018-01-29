#ifndef _CHAMPNET_MAIN_H
#define _CHAMPNET_MAIN_H

#include <string>
#include <RakNet\RakNetTime.h>

namespace RakNet
{
	class RakPeerInterface;
	struct SystemAddress;
}
enum PacketPriority;
enum PacketReliability;

/** \defgroup champnet ChampNet */

/** \addtogroup champnet
* @{
*/
//! ChampNet wraps all RakNet operations with easy to manage Packets and Network interfaces
namespace ChampNet
{

	class PacketQueue;
	class Packet;
	struct TimeStamp;

	//! The total size of a timestamp addition
	//! \author Jake Ruth
	const int SIZE_OF_TIMESTAMPS = sizeof(char) + sizeof(RakNet::Time) + sizeof(RakNet::Time);

	//! Base class for handling all <see cref="ChampNet::Packet"/> data.
	class Network
	{
	public:
		//! Function delegate prototype for output callbacks
		typedef void(*MsgCallBack)(const char* message, int color, int size);

		//! Type Definition for <see cref="ChampNet::Packet"/> data
		typedef char* Data;
		//! Type Definition for <see cref="ChampNet::Packet"/> data size
		typedef unsigned int DataSize;

	private:
		//! The interface used to connect to another computer
		RakNet::RakPeerInterface *mpPeerInterface;
		//! All the packets to be processed
		PacketQueue* mpPackets;

	public:

		//! The callback for sending logs
		MsgCallBack logger = NULL;

		//! Output to console through the <see cref="Network::logger"/>
		void sendLog(const char *msg, int color);

		Network();
		~Network();

		//! Startup the server interface
		void initServer(const int port, const int maxClients);
		//! Startup the client interface
		void initClient();
		//! Connect the interface to its destination
		void connectToServer(const std::string &address, const int port);

		//! Fetch the address the peer is bound to
		// TODO: Encapsulation Leek
		void queryAddress(RakNet::SystemAddress *address);

		//! Return the IP string from the peer
		std::string getIP();

		//! Returns true if the network interface (RakNet thread) is active
		bool isActive();

		//! Returns the total number of packets currently cached
		int getPacketCount() const;

		//! Shutdown the peer interface
		void disconnect();

		//! Send packet data over RakNet
		// TODO: Encapsulation Leek
		void sendTo(Data data, DataSize size,
			RakNet::SystemAddress *address,
			PacketPriority *priority, PacketReliability *reliability,
			char channel, bool broadcast, bool timestamp, const TimeStamp *timestampInfo = NULL
		);

		//! Handle sending struct packets to RakNet address
		// TODO: Encapsulation Leek
		template <typename T>
		void sendTo(T packet,
			RakNet::SystemAddress *address,
			PacketPriority *priority, PacketReliability *reliability,
			char channel, bool broadcast, bool timestamp
		)
		{
			// Package up the packet
			Data data = (Data)(&packet);
			DataSize size = sizeof(packet);
			// Send via RakNet
			this->sendTo(data, size, address, priority, reliability, channel, broadcast, timestamp);
		}

		//! Handle sending struct packets to RakNet address
		// TODO: Encapsulation Leek
		template <typename T>
		void sendTo(T packet, RakNet::SystemAddress *address)
		{
			this->sendTo(packet, address, HIGH_PRIORITY, RELIABLE_ORDERED, 0, false, true);
		}

		//! Cache all incoming packets (should be run regularly)
		void fetchAllPackets();

		//! Poll the next cached packet
		//! Returns true if a packet was found;
		bool pollPackets(Packet *&nextPacket);

		//! Write the time stamp to a buffer
		int writeTimestamps(char *buffer, const RakNet::Time &time1, const RakNet::Time &time2);
		//! Read the time stamp from a buffer
		int readTimestamps(const char *buffer, RakNet::Time &time1, RakNet::Time &time2);

	};

};
/** @} */

#endif // _CHAMPNET_MAIN_H