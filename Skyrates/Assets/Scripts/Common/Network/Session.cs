using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Server.Network;
using UnityEngine;

namespace Skyrates.Common.Network
{

    [CreateAssetMenu(menuName = "Data/Session")]
    public class Session : ScriptableObject
    {

        /// <summary>
        /// The IPv4 address of this peer (client or server).
        /// </summary>
        public string Address;

        /// <summary>
        /// The IPv4 address of this peer (client or server).
        /// </summary>
        public string TargetAddress;

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
        
        public void Init()
        {
            this.SetAddressBoth("");
            this.Port = 425;
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
        
    }


}
