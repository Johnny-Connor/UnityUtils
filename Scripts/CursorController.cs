using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    // Variables.
    [Tooltip("Displays cursor when mouse is used.")]
    [SerializeField] private bool _enableCursor;

    [Tooltip("If enabled, the cursor's visibility will not be updated when switching between mouse and "
    + "keyboard devices.")]
    [SerializeField] private bool _treatMouseAndKeyboardAsSameDevice;

    /*
    A timer to limit the frequency of CursorLockMode calls. Prevents performance impact when player 
    purposely spams inputs from different devices.
    */
    private Timer _cursorLockModeUpdateCooldownTimer;
    private readonly float _cursorLockModeUpdateCooldown = 0.1f;
    private bool _canUpdateCursorLockMode;


    // MonoBehaviour.
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;

        _cursorLockModeUpdateCooldownTimer = new(() => _canUpdateCursorLockMode = true);
        _cursorLockModeUpdateCooldownTimer.StartTimer(0);

        InputSystem.onActionChange += InputSystem_OnActionChange;
    }

    private void Update() => _cursorLockModeUpdateCooldownTimer.UpdateTimer();


    // Event Handlers.
    private void InputSystem_OnActionChange(object obj, InputActionChange inputActionChange)
    {
        if (!_enableCursor || !_canUpdateCursorLockMode) return;

        if (obj is not InputAction inputAction || inputAction.activeControl == null) return;

        if (inputAction.activeControl.device is InputDevice newDevice)
        {
            bool showCursor = _treatMouseAndKeyboardAsSameDevice ? 
                newDevice.displayName == "Mouse" || newDevice.displayName == "Keyboard" :
                newDevice.displayName == "Mouse"
            ;
            CursorLockMode cursorLockMode = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
            
            Cursor.lockState = cursorLockMode;
            _canUpdateCursorLockMode = false;
            _cursorLockModeUpdateCooldownTimer.StartTimer(_cursorLockModeUpdateCooldown);
        }
    }
}
