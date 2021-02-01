using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    //Game Over Panel
    [SerializeField] private GameObject gameOverDisplayParent = null;
    //Game Over Text
    [SerializeField] private TMP_Text winnerNameText = null;

    //Subscribing An Event
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    //Leave Game Button In Game Over Panel
    public void LeaveGame()
    {
        //If Pressed Leave Game Stop Game
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            //Stop Game As A Host
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //Stop Game As A Client
            NetworkManager.singleton.StopClient();
        }
    }

    //Raises Game Over Panel
    private void ClientHandleGameOver(string winner)
    {
        gameOverDisplayParent.SetActive(true);

        winnerNameText.text = $"{winner} Has Won!";
    }
}
