using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    //Visual of Building
    [SerializeField] private GameObject buildingPreview = null;
    //Building Create Icon
    [SerializeField] private Sprite icon = null;
    //Building Id To Send Server
    [SerializeField] int id = -1;
    //Building Price
    [SerializeField] private int price = 100;

    //It Is Called When Buildings Spawned
    public static event Action<Building> ServerOnBuildingSpawned;
    //It Is Called When Buildings Despawned
    public static event Action<Building> ServerOnBuildingDeSpawned;

    //For Storing Buildings As A Client
    //It Is Called When Buildings Spawned
    public static event Action<Building> AuthorityOnBuildingSpawned;
    //It Is Called When Buildings Despawned
    public static event Action<Building> AuthorityOnBuildingDeSpawned;

    //To Reach Preview of Building
    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    //To Reach Icon
    public Sprite GetIcon()
    {
        return icon;
    }

    //To Reach Id
    public int GetId()
    {
        return id;
    }

    //To Reach Price
    public int GetPrice()
    {
        return price;
    }

    #region Server
    //Raise Event Whenever A Building Start Existing
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }
    //Raise Event Whenever A Building Stop Existing
    public override void OnStopServer()
    {
        ServerOnBuildingDeSpawned?.Invoke(this);
    }

    #endregion

    #region Client

    //Raising Event Server Side For Client To See All Changes
    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        AuthorityOnBuildingDeSpawned?.Invoke(this);
    }

    #endregion
}
