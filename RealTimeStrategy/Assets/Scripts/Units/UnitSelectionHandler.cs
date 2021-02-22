using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    //For ReScale DragSelection Area
    [SerializeField] private RectTransform unitSelectionArea = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;

    //Reference
    private RTSPlayer player;
    private Camera mainCamera;

    //Selected Unit List , Protecting Setting with get;
    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;

        //Get our connection, get the player object for our connection and on the player object get the RTSPlayer script
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        //Subscribe
        Unit.AuthorityOnUnitDeSpawned += AuthorityHandleUnitDeSpawned;
        //For Disable This Script When We Die
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        //Unsubscribe
        Unit.AuthorityOnUnitDeSpawned -= AuthorityHandleUnitDeSpawned;

        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        //To Selecting Units
        MouseDragging();
    }

    private void MouseDragging()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }

        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }

        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        //To Clear Selection Area
        ClearAreaAtBegin();

        StartDragging();
    }

    private void StartDragging()
    {
        //Set Active Selecting Area Image
        unitSelectionArea.gameObject.SetActive(true);

        //Get Start Position to Start Drag Process
        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    //To Clear Selection Area While Clicking Void At The First Click
    private void ClearAreaAtBegin()
    {
        //Clear Selection If is Not Pressing Left Shift
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            //Clear Selection If is Not Clicking Units
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.UnSelected();
            }

            SelectedUnits.Clear();
        }
    }

    //For Updateing Image Scale While Dragging
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        //Changing Size of The UI Area Box
        //We Are Getting Vector 2 Values Which is ignoring minus(if x value is -10 return 10)
        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        //Calculating Center AchorPoint
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    //----We hit Something, Is it Unit, Is this Unit Belongs to Us.
    private void ClearSelectionArea()
    {
        //When We Realese Mouse Button Deactivate SelectionArea
        unitSelectionArea.gameObject.SetActive(false);

        //Clicking But Not Dragging(Selecting One Unit)
        if(unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            //If We Do not hit Then Return
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            //If We Do not Hit Unit Then Return
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            //Is The Unit Our Client Unit
            if (!unit.hasAuthority) { return; }

            //Adding Selected Unit To The List.
            SelectedUnits.Add(unit);

            //Turn On Green Highlight
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Selected();
            }

            return;
        }

        //Multiselecting Units
        foreach (Unit unit in player.GetMyUnits())
        {
            //If the list contains Already Selected Units Then Continue Next One
            if (SelectedUnits.Contains(unit)) { continue; }

            //Raycasting is ScreenSpace But Tanks Are WorldSpace, So To Select Tanks to get this positions
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            //Calculating Vector Points, Is the Corner Points Greater Than unit X and Y value (It Means units are inside of the SelectionArea)

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

            if (screenPosition.x > min.x &&
                screenPosition.x < max.x && 
                screenPosition.y > min.y && 
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Selected();
            }
        }
    }

    //When Selected Unit Dies Remove Them From The List
    private void AuthorityHandleUnitDeSpawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    //When The Game is Over Disable This Script
    private void ClientHandleGameOver(string winnerName)
    {
        //When The Game is Over It Will Stop Runnin Update Method
        enabled = false;
    }
}
