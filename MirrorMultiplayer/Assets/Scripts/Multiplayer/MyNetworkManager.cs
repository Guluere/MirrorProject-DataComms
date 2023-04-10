using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("I Connected to a Server");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer Player = conn.identity.GetComponent<MyNetworkPlayer>();

        Player.SetDisplayName($"Player {numPlayers}");

        Color DisplayColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        Player.SetDisplayColor(DisplayColor);
    }
}
