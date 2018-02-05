#include "Packet.h"

#include <stdlib.h>     /* srand, rand */
#include <time.h>       /* time */

/** Author: Dustin Yost
* A class to handle packet data from RakNet
*/
namespace ChampNet // Packet
{

	Packet::Packet(const unsigned int lengthAddress, const char* address, const unsigned int dataLength, const unsigned char* data)
	{
		mAddressLength = lengthAddress;
		mAddress = address;
		mDataLength = dataLength;
		// Copy the data, thereby creating a new pointer array
		copy(data, mData, dataLength);
	}

	Packet::~Packet()
	{
		mAddressLength = 0;
		mDataLength = 0;
		mAddress = NULL;
		// Data is assumed to have been copied, and in doing so, created a new pointer
		delete[] mData;
		mData = NULL;
	}

	// Create a new char[] at dest, and copies the contents of the range [0, length) from source into dest
	void Packet::copy(const unsigned char* &source, unsigned char* &dest, unsigned int length)
	{
		// Allocate the memory for the data copy
		dest = new unsigned char[length];
		// Linearly copy all data from source into dest from 0 to length
		for (unsigned int i = 0; i < length; i++)
		{
			dest[i] = source[i];
		}
	}

	// Sets address and length to the address characters of the ip address
	void Packet::getAddress(const char* &address, unsigned int &length)
	{
		address = this->mAddress;
		length = this->mAddressLength;
	}

	std::string Packet::getAddress()
	{
		const char *address;
		unsigned int length;
		this->getAddress(address, length);
		return std::string(address);
	}

	// Sets data and length to the byte data from the packet
	void Packet::getData(unsigned char* &data, unsigned int &length)
	{
		data = this->mData;
		length = this->mDataLength;
	}

}
namespace ChampNet // PacketQueue
{

	PacketQueue::PacketQueue()
	{
		mHead = NULL;
		mTail = NULL;
	}

	PacketQueue::~PacketQueue()
	{
		this->clear();
	}

	void PacketQueue::enqueue(Packet* data)
	{
		// Add node to end of list

		// If list empty, make the first node
		if (mHead == NULL)
		{
			// Set to both beginning and end
			mHead = data;
			mTail = data;
		}
		else
		{
			// Not the first, add to last node and set as last
			mTail->mNext = data;
			mTail = data;
		}

		mCount++;

	}

	void PacketQueue::dequeue(Packet *&data)
	{
		// Always ensure data is wiped on entrance
		data = NULL;

		// Check if the list is empty
		if (!isEmpty())
		{
			// Get the first element in the queue
			data = mHead;

			// Set the first element to the next element
			mHead = mHead->mNext;

			// Set the next element of the previous start to NULL (no longer a part of the list)
			data->mNext = NULL;

			// If the head is now empty, make sure to empty the tail
			if (mHead == NULL)
			{
				mTail = NULL;
			}

			mCount--;

		}

	}

	// Deallocates all packets
	void PacketQueue::clear()
	{
		Packet* data;
		while (!isEmpty())
		{
			dequeue(data);
			delete data;
		}
	}

	// Returns true if there are no packets in the queue (count == 0 / mHead == NULL)
	bool PacketQueue::isEmpty()
	{
		return mHead == NULL;
	}

}
