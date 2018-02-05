#ifndef _CHAMPNET_PACKET_H
#define _CHAMPNET_PACKET_H

#include <string>
#include <RakNet\RakNetTime.h>

/** \addtogroup champnet
* @{
*/

namespace ChampNet
{

	//! Holds TimeStamped information which wraps RakNet::Time for usage for cross-network & forwarded packet time tracking
	struct TimeStamp
	{
		//! If the timestamps were found in the packet
		bool timesLoaded = false;
		//! The lcoal time that the packet was read at
		RakNet::Time packetReadTime_local = 0;
		//! The local time duration the packet took to start reading
		RakNet::Time readDiff_local = 0;
		//! The remote time that the packet was sent at
		RakNet::Time sentTime_remote = 0;
		//! The total time (local) that the packet took to be received/read
		RakNet::Time totalTransferTime_local = 0;
	};

	//! A class to handle packet data from RakNet
	class Packet
	{
		friend class PacketQueue;
		friend class Network;

	private:
		//! The character length of the address
		unsigned int mAddressLength;
		//! A pointer to the characters of the address
		const char* mAddress;
		//! The length of bytes in the data
		unsigned int mDataLength;
		//! The dynamic array of packet data
		unsigned char* mData;

		//! The next packet in the <see cref="ChampNet::PacketQueue"\>
		Packet* mNext;

		/** @brief Constructor
		* @param lengthAddress the total length of the address string
		* @param address the address from which this packet came
		* @param dataLength the total number of bytes in the packet
		* @param data the byte[] of data
		*/
		Packet(const unsigned int lengthAddress, const char* address, const unsigned int dataLength, const unsigned char* data);

	public:

		//! The timestamp information about this packet
		TimeStamp timestampInfo;

		//! Destructor
		~Packet();

		//! Create a new char[] at dest, and copies the contents of the range [0, length) from source into dest
		void copy(const unsigned char* &source, unsigned char* &dest, unsigned int length);

		//! Returns the ID of this packet (assumed the first byte of data in the packet)
		inline unsigned int getID() const { return this->mData[0]; }

		//! Sets address and length to the address characters of the ip address
		void getAddress(const char* &address, unsigned int &length);
		//! Returns the const char* address as a string (wraps getAddress(const char*, unsigned int))
		std::string getAddress();

		//! Sets data and length to the byte data from the packet
		void getData(unsigned char* &data, unsigned int &length);
		//! Returns the data length (same thing as length in getData)
		inline unsigned int getDataLength() const { return mDataLength; }

		//! Casts the packet data to some structure T, returning the length of data as a reference parameter
		template <typename T>
		T* getPacketAs(unsigned int &dataSize)
		{
			unsigned char *data;
			this->getData(data, dataSize);
			return (T*)data;
		}

		//! Returns the total ms it took to transmit this packet from its original source to its final destination
		inline uint64_t getTransmitTime() const { return this->timestampInfo.timesLoaded ? this->timestampInfo.totalTransferTime_local : 0; }

	};

	/// A class to handle a queue of Packets (wrapped RakNet packets).
	/// Operates as a true queue (sub-structure of LinkedList).
	/// \author Dustin Yost
	class PacketQueue
	{

	private:
		//! The head of the linked list of packets
		Packet* mHead;
		//! The tail of the linked list
		Packet* mTail;
		//! The quantity of packets in the queue
		int mCount;

	public:
		//! Constructor
		PacketQueue();
		//! Destructor
		~PacketQueue();

		//! Pushes a packet onto the end of the list
		void enqueue(Packet* packet);

		//! Pops a packet from the front of the list
		//! Parameter will be set with the first packet
		void dequeue(Packet *&packet);

		//! Deallocates all packets
		void clear();

		//! Returns true if there are no packets in the queue (count == 0)
		bool isEmpty();

		//! Returns the quantity of packets in the queue
		inline int getCount() const { return mCount; };

	};

}
/** @} */

#endif // _CHAMPNET_PACKET_H