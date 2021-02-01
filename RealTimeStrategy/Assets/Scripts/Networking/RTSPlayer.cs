using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    //To Reach Unit List From Another Script
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    //To Reach Building List From Another Script
    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    #region Server

    //Subscribing Events Whenever We Invoke This Server On Spawned
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDeSpawned += ServerHandleUnitDeSpawned;

        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDeSpawned += ServerHandleBuildingDeSpawned;
    }

    //Unsubscribing Events Whenever We Stop Server
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDeSpawned -= ServerHandleUnitDeSpawned;

        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDeSpawned -= ServerHandleBuildingDeSpawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        //Is The Client That Owns This Unit The Same As This Player
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        //If It Is Add Units to The List
        myUnits.Add(unit);
    }

    private void ServerHandleUnitDeSpawned(Unit unit)
    {
        //Is The Client That Owns This Unit The Same As This Player
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        //If It Is Remove Units From The List
        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        //Is The Client That Owns This Building The Same As This Player
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        //If It Is Add Building to The List
        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDeSpawned(Building building)
    {
        //Is The Client That Owns This Building The Same As This Player
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        //If It Is Remove Building From The List
        myBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        //If We Have Authority, If This is Us, Then We Will Store This Units
        //If This Machine Running As A Server
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDeSpawned += AuthorityHandleUnitDeSpawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingDeSpawned;
    }

    public override void OnStopClient()
    {
        //If We The Server Or Not Has Authority
        //Do not Subscribe If We Do not Have Authority
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDeSpawned -= AuthorityHandleUnitDeSpawned;

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingDeSpawned;
    }

    //Unit Part
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDeSpawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    //Building Part
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDeSpawned(Building building)
    {
        myBuildings.Remove(building);
    }

    #endregion
}
