using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //When We Joined Server Instantiate SpawnerButton Belongs To Us.
        GameObject unitSpawnerInstance =  Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        //Spawn On The Network and Conn means as The Same connectionToClient = Give this SpawnerPoint To The Belongs Client
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    //Spawn Game Over Handler Only If We Are In Level Not Menu Scene
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
