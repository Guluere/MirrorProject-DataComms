using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Experimental;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] InputActionReference InputRefClick;
    [SerializeField] InputActionReference InputRefMousePos;

    private Camera MainCamera;

    [SerializeField]
    UnityEvent OnClick;
    [SerializeField]
    UnityEvent OnMove;

    #region Server

    [Command]
    private void CmdMove(Vector3 Pos)
    {
        transform.position = Pos;
        MoveInvoke();
    }
    [ClientRpc]
    private void MoveInvoke()
    {
        OnMove.Invoke();
    }

    [Command]
    private void CmdShoot()
    {
        ClickInvoke();
    }

    [ClientRpc]
    private void ClickInvoke()
    {
        OnClick.Invoke();
    }
    #endregion

    #region Client
    //Start Method for Client
    public override void OnStartAuthority()
    {
        MainCamera = Camera.main; //Camera Ref
    }

    [ClientCallback]
    private void Update()
    {
        if (!isOwned) return; //If this server client have no owner

        if (InputRefClick.action.WasPressedThisFrame()) CmdShoot();

        //Debug.LogError("Clicked" + InputRefClick.action.WasPerformedThisFrame());

        Vector2 MouseScreenPos = InputRefMousePos.action.ReadValue<Vector2>();
        Vector2 Point = MainCamera.ScreenToWorldPoint(MouseScreenPos);
        //Ray ray = MainCamera.ScreenPointToRay(MouseScreenPos);

        //Debug.LogError(MouseScreenPos);

        //if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

        //.LogError(hit.point);

        CmdMove(Point);

    }
    #endregion Client
}
