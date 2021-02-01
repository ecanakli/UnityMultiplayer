using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    //When We Die Our Id Matched this Id, And We Can Manage Unit Whatever We Want After Died checking By Id
    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDeSpawned;


    #region Server

    //When This Object Alive We Called ServerHandleDie Method, If Health = 0 Then Raise This Event
    public override void OnStartServer()
    {
        //Subscribe This Event
        //Health is 0 Then Destro Base
        health.ServerOnDie += ServerHandleDie;
        //Adding Base To The List
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        //UnSubscribe This Event When Base Destroyed
        health.ServerOnDie -= ServerHandleDie;
        //Removing Base From The List
        ServerOnBaseDeSpawned?.Invoke(this);
    }

    //When This Object is Alive, this Funcition Will Be Called Whenever We Die On The Server.
    [Server]
    private void ServerHandleDie()
    {
        //Raises This Event With Our Player Id
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion


    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    #endregion
}
