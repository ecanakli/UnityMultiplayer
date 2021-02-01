using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    //To Server to Know, Game Is Finished
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;
    //Base List
    private List<UnitBase> bases = new List<UnitBase>();

    #region Server
    //Events For Tracking How Many Base Are Left
    public override void OnStartServer()
    {
        //When A Base Spawned Subscribe
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        //When A Base Destroyed Subscribe
        UnitBase.ServerOnBaseDeSpawned += ServerHandleBaseDeSpawned;
    }

    public override void OnStopServer()
    {
        //Stop Calling Events Unsubscribe
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDeSpawned -= ServerHandleBaseDeSpawned;
    }

    [Server]
    //Whenever A Base Spawned Add Base To Bases List
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }
    //Whenever A Base DeSpawned Remove Base From Bases List
    private void ServerHandleBaseDeSpawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if(bases.Count != 1) { return; }

        
        //Get Winner Id
        int playerId = bases[0].connectionToClient.connectionId;
        //Show Who Winned The Game
        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    //Event Raise When Game is Over
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
