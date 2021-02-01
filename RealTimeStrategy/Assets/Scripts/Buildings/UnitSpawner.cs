using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server
    //When This Object Alive We Called ServerHandleDie Method, If Health = 0 Then Raise This Event
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    //When This Object is Alive, this Funcition Will Be Called Whenever We Die On The Server.
    [Server]
    private void ServerHandleDie()
    {
        //NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        //Spawnin Prefab on the Server
        GameObject unitInstantiate = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);

        //Spawning prefab on the Network So Clients Can Get This Instantiation
        NetworkServer.Spawn(unitInstantiate, connectionToClient); //ConnectionToClient = Spawning object Belongs to the client itself ("This prefab Belongs to me").
        //"NetworkServer.Spawn(unitInstantiate);" = //If there is no comma as it is, Instantiated Object belongs to the Server
    }

    #endregion

    #region Client

    //Inherit IPointerClickHandler Interface and Whenever We Click On This Gameobject, Calls The Following Code.
    public void OnPointerClick(PointerEventData eventData)
    {
        //If We Clicked Left Mouse Button then Call Cmd
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        //To Avoid Clicking Someoneelse SpawnPoint, It is still Not Execute Because of ConnectionToClient Method
        //But This Function Allow Us Not To Bother The Server.
        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }

    #endregion
}
