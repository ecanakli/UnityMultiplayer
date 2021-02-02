using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent unSelected = null;

    //It is called when units spawned
    public static event Action<Unit> ServerOnUnitSpawned;
    //It is called when units despawned
    public static event Action<Unit> ServerOnUnitDeSpawned;


    //It is called when units spawned
    public static event Action<Unit> AuthorityOnUnitSpawned;
    //It is called when units despawned
    public static event Action<Unit> AuthorityOnUnitDeSpawned;

    public int GetResourceCost()
    {
        return resourceCost;
    }

    //For Reach Another Script Unit Movement But Refactor this
    //To Refactor this, Create Void Start and Referance To UnitMovement in Reaching Script Which is UnitCommandGiver = Not Define Function Here
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    //To Reach Unit's Target
    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server
    public override void OnStartServer()
    {
        //Adding Units
        ServerOnUnitSpawned?.Invoke(this);
        //Subscribe This Event
        //Health is 0 Then Destroy
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        //Removing Units
        ServerOnUnitDeSpawned?.Invoke(this);
        //UnSubscribe This Event When Unit Destroyed
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    //When Health is 0 Then Destroy This Object
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client
    //Raising Event Server Side For Client To See All Changes
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        AuthorityOnUnitDeSpawned?.Invoke(this);
    }

    [Client]
    public void Selected()//Turn On Green Highlight
    {
        //To Avoid Tp Select Other Client Units.
        if (!hasAuthority) { return; }

        //? = If this is null, to do nothing, do not invoke it.
        onSelected?.Invoke();
    }

    [Client]
    public void UnSelected()//Turn Off Green Highlight
    {
        if (!hasAuthority) { return; }

        unSelected?.Invoke();
    }

    #endregion
}
