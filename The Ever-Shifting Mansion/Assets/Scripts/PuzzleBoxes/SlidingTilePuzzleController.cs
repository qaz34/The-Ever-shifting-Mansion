using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBox
{
    [RequireComponent(typeof(SlidingTilePuzzle))]
    public class SlidingTilePuzzleController : MonoBehaviour
    {
        SlidingTilePuzzle puzzle;

        void Awake()
        {
            puzzle = GetComponent<SlidingTilePuzzle>();
        }

        void Update()
        {
            InputDevice device = InputManager.ActiveDevice;
            //Vector2 stickInput = device.LeftStick.HasChanged ? new Vector2(0, 0) : new Vector2(Mathf.RoundToInt(device.LeftStickX), Mathf.RoundToInt(device.LeftStickY));

            if (Input.GetKeyDown(KeyCode.W))//device.DPadUp.WasPressed || stickInput.y > 0)
            {
                puzzle.SlideTileIntoEmpty(SlidingTilePuzzle.Direction.DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.D))//device.DPadRight.WasPressed || stickInput.x > 0)
            {
                puzzle.SlideTileIntoEmpty(SlidingTilePuzzle.Direction.LEFT);
            }
            else if (Input.GetKeyDown(KeyCode.S))//device.DPadDown.WasPressed || stickInput.y < 0)
            {
                puzzle.SlideTileIntoEmpty(SlidingTilePuzzle.Direction.UP);
            }
            else if (Input.GetKeyDown(KeyCode.A))//device.DPadLeft.WasPressed || stickInput.x < 0)
            {
                puzzle.SlideTileIntoEmpty(SlidingTilePuzzle.Direction.RIGHT);
            }
        }
    }
}
