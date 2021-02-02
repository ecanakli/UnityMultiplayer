using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask groundMask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;

    private BoxCollider buildingCollider;

    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Start()
    {
        mainCamera = Camera.main;

        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
        buildingCollider = building.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (player == null)
        {
            //Get our connection, get the player object for our connection and on the player object get the RTSPlayer script
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if(buildingPreviewInstance == null) { return; }

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        //Cheking If We Have Enough Resources To Build
        if(player.GetMyResources() < building.GetPrice()) { return; }

        //If We Click Mouse Left Button On This Object Instantiate Preview And Get Renderer
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        //To Avoid Instantiate Object in (0,0,0) Position We Set It False At The Beginning
        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //If We Arent Dragging, We Release Mouse Button
        if(buildingPreviewInstance == null) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Place Building At Hit Position
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //If Ray Does't Hit Something Then Return
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask)) { return; }
        
        //Dragging Building Preview Object
        buildingPreviewInstance.transform.position = hit.point;
        //If Building Preview Is Not Active
        if (!buildingPreviewInstance.activeSelf)
        {
            //Activate Preview While Dragging
            buildingPreviewInstance.SetActive(true);
        }

        //Setting Building Preview Color Looking CanPlace Situation

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

        buildingRendererInstance.material.SetColor("_BaseColor", color);
    }
}
