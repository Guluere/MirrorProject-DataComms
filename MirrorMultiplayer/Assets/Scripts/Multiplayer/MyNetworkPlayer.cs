using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text DisplayNameText = null;

    [SerializeField]
    private Renderer DisplayColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayNameUpdate))]
    [SerializeField]
    private string DisplayName = "Missing Name";


    [SyncVar(hook = nameof(HandleDisplayColorUpdate))]
    [SerializeField]
    private Color DisplayColor = Color.black;

    #region Server
    [Server]
    public void SetDisplayName(string NewDisplayName)
    {
        DisplayName = NewDisplayName;
    }

    [Server]
    public void SetDisplayColor(Color NewDisplayColor)
    {
        DisplayColor = NewDisplayColor;
    }    

    [Command]
    public void CmdSetDisplayName(string NewDisplayName)
    {
        //Server Authority to limit display name into 2 - 20 letter length
        if( NewDisplayName.Length < 2 || NewDisplayName.Length > 20)
        {
            return;
        }
        RpcDisplayNewName(NewDisplayName);
        SetDisplayName(NewDisplayName);
    }

    private void HandleDisplayNameUpdate(string OldName, string NewName)
    {
        DisplayNameText.text = NewName;
    }
    private void HandleDisplayColorUpdate(Color OldColor, Color NewColor)
    {
        DisplayNameText.color = NewColor;
    }

    [ContextMenu ("Set This Name")]
    private void SetThisName()
    {
        CmdSetDisplayName("My New Name");
    }    

    [ClientRpc]
    private void RpcDisplayNewName(string NewDisplayName)
    {
        Debug.Log(NewDisplayName);
    }

    #endregion Server
}