using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummyClient : Client
{

    public override void Create() { }

    public override void Destroy() { }

    public override void Connect(Session session) { }

    public override void Disconnect() { }

    public override void Start(Session session) { }

    public override void Dispatch(NetworkEvent evt, string address, int port) { }

    public override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    public override void JoinServer() { }

    public override void UpdateNetwork() { }

}
