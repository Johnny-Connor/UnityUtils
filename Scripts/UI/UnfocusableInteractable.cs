using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// An interactable that does not take focus from the current selection when interacted with.
/// <para>
/// Note: If the <see cref="GameObject"/> containing this has any other script implementing 
/// <see cref="ISelectHandler"/>, the <see cref="EventSystem"/> will clear the selection when this 
/// interactable is clicked.
/// </para>
/// </summary>
[ExecuteAlways]
public class UnfocusableInteractable:
    UIBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler
{
    // Variables.
    [SerializeField] private bool _interactable = true;
    [SerializeField] private Graphic _targetGraphic;
    [SerializeField] private ColorBlock _colorBlock = ColorBlock.defaultColorBlock;


    // Properties.
    public ColorBlock ColorBlock
    { 
        get => _colorBlock; 
        set 
        {
            _colorBlock = value;
            UpdateGraphicColor();
        }
    }

    public bool Interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;
            UpdateGraphicColor();
        }
    }


    // Events.
    public event EventHandler OnPointerClicked;
    public event EventHandler OnPointerDowned;
    public event EventHandler OnPointerEntered;
    public event EventHandler OnPointerExited;
    public event EventHandler OnPointerUped;


    // MonoBehaviour Methods.
    protected override void Awake() => UpdateGraphicColor();
    protected override void OnValidate() => UpdateGraphicColor();
    protected override void OnEnable() => StartColorTween(_colorBlock.normalColor, true);
    protected override void OnDisable() => StartColorTween(Color.white, true);


    // Methods.
    private void UpdateGraphicColor() => 
        StartColorTween(_interactable ? _colorBlock.normalColor : _colorBlock.disabledColor, true)
    ;

    private void StartColorTween(Color targetColor, bool instant = false)
    {
        if (!_targetGraphic) return;

        _targetGraphic.CrossFadeColor(
            targetColor * _colorBlock.colorMultiplier, 
            instant ? 0f : _colorBlock.fadeDuration, 
            true, 
            true
        );
    }


    // Interface Methods.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable) OnPointerClicked?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;
        StartColorTween(_colorBlock.pressedColor);
        OnPointerDowned?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
        StartColorTween(_colorBlock.highlightedColor);
        OnPointerEntered?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;
        StartColorTween(_colorBlock.normalColor);
        OnPointerExited?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;
        StartColorTween(_colorBlock.normalColor);
        OnPointerUped?.Invoke(this, EventArgs.Empty);
    }
}
