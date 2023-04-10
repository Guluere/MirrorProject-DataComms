using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class NetworkObjectCreateAndDestroyer : NetworkBehaviour
{
    public void CheckDestroySelf() //If the object is on client, destroy by client, else by server, else by self
    {
        if (isServer) TellServerToDestroySelf();
        else if (isClient) TellClientToDestroySelfAndAll();
        else Destroy(gameObject);
    }
    [Server]
    public void TellServerToDestroyObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    [Server]
    public void TellServerToDestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void TellClientToDestroySelfAndAll()
    {
        TellServerToDestroySelf();
    }
}
