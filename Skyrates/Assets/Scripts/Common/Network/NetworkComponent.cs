﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using Skyrates.Client.Game;
using Skyrates.Client.Game.Event;
using Skyrates.Common.Network.Event;
using Skyrates.Server.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Skyrates.Common.Network
{
    
    /// <summary>
    /// A MonoBehaviour Singleton to wrap Networking
    /// </summary>
    public class NetworkComponent : Singleton<NetworkComponent>
    {

        /// <summary>
        /// The static instance of this Singleton
        /// </summary>
        public static NetworkComponent Instance;

        /// <summary>
        /// The session which is instantiated prior to running
        /// <see cref="NetworkComponent.CreateNetworkAndConnect"/>.
        /// </summary>
        public Session Session;
        
        /// <summary>
        /// The networking object, clients and client-on-top-of-servers are subclasses
        /// </summary>
        private NetworkCommon _network;

        /// <summary>
        /// Loads the Singleton and instatiates data objects
        /// </summary>
        private void Awake()
        {
            this.loadSingleton(this, ref Instance);

            this._network = null;

            this.InitData();

            this.Session.SetAddressBoth(this.GetIP());
            StartCoroutine(this.CheckIP());
        }

        void OnDestroy()
        {

            if (this._network != null)
            {
                if (GetSession.HandshakeComplete || GetSession.HasValidClientID())
                {
                    GetNetwork().DispatchAllImmediate(new EventDisconnect(GetSession.ClientID));
                }

                GetNetwork().Shutdown();
                GetNetwork().Destroy();
            }

            this.InitData();
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this._network != null && !this._network.HasSubscribed)
            {
                this._network.SubscribeEvents();
                this.SubscribeEvents();
            }
        }

        void OnSceneUnloaded(Scene scene)
        {
            if (this._network != null && this._network.HasSubscribed)
            {
                this._network.UnsubscribeEvents();
                this.UnsubscribeEvents();
            }
            this.OnEvtSceneUnloaded(scene);
        }

        // Wipes Session and GameState
        private void InitData()
        {
            this.Session.Init();
        }

        #region Static

        public static Session GetSession
        {
            get
            {
                Debug.Assert(Instance != null);
                return Instance.Session;
            }
        }
        
        public static NetworkCommon GetNetwork()
        {
            return Instance._network;
        }
        
        #endregion

        #region IP Helper

        private string GetIP()
        {
            IPAddress[] addr = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
            return addr[addr.Length - 1].ToString();
        }

        private IEnumerator CheckIP()
        {
            var myExtIPWWW = new WWW("http://checkip.dyndns.org");
            if (myExtIPWWW == null)
                yield break;
            yield return myExtIPWWW;
            var myExtIP = myExtIPWWW.text;
            myExtIP = myExtIP.Substring(myExtIP.IndexOf(":") + 1);
            myExtIP = myExtIP.Substring(1, myExtIP.IndexOf("<") - 1);
            this.Session.SetAddress(myExtIP);
        }

        #endregion

        #region Actions

        public void StartStandalone()
        {
            this.StartGame(Session.NetworkMode.Standalone);
            // Start the world asap
            SceneLoader.Instance.ActivateNext();
            this.LoadWorldNonClient();
        }

        public void StartClient()
        {
            this.StartGame(Session.NetworkMode.Client);
        }

        public void StartHost()
        {
            this.StartGame(Session.NetworkMode.Host);
            // Start the world asap
            SceneLoader.Instance.ActivateNext();
            this.LoadWorldNonClient();
        }

        private void StartGame(Session.NetworkMode mode)
        {
            SceneLoader.Instance.Enqueue(SceneData.SceneKey.LoadingWorld);
            SceneLoader.Instance.ActivateNext();

            SceneLoader.Instance.Enqueue(SceneData.SceneKey.World);

            Debug.Log("NetComp: Starting network as " + mode);

            this.Session.Mode = mode;
            this.CreateNetworkAndConnect();
            GameManager.Events.Dispatch(new EventGameStart(mode));
        }

        #endregion

        #region Start Network

        public void CreateNetworkAndConnect()
        {
            Debug.Assert(this.Session.IsValid);

            // Create the network
            this._network = this.CreateNetwork();
            this._network.Create();
            // Start it up & Connect
            this._network.StartAndConnect(this.Session);
        }

        private NetworkCommon CreateNetwork()
        {
            switch (this.Session.Mode)
            {
                case Session.NetworkMode.Standalone:
                    return new Client.Network.DummyClient();
                case Session.NetworkMode.Client:
                    return new Client.Network.Client();
                case Session.NetworkMode.Host:
                    return new Server.Network.ClientServer();
                default:
                    return null;
            }
        }

        #endregion
        
        public void FixedUpdate()
        {
            if (this._network != null)
            {
                this._network.Update();
            }
        }

        void SubscribeEvents()
        {
        }

        void UnsubscribeEvents()
        {
        }

        void LoadWorldNonClient()
        {
            // TODO: Get the game to stop on the laoding scene until both world and world nonclient are fully loaded
            if (this.Session.IsOwner)
            {
                SceneLoader.Instance.Enqueue(SceneData.SceneKey.WorldNonClient, LoadSceneMode.Additive);
                SceneLoader.Instance.ActivateNext();
            }
        }

        void OnEvtSceneUnloaded(Scene scene)
        {
            if (this.Session.IsOwner && scene.name == SceneLoader.Instance.SceneData.GameName)
            {
                SceneManager.UnloadSceneAsync(SceneLoader.Instance.SceneData.WorldNonClient);
            }
        }

    }


}
