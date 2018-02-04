using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "data/Session")]
public class Session : ScriptableObject
{

    // Various network modes a game can be in
    public enum NetworkMode
    {
        // Game has not started
        None,
        // No networking
        Standalone,
        // Joiner of game: Client
        Client,
    }

    // The IPv4 address of this peer (client or server)
    public string Address;

    // The IPv4 address of this peer (client or server)
    public string TargetAddress;

    // Identifier which stores what type of game is being run (network wise)
    public NetworkMode Mode;

    // The port of the address
    public int Port;

    // The maximum amount of clients allowed to connect to the server
    // Only used when Mode == Host
    public int MaxClients = 10;

    public uint ClientID = 0;

    public bool Connected = false;

    // Can this session be considered valid _gameStateData? True only if the game has started (user has entered world scene)
    public bool IsValid
    {
        get { return this.Mode != NetworkMode.None; }
    }

    // If the game session is networked or not (if it is, it is also a client)
    public bool IsNetworked
    {
        get { return this.Mode != NetworkMode.Standalone; }
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
