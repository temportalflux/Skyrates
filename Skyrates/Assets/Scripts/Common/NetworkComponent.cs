using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkComponent : Singleton<NetworkComponent>
{

    public static NetworkComponent Instance;

    /// <summary>
    /// The session which is instantiated prior to running
    /// <see cref="NetworkComponent.CreateNetworkAndConnect"/>.
    /// </summary>
    public Session session;

    public GameState gameState;

    /// <summary>
    /// The networking object, clients and client-on-top-of-servers are subclasses
    /// </summary>
    private Client _network;

    private void Awake()
    {
        this.loadSingleton(this, ref Instance);

        this._network = null;

        this.InitData();
        
        this.session.SetAddressBoth(this.GetIP());
        StartCoroutine(this.CheckIP());
    }

    void OnDestroy()
    {

        if (this._network != null)
        {
            if (Session.Connected || Session.HasValidClientID())
            {
                GetClient().Dispatch(new EventDisconnect(NetworkComponent.Session.ClientID));
            }

            GetClient().Disconnect();
            GetClient().Destroy();
        }

        this.InitData();
    }

    // Wipes Session and GameState
    private void InitData()
    {

        this.session.Mode = Session.NetworkMode.None;
        this.session.SetAddressBoth("");
        this.session.Port = 0;
        this.session.SetClientID(uint.MaxValue); // mark as something horrible instead of -1
        this.session.Connected = false;

        this.gameState.SetClients(new GameStateData.Client[0]);

    }

    #region Static

    public static Session Session
    {
        get
        {
            Debug.Assert(Instance != null);
            return Instance.session;
        }
    }

    public static GameState GameState
    {
        get
        {
            Debug.Assert(Instance != null);
            return Instance.gameState;
        }
    }

    public static Client GetClient()
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
        this.session.SetAddress(myExtIP);
    }
    
    #endregion

    #region Actions

    public void StartStandalone()
    {
        this.StartGame(Session.NetworkMode.Standalone);
        // Start the world asap
        SceneLoader.Instance.ActivateNext();
    }

    public void StartClient()
    {
        this.StartGame(Session.NetworkMode.Client);
        // will wait to start world until net handshake
    }

    private void StartGame(Session.NetworkMode mode)
    {
        SceneLoader.Instance.ActivateNext();
        SceneLoader.Instance.Enqueue(SceneData.SceneKey.World);

        Debug.Log("NetComp: Starting network as " + mode);

        this.session.Mode = mode;
        this.CreateNetworkAndConnect();
    }

    #endregion

    #region Start Network

    public void CreateNetworkAndConnect()
    {
        Debug.Assert(this.session.IsValid);

        // Create the network
        this._network = this.CreateNetwork();
        this._network.Create();
        // Create it up
        this._network.Start(this.session);
        // Connect
        this._network.Connect(this.session);
    }

    private Client CreateNetwork()
    {
        switch (this.session.Mode)
        {
            case Session.NetworkMode.Standalone:
                return new DummyClient();
            case Session.NetworkMode.Client:
                return new Client();
            //case Session.NetworkMode.Host:
                //return new ServerClient(new Client());
            default:
                return null;
        }
    }

    #endregion

    #region SceneEvents

    public void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this._network != null)
        {
            this._network.OnSceneLoaded(scene, mode);
        }
    }

    public void FixedUpdate()
    {
        if (this._network != null)
        {
            this._network.UpdateNetwork();
        }
    }

    #endregion

}
