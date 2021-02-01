using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        //For Disable This Script When We Die
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        //Unsubscribe
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        RaycastMove();
    }

    //When We RightClick Do A Raycast, Try To Move Only Selected Units
    private void RaycastMove()
    {
        //If We Are Not Clicking Right Button Then Return
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //If We Do not hit Then Return
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        //If We Hitting Targetable Object
        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            //If Targetted Object Belong To Us
            if (target.hasAuthority)
            {
                //Try To Move
                TryMove(hit.point);
                return;
            }
            //Else Do Not Move, Target!
            TryTarget(target);
            return;
        }
        //If We Do Not Hit Targetable Object Try To Move
        TryMove(hit.point);
    }

    //Go Over Selected Units And Do Move
    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }

    //Get Unit Targeter Script Set Into Targetable Object
    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    //When The Game is Over Disable This Script
    private void ClientHandleGameOver(string winnerName)
    {
        //When The Game is Over It Will Stop Doing Raycast
        enabled = false;
    }
}
