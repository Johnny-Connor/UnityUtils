using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Controls cursor visibility based on the current input device.
/// <para>
/// Note 1: <see cref="CursorLockMode"\n> is not being used because it currently performs very poorly.
/// </para>
/// <para>
/// Note 2: For consistent testing in the editor, ensure the 'Game' window is focused after starting the
/// game to allow Unity's cursor control methods to work as intended.
/// </para>
/// </summary>
public class CursorVisibilityController : MonoBehaviour
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
                if (graphicRaycaster != null) graphicRaycaster.enabled = value
            ;
        }
    }


    // MonoBehaviour Methods.
    private void Awake() => CursorState = _canCursorBeUsed;
    private void OnEnable() => InputSystem.onActionChange += InputSystem_OnActionChange;
    private void OnDisable() => InputSystem.onActionChange -= InputSystem_OnActionChange;


    // Event Handlers.
    private void InputSystem_OnActionChange(object obj, InputActionChange inputActionChange)
    {
        if (!_canCursorBeUsed) return;

        if (obj is not InputAction inputAction || inputAction.activeControl == null) return;

        if (inputAction.activeControl.device is InputDevice newDevice && _lastInputDevice != newDevice)
        {
            _lastInputDevice = newDevice;

            if
            (
                newDevice.displayName == "Mouse" ||
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
