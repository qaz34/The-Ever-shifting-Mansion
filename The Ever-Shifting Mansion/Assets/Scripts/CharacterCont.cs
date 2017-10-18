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
    [Tooltip("Angle before can move")]
    public float moveAngle = 45;
    public float turnSpeed = .2f;
    float yMovement;
    [HideInInspector]
    public bool aiming;
    [HideInInspector]
    public Camera currentCamera;
    Camera prevCamera;
    bool cameraChanged;
    Vector3 heading;
    public void ToggleEnabled()
    {
        enabled = !enabled;
        GetComponent<CombatController>().enabled = enabled;
    }
    public void SetEnabled(bool _enabled)
    {
        enabled = _enabled;
        GetComponent<CombatController>().enabled = _enabled;
    }
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
        if (!currentCamera)
        {
            Debug.Log("No Camera Set");
            return;
        }
        InputDevice device = InputManager.ActiveDevice;
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
        // transform.forward = Vector3.Lerp(transform.forward, movement.normalized, .2f);
        if (!aiming)
        {
            if (cameraChanged)
            {
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
            if ((transform.forward + movement.normalized).magnitude < .01f)
                transform.forward = Vector3.Lerp(transform.forward, movement.normalized + transform.right, turnSpeed);
            else
                transform.forward = Vector3.Lerp(transform.forward, movement.normalized, turnSpeed);
            movement *= moveSpeed;
            if (Vector3.Angle(transform.forward, movement) < moveAngle)
                cController.Move(movement * (1 - Vector3.Angle(transform.forward, movement) / moveAngle) * Time.deltaTime);
        }
        yMovement += Physics.gravity.y * Time.deltaTime;
        if (cController.isGrounded)
            yMovement = Physics.gravity.y * Time.deltaTime;
        cController.Move(new Vector3(0, yMovement) * Time.deltaTime);


    }
}
