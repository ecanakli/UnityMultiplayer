using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    //To Change Objects Renderer Color (If We Have More Than One Renderer, To Change All Of Colors We Are Using Array)
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();

    #region Server

    public override void OnStartServer()
    {
        //Find Which Player Owns This Object
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        //Get Color Belongs To Us
        teamColor = player.GetTeamColor();
    }

    #endregion

    #region Client

    //When Objects Spawned Belongs To Us, Change Objects Renderer Color
    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    #endregion
}
