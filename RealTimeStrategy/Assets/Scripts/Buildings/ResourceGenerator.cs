using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    //Every 2 Seconds We Got 10
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    //For Count Time
    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        //To Reducing Interval We Set It To The Timer
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        //Subscribing Events
        //When It Dies
        health.ServerOnDie += ServerHandleDie;
        //When Game is Over
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        //Unsubscribe
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        //Reduce Timer
        timer -= Time.deltaTime;

        if(timer <= 0) //We Set it interval Value If It Reaches 0 Then
        {
            //Increase Resources
            player.SetResources(player.GetMyResources() + resourcesPerInterval);

            //Reset Timer To Beginning interval value
            timer += interval;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        //To Stop Generate Resources We Disabled This Scriot
        enabled = false;
    }
}
