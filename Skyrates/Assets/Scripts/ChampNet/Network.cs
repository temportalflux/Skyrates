/*
Names and ID: Christopher Brennan: 1028443, Dustin Yost: 0984932, Jacob Ruth: 0890406
Course Info: EGP-405-01 
Project Name: Project 3: Synchronized Networking
Due: 11/22/17
Certificate of Authenticity (standard practice): “We certify that this work is entirely our own.  
The assessor of this project may reproduce this project and provide copies to other academic staff, 
and/or communicate a copy of this project to a plagiarism-checking service, which may retain a copy of the project on its database.”
*/
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

/// \addtogroup client
/// @{

namespace ChampNetPlugin
{

    /// <summary>
    /// The message IDs accessible by client. Same as the server Message IDs.
    /// </summary>
    public enum MessageIDs
    {
        // RakNet Messages (used for clients)
        // Send to client from server on client connection
        ID_CLIENT_CONNECTION_ACCEPTED = 16,
        ID_CLIENT_CONNECTION_REJECTED = 17,
        ID_CONNECTION_LOST = 22,

        // Client-Sent Messages
        // C->S: ask server to join
        ID_CLIENT_JOINED = 135,
        // C->S: ask server to move player
        ID_PLAYER_REQUEST_MOVEMENT,
        //! C->: ask server to add monster to player
        ID_PLAYER_ADD_MONSTER, // PacketUserIDDouble

        // Battle Messages
        // C->S: request a battle with some other player
        // S->B: forwarded packet for request
        ID_BATTLE_REQUEST,
        // B->S: accept or deny battle with some requesting player
        // S->A: forwarded packet for response (mainly for denial)
        ID_BATTLE_RESPONSE,
        // S->AB: prompt battle actions from A and B
        ID_BATTLE_PROMPT_SELECTION,
        // AB->S: Respond to PROMPT_SELECTION with a selection for battle
        ID_BATTLE_SELECTION,
        //! S->A: Notify A that B disconnected during battle
        ID_BATTLE_OPPONENT_DISCONNECTED,
        // S->AB: notify clients of outcome of battle
        ID_BATTLE_RESULT,
        // AB->S: Tell server the client has acknowledged the battle result and is return to normal space
        ID_BATTLE_RESULT_RESPONSE,
        // C->S: Tell server the client has engaged in a local battle with AI
        ID_BATTLE_LOCAL_TOGGLE,

        // Server-Sent Messages
        // 2) Sent to clients to notify them of the values for some spawning user
        // Sender uses to place self, peers use to place a dummy unit
        ID_UPDATE_GAMESTATE,
        // 3) Sent to clients to mandate their ID
        ID_CLIENT_REQUEST_PLAYER,
        // Sent to server and forwarded to clients notifying them a user has left the server
        ID_CLIENT_LEFT,
        // Notification to clients that the server has been disconnected
        ID_DISCONNECT,

        // Increment Client Score by 1
        ID_CLIENT_SCORE_UP,
        // Rank Up
        ID_CLIENT_RANK_UPDATE,
    }

    /// <summary>
    /// A Unity interface with the C++ plugin under <see cref="ChampNet::Network"/> and <see cref="ChampNetPlugin"/>.
    /// </summary>
    public class Network
    {

        /// <summary>
        /// The DLL identifier
        /// </summary>
        const string IDENTIFIER = "ChampNetPlugin";

        public delegate void debugCallback(IntPtr request, int color, int size);
        enum Color { red, green, blue, black, white, yellow, orange };

        /// Create a network to connect with
        [DllImport(IDENTIFIER)]
        public static extern int Create();

        /// <summary>
        /// Register a callback function for console logs
        /// </summary>
        /// <param name="callback"></param>
        [DllImport(IDENTIFIER, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterDebugCallback(debugCallback callback);

        /// <summary>
        /// Sets up the debug callback in <see cref="RegisterDebugCallback(debugCallback)"/>
        /// </summary>
        public static void SetDebugCallback()
        {
            RegisterDebugCallback(OnDebugCallback);
        }

        /// <summary>
        /// Handles marshalling string console logs
        /// </summary>
        /// <param name="request"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        [MonoPInvokeCallback(typeof(debugCallback))]
        static void OnDebugCallback(IntPtr request, int color, int size)
        {
            //Ptr to string
            string debug_string = Marshal.PtrToStringAnsi(request, size);

            //Add Specified Color
            debug_string =
                String.Format("{0}{1}{2}{3}{4}",
                "<color=",
                ((Color)color).ToString(),
                ">",
                debug_string,
                "</color>"
                );

            UnityEngine.Debug.Log(debug_string);
        }

        /// Destroy the network (must call Create prior) (must call when owning object is destroyed)
        [DllImport(IDENTIFIER)]
        public static extern int Destroy();

        /// Start a server with the specified credentials using this object
        [DllImport(IDENTIFIER)]
        public static extern int StartServer(int port, int maxClients);

        /// Start a client using this object
        [DllImport(IDENTIFIER)]
        public static extern int StartClient();

        /// Connect this CLIENT to some server using the specified credentials
        [DllImport(IDENTIFIER)]
        public static extern int ConnectToServer(string address, int port);

        /// Fetch all incoming packets. Must call prior to PollPacket. Must be called after Create and before Destroy. Returns the quantity of packets in the queue after fetch.
        [DllImport(IDENTIFIER)]
        public static extern int FetchPackets();

        /// Poll the packet queue. Returns a pointer to the first packet, and removes that packet from the queue. If valid is true, then the packet can be processed, else the packet does not exist (no packets presently in the queue).
        [DllImport(IDENTIFIER)]
        public static extern IntPtr PollPacket(ref bool valid);

        /// Returns the packet's address, given some valid packet pointer (Call after PollPacket if valid is true).
        [DllImport(IDENTIFIER)]
        public static extern IntPtr GetPacketAddress(IntPtr packetRef, ref uint length);

        /// Returns the packet's data, given some valid packet pointer (Call after PollPacket if valid is true).
        [DllImport(IDENTIFIER)]
        public static extern IntPtr GetPacketData(IntPtr packetRef, ref uint length, ref ulong transmitTime);

        /// Frees the memory of some packet, given some valid packet pointer (Call after PollPacket if valid is true).
        [DllImport(IDENTIFIER)]
        public static extern void FreePacket(IntPtr packetRef);

        /// Send a byte array to the server
        [DllImport(IDENTIFIER)]
        public static extern void SendByteArray(string address, int port, byte[] byteArray, int byteArraySize);

        /// WRAPPER METHOD
        /// Handles polling the network for packets, and returning the address and data of that packet.
        /// Use instead of PollPacket(bool), GetPacketAddress, GetPacketData, and FreePacket
        /// Copies out the data from a valid packet, and frees the packet from memory.
        /// Returns true if a valid packet was found, else false.
        public static bool PollPacket(out string address, out byte[] data, out ulong transmitTime)
        {

            // Ensure the out params have some value (invalid at start)
            address = null;
            data = null;
            transmitTime = 0;

            // Determine if there are valid packets in the buffer.
            bool foundPacket = false;
            IntPtr packetRef = PollPacket(ref foundPacket);

            // If a valid packet has be dequeued, copy out the data.
            if (foundPacket)
            {
                //Debug.Log("Got packet ptr " + packetRef);

                // Get the address
                uint addressLength = 0;
                try
                {
                    IntPtr strPtr = GetPacketAddress(packetRef, ref addressLength);
                    address = Marshal.PtrToStringAnsi(strPtr);
                    //Debug.Log("Got address " + address);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                // Get the data
                uint dataLength = 0;
                IntPtr ptrData = GetPacketData(packetRef, ref dataLength, ref transmitTime);

                data = new byte[dataLength];
                Marshal.Copy(ptrData, data, 0, (int)dataLength);
                // Data is now possessed by C#
                
                // Free the packet - all data is copied over
                FreePacket(packetRef);
            }

            // Return if a valid packet was found
            return foundPacket;
        }

        /// Disconnect from the interface
        [DllImport(IDENTIFIER)]
        public static extern void Disconnect();

    }

}
/// @}
