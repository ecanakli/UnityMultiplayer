using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        //Every Frame If You Are On Server Try And Produce Units
        if (isServer)
        {
            ProduceUnits();
        }
        //If You Are On Client, Timer Value Updating On Client Side, Every Frame
        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

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

    [Server]
    private void ProduceUnits()
    {
        if(queuedUnits == 0) { return; }

        //Increase Timer
        unitTimer += Time.deltaTime;

        if(unitTimer < unitSpawnDuration) { return; }

        //Spawnin Prefab on the Server
        GameObject unitInstantiate = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, Quaternion.identity);

        //Spawning prefab on the Network So Clients Can Get This Instantiation
        NetworkServer.Spawn(unitInstantiate, connectionToClient); //ConnectionToClient = Spawning object Belongs to the client itself ("This prefab Belongs to me").
        //"NetworkServer.Spawn(unitInstantiate);" = //If there is no comma as it is, Instantiated Object belongs to the Server

        //Moving Units Nearby, To Not Stacking Top Of Eachother
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;

        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstantiate.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        //Reset Timer
        queuedUnits--;
        unitTimer = 0f;
    }

    //When This Object is Alive, this Funcition Will Be Called Whenever We Die On The Server.
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        //If Queue is Full Then Not Spawn Unit
        if(queuedUnits == maxUnitQueue) { return; }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        //If Player Resource Smaller Then Prefab Cost Then Not Spawn Unit
        if(player.GetMyResources() < unitPrefab.GetResourceCost()) { return; }

        queuedUnits++;
        //Set Player Resources To CurrentResources - UnitCost
        player.SetResources(player.GetMyResources() - unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        //How Much Fill Up The Image
        float newProgress = unitTimer / unitSpawnDuration;

        //When It Fills Up Full, Snap It Back To The Start Point
        if(newProgress < unitProgressImage.fillAmount)
        {
            //Set It To The Start
            unitProgressImage.fillAmount = newProgress;
        }
        else //Otherwise Fill Amount
        {
            //Smoothly Fill Image
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
        }
    }

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


    //Whenever Queued Units Change Update Text
    private void ClientHandleQueuedUnitsUpdated(int oldQueuedUnits, int newQueuedUnits)
    {
        remainingUnitsText.text = newQueuedUnits.ToString();
    }

    #endregion
}
