using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
[RequireComponent(typeof(CharacterController))]
public class CharacterCont : MonoBehaviour
{
    CharacterController cController;
    float moveSpeed;
    public float currentSpeed;
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
    Animator animator;

    public List<CameraTrigger> amIn = new List<CameraTrigger>();
    public Camera movingFrom;
    public Camera currentlyInCamera;

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
        animator = GetComponent<Animator>();
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
            return;

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
        float turnAmount = 0;
        var movement = new Vector3(device.LeftStickX, 0, device.LeftStickY);
        // transform.forward = Vector3.Lerp(transform.forward, movement.normalized, .2f);
        if (!aiming)
        {

            if (cameraChanged)
            {
                if (movement.magnitude <= .05f)
                {
                    cameraChanged = false;
                    prevCamera = null;
                }
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

            Vector3 first = transform.forward;
            if ((transform.forward + movement.normalized).magnitude < .01f)
                transform.forward = Vector3.Lerp(transform.forward, movement.normalized + transform.right, turnSpeed);
            else
                transform.forward = Vector3.Lerp(transform.forward, movement.normalized, turnSpeed);

            turnAmount = Vector3.SignedAngle(first, transform.forward, Vector3.up) / 10;

            movement *= moveSpeed;
            if (Vector3.Angle(transform.forward, movement) < moveAngle)
            {
                movement *= (1 - Vector3.Angle(transform.forward, movement) / moveAngle);
                cController.Move(movement * Time.deltaTime);
            }
            else
                movement = movement / 5;
            currentSpeed = movement.magnitude;
        }
        yMovement += Physics.gravity.y * Time.deltaTime;
        if (cController.isGrounded)
            yMovement = Physics.gravity.y * Time.deltaTime;
        cController.Move(new Vector3(0, yMovement) * Time.deltaTime);
        if (animator)
        {
            animator.SetFloat("Turn", turnAmount);
            animator.SetFloat("Forward", (movement.magnitude / 5));
        }
    }
}
