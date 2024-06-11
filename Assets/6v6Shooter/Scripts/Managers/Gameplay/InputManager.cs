using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public enum InputType
    {
        MouseAndKeyboard,
        Gamepad,
        PlayStation,
        Xbox
    }

    public InputType currentInputType;
    private InputType previousInputType;

    private void Start()
    {
        previousInputType = currentInputType;
    }

    private void Update()
    {
        CheckInputDevice();
    }

    private void CheckInputDevice()
    {
        bool inputDetected = false;

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            if (Gamepad.current.name.Contains("DualShock") || Gamepad.current.name.Contains("DualSense"))
            {
                currentInputType = InputType.PlayStation;
            }
            else if (Gamepad.current.name.Contains("Xbox"))
            {
                currentInputType = InputType.Xbox;
            }
            else
            {
                currentInputType = InputType.Gamepad;
            }
            inputDetected = true;
        }
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
        {
            currentInputType = InputType.MouseAndKeyboard;
            inputDetected = true;
        }
        else if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
        {
            currentInputType = InputType.MouseAndKeyboard;
            inputDetected = true;
        }

        if (inputDetected && currentInputType != previousInputType)
        {
            LogCurrentInputType();
            previousInputType = currentInputType;
        }
    }

    private void LogCurrentInputType()
    {
        Debug.Log("Input type changed to: " + currentInputType);
    }

    public InputType GetCurrentInputType()
    {
        return currentInputType;
    }
}
