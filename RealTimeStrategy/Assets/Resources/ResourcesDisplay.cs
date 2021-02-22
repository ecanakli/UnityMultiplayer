using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;

    private void Start()
    {
        //Get our connection, get the player object for our connection and on the player object get the RTSPlayer script
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        //Set Resorces To The UI
        ClientHandleResourcesUpdated(player.GetMyResources());
        //Subscribe Event
        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    private void OnDestroy()
    {
        //Unsubscribe Event
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    //When Resources Changes Update Resources Text
    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
