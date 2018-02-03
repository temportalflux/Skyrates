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
struct PacketID
{
	/// The packetID
	unsigned char id;
	/// Some identifier
	unsigned int dataID;
};

/// @}
