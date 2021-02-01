using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyncTest : NetworkBehaviour
{
    //-------------------(hook) Method-------------
    [SyncVar(hook = nameof(UpdateChangedColor))]
    [SerializeField] Color sapphireColor;

    [Command]
    private void CmdChangeColor()
    {   
        sapphireColor = Random.ColorHSV();    
        Debug.Log("Renk deðiþti");
    }

    //---Client
    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            CmdChangeColor();
        }
    }

    private void UpdateChangedColor(Color oldColor, Color newColor)
    {
        GameObject Sapphire = GameObject.Find("SyncTest");
        Sapphire.GetComponent<MeshRenderer>().material.color = newColor;
        Debug.Log("Material deðiþti");
    }

    //-------------------[ClientRpc] Method-------------

    /*
    float xMove = Input.GetAxisRaw("Horizontal");
    float zMove = Input.GetAxisRaw("Vertical");

    [Client]
    -----------------------------

    private void Update()
    {
        CmdMove();
    }



    [Command]
    //Requesting to the Server I wan't to move.
    private void CmdMove()
    {   
        //For Validation, For Avoid To Hack
        if(!hasAuthority){return;}
        RPCMove();
    }

    [Server]
    -----------------------------
    [ClientRPC]
    //Server Execute Client Request And Everyone Receive Which Is Same As Sycn
    private void RPCMove()
    {   
        transform.position += new Vector3(xMove, 0, zMove).normalized* speed * Time.deltaTime;
    }
    
    */
}
