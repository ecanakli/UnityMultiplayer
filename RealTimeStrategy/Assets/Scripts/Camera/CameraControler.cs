using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControler : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    //To Calculate How Close Mouse To The Edge
    [SerializeField] private float screenBorderThickness = 10f;
    //To Limit Camera X And Y Movement
    [SerializeField] private Vector2 screenXLimit = Vector2.zero;
    [SerializeField] private Vector2 screenZLimit = Vector2.zero;

    private Vector2 previousInput;
    private Controls controls;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        //If We Had No Keyboard Input Then
        if(previousInput == Vector2.zero)
        {
            //Then Mouse Move
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if(cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else //If We Did Keyboard Input
        {
            //Then Keyboard Move
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimit.x, screenXLimit.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimit.x, screenZLimit.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}
