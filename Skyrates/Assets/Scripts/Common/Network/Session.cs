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
        /// The local client ID. Will be INVALID if Mode == Host (host does not have a clientID, it just is).
        /// </summary>
        public uint ClientID;

        /// <summary>
        /// If this client has successfully authenticated with the server.
        /// </summary>
        public bool HandshakeComplete = false;

        public void Init()
        {
            this.SetAddressBoth("");
            this.ClientID = uint.MaxValue; // marked as something horrible instead of -1
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
        
    }


}
