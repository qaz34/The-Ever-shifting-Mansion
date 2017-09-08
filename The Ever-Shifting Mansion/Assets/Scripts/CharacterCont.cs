using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
[RequireComponent(typeof(CharacterController))]
public class CharacterCont : MonoBehaviour
{
    CharacterController cController;
    float moveSpeed;
    public float walkSpeed = 5;
    public float sprintSpeed = 10;
    float yMovement;

    public Camera currentCamera;
    public Camera prevCamera;
    bool cameraChanged;
    Vector3 heading;
    // Use this for initialization
    void Start()
    {
        cController = GetComponent<CharacterController>();
    }
    public Camera PreviousCamera
    {
        get
        {
            return prevCamera;
        }
        set
        {
            cameraChanged = true;
            InputDevice device = InputManager.ActiveDevice;
            heading = new Vector3(device.LeftStickX, 0, device.LeftStickY);
            prevCamera = value;
        }
    }

    void Update()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device == null)
            return;
        Sprint(device);
        Movement(device);
    }
    void Sprint(InputDevice device)
    {
        if (device.Action3)
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }
    void Movement(InputDevice device)
    {

        var movement = new Vector3(device.LeftStickX, 0, device.LeftStickY);

        if (cameraChanged)
        {
            float sAngle = Vector3.SignedAngle(movement, heading, Vector3.up);

            if (Vector3.Angle(movement, heading) > 50)
            {
                cameraChanged = false;
            }
            else if (movement.magnitude == 0)
                cameraChanged = false;
        }
        Transform tempTransfrom;
        if (cameraChanged)
            tempTransfrom = PreviousCamera.transform;
        else
            tempTransfrom = currentCamera.transform;

        Vector3 camForward = Vector3.Scale(tempTransfrom.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = new Vector3(camForward.z, 0, -camForward.x);       
        movement = movement.x * camRight + movement.z * camForward;
        movement.y = 0;
        movement *= moveSpeed;
        movement.y = yMovement += Physics.gravity.y * Time.deltaTime;
        if (cController.isGrounded)
            yMovement = Physics.gravity.y * Time.deltaTime;
        cController.Move(movement * Time.deltaTime);
    }
}
