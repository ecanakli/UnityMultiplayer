using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    //To Reach Target Variable
    public Targetable GetTarget()
    {
        return target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        //Validating Targetable Object
        //If The Gameobject Sent To Us Does Not Have Targetable Script then Return If It Does Take This Target.
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        //Setting Target Gameobject
        target = newTarget;
    }

    [Server]
    //To Clear Target While Not Targetting
    public void ClearTarget()
    {
        target = null;
    }

    [Server]
    //When The Game Is Over Clear The Target
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }
}
