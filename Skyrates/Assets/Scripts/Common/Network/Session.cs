using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyrates.Common.Network
{

    [CreateAssetMenu(menuName = "data/Session")]
    public class Session : ScriptableObject
    {

        /// <summary>
        /// Various network modes a game can be in.
        /// </summary>
        public enum NetworkMode
        {
            /// <summary>
            /// Game has not started.
            /// </summary>
            None,

            /// <summary>
            /// No networking.
            /// </summary>
            Standalone,

            /// <summary>
            /// Joiner of game: Client.
            /// </summary>
            Client,

            /// <summary>
            /// Host of a game: Acts as Server On Top of Client.
            /// </summary>
            Host,
        }

        /// <summary>
        /// The IPv4 address of this peer (client or server).
        /// </summary>
        public string Address;

        /// <summary>
        /// The IPv4 address of this peer (client or server).
        /// </summary>
        public string TargetAddress;

        /// <summary>
        /// Identifier which stores what type of game is being run (network wise).
        /// </summary>
        public NetworkMode Mode;

        /// <summary>
        /// The port of the address.
        /// </summary>
        public int Port;

        /// <summary>
        /// The maximum amount of clients allowed to connect to the server.
        /// Only used when Mode == Host.
        /// </summary>
        public int MaxClients = 10;

        /// <summary>
        /// Only used when Mode == Host
        /// </summary>
        public float ServerTickUpdate;

        /// <summary>
        /// The local client ID. Will be INVALID if Mode == Host (host does not have a clientID, it just is).
        /// </summary>
        public uint ClientID = 0;

        /// <summary>
        /// If this client has successfully authenticated with the server.
        /// </summary>
        public bool HandshakeComplete = false;

        /// <summary>
        /// Can this session be considered valid data? True only if the game has started (user has entered world scene)
        /// </summary>
        public bool IsValid
        {
            get { return this.Mode != NetworkMode.None; }
        }

        /// <summary>
        /// If the game session is networked or not (if it is, it is also a client)
        /// </summary>
        public bool IsNetworked
        {
            get { return this.Mode != NetworkMode.Standalone; }
        }

        public void Init()
        {
            this.Mode = Session.NetworkMode.None;
            this.SetAddressBoth("");
            this.Port = 0;
            this.SetClientID(uint.MaxValue); // mark as something horrible instead of -1
            this.HandshakeComplete = false;
        }

        public bool HasValidClientID()
        {
            return this.ClientID < uint.MaxValue;
        }

        public void SetAddress(string address)
        {
            this.Address = address;
        }

        public void SetTargetAddress(string address)
        {
            this.TargetAddress = address;
        }

        public void SetAddressBoth(string address)
        {
            this.SetAddress(address);
            this.SetTargetAddress(address);
        }

        public void SetAddressFrom(UnityEngine.UI.InputField field)
        {
            this.SetAddress(field.text);
        }

        public void SetTargetAddressFrom(UnityEngine.UI.InputField field)
        {
            this.SetTargetAddress(field.text);
        }

        public void SetPortFrom(UnityEngine.UI.InputField field)
        {
            int.TryParse(field.text, out this.Port);
        }

        public void SetClientID(uint id)
        {
            this.ClientID = id;
        }

    }


}
