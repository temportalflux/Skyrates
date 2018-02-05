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

namespace ChampNetPlugin
{

    /// <summary>
    /// The message IDs accessible by client. Same as the server Message IDs.
    /// </summary>
    public enum MessageIDs
    {
        // RakNet Messages (used for clients)
        #region RakNet
        // Common messages
        CONNECTION_LOST = 22, // ID_CONNECTION_LOST
        // Send to client from server on client connection
        CLIENT_CONNECTION_ACCEPTED = 16, // ID_CONNECTION_REQUEST_ACCEPTED
        CLIENT_CONNECTION_REJECTED = 17, // ID_CONNECTION_ATTEMPT_FAILED
        // Send to server from client on client connection
        CLIENT_CONNECTION_INCOMING = 19, // ID_NEW_INCOMING_CONNECTION
        CLIENT_DISCONNECTION = 21, // ID_DISCONNECTION_NOTIFICATION
        #endregion

        NONE = 134, // ID_USER_PACKET_ENUM
        
    }

    /// <summary>
    /// A Unity interface with the C++ plugin under <see cref="ChampNet::Network"/> and <see cref="ChampNetPlugin"/>.
    /// </summary>
    public class NetworkPlugin
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

        /// Create a server with the specified credentials using this object
        [DllImport(IDENTIFIER)]
        public static extern int StartServer(int port, int maxClients);

        /// Create a client using this object
        [DllImport(IDENTIFIER)]
        public static extern int StartClient();

        /// Connect this Client to some server using the specified credentials
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

        /// Returns the packet's _gameStateData, given some valid packet pointer (Call after PollPacket if valid is true).
        [DllImport(IDENTIFIER)]
        public static extern IntPtr GetPacketData(IntPtr packetRef, ref uint length, ref ulong transmitTime);

        /// Frees the memory of some packet, given some valid packet pointer (Call after PollPacket if valid is true).
        [DllImport(IDENTIFIER)]
        public static extern void FreePacket(IntPtr packetRef);

        /// Send a byte array to the server
        [DllImport(IDENTIFIER)]
        public static extern void SendByteArray(string address, int port, byte[] byteArray, int byteArraySize);

        /// WRAPPER METHOD
        /// Handles polling the network for packets, and returning the address and _gameStateData of that packet.
        /// Use instead of PollPacket(bool), GetPacketAddress, GetPacketData, and FreePacket
        /// Copies out the _gameStateData from a valid packet, and frees the packet from memory.
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

            // If a valid packet has be dequeued, copy out the _gameStateData.
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

                // Get the _gameStateData
                uint dataLength = 0;
                IntPtr ptrData = GetPacketData(packetRef, ref dataLength, ref transmitTime);

                data = new byte[dataLength];
                Marshal.Copy(ptrData, data, 0, (int)dataLength);
                // data is now possessed by C#
                
                // Free the packet - all _gameStateData is copied over
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
