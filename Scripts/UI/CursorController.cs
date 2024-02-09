using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
Note 1: CursorLockMode is not being used because it currently performs very poorly.
Note 2: For consistent testing in the editor, click inside the 'Game' window after starting the game to
ensure most Unity's cursor control methods work as intended.
*/
public class CursorController : MonoBehaviour
{
    // Variables.
    [Tooltip("These components found in canvases will be disabled when the cursor is made invisible " +
    "to prevent unintended selections.")]
    [SerializeField] private List<GraphicRaycaster> _graphicRaycasters;

    [Tooltip("Enables cursor usage.")]
    [SerializeField] private bool _canCursorBeUsed = true;

    [Tooltip("If enabled, the cursor's visibility will not be updated when switching between mouse " +
    "and keyboard devices.")]
    [SerializeField] private bool _treatMouseAndKeyboardAsSameDevice;

    private InputDevice _lastInputDevice;


    // Properties.
    private bool CursorState
    {
        get => Cursor.visible;
        set
        {
            Cursor.visible = value;

            foreach (GraphicRaycaster graphicRaycaster in _graphicRaycasters)
                graphicRaycaster.enabled = value
            ;
        }
    }


    // MonoBehaviour.
    private void Awake()
    {
        InputSystem.onActionChange += InputSystem_OnActionChange;
        CursorState = _canCursorBeUsed;
    }


    // Event Handlers.
    private void InputSystem_OnActionChange(object obj, InputActionChange inputActionChange)
    {
        if (!_canCursorBeUsed) return;

        if (obj is not InputAction inputAction || inputAction.activeControl == null) return;

        if (inputAction.activeControl.device is InputDevice newDevice && _lastInputDevice != newDevice)
        {
            _lastInputDevice = newDevice;

            if (newDevice.displayName == "Mouse" ||
                (newDevice.displayName == "Keyboard" && _treatMouseAndKeyboardAsSameDevice)
            )
            {
                if (CursorState) return;
                CursorState = true;
            }
            else
            {
                if (!CursorState) return;
                CursorState = false;
            }
        }
    }
}
