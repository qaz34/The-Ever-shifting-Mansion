using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace CustomProfileExample
{
    // This custom profile is enabled by adding it to the Custom Profiles list
    // on the InControlManager component, or you can attach it yourself like so:
    // InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
    // 
    public class KeyboardAndMouseProfile : UnityInputDeviceProfile
    {
        public KeyboardAndMouseProfile()
        {
            Name = "Keyboard/Mouse";
            Meta = "A keyboard and mouse combination profile appropriate for FPS.";

            // This profile only works on desktops.
            SupportedPlatforms = new[]
            {
                "Windows",
                "Mac",
                "Linux"
            };

            Sensitivity = 1.0f;
            LowerDeadZone = 0.0f;
            UpperDeadZone = 1.0f;

            ButtonMappings = new[]
            {
                new InputControlMapping
                {
                    Handle = "Fire - Mouse",
                    Target = InputControlType.RightTrigger,
                    Source = MouseButton0
                },
                new InputControlMapping
                {
                    Handle = "Aim - Mouse",
                    Target = InputControlType.LeftTrigger,
                    Source = MouseButton1
                },
                new InputControlMapping
                {
                    Handle = "Interact",
                    Target = InputControlType.Action1,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.F, KeyCode.Return )
                },
                new InputControlMapping
                {
                    Handle = "Sprint",
                    Target = InputControlType.Action3,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.LeftShift )
                },
                new InputControlMapping
                {
                    Handle = "Reload",
                    Target = InputControlType.Action3,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.R )
                },
                new InputControlMapping
                {
                    Handle = "Menu",
                    Target = InputControlType.Menu,
                    Source = KeyCodeButton( KeyCode.Escape)
                },
                new InputControlMapping
                {
                    Handle = "Inventory",
                    Target = InputControlType.Action4,
                    Source = KeyCodeButton( KeyCode.Tab )
                },
                new InputControlMapping
                {
                    Handle = "LeftBumper",
                    Target = InputControlType.DPadLeft,
					// KeyCodeComboButton requires that all KeyCode params are down simultaneously.
					Source = KeyCodeButton( KeyCode.Q )
                },
                new InputControlMapping
                {
                    Handle = "RightBumper",
                    Target = InputControlType.DPadRight,
					// KeyCodeComboButton requires that all KeyCode params are down simultaneously.
					Source = KeyCodeButton( KeyCode.E )
                },
            };

            AnalogMappings = new[]
            {
                new InputControlMapping
                {
                    Handle = "Move X",
                    Target = InputControlType.LeftStickX,
					// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
                },
                new InputControlMapping
                {
                    Handle = "Move Y",
                    Target = InputControlType.LeftStickY,
					// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
                },
                new InputControlMapping {
                    Handle = "Move X Alternate",
                    Target = InputControlType.LeftStickX,
                    Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
                },
                new InputControlMapping {
                    Handle = "Move Y Alternate",
                    Target = InputControlType.LeftStickY,
                    Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
                },
                new InputControlMapping
                {
                    Handle = "Look X",
                    Target = InputControlType.RightStickX,
                    Source = MouseXAxis,
                    Raw    = true,
                    Scale  = 0.1f
                },
                new InputControlMapping
                {
                    Handle = "Look Y",
                    Target = InputControlType.RightStickY,
                    Source = MouseYAxis,
                    Raw    = true,
                    Scale  = 0.1f
                },
                new InputControlMapping
                {
                    Handle = "Look Z",
                    Target = InputControlType.ScrollWheel,
                    Source = MouseScrollWheel,
                    Raw    = true,
                    Scale  = 0.1f
                }
            };
        }
    }
}

