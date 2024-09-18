using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
An interactable that does not take the focus from a selected Selectable upon interaction.
Note: If the GameObject containing this script has any other script with the ISelectHandler interface in it, 
the EventSystem will clear the selection whenever this interactable is clicked.
*/
public class UnfocusableInteractable:
    MonoBehaviour,
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
    private void Awake()
    {
        if (GetComponent<Selectable>())
            Debug.LogError($"Do not attach {nameof(Selectable)}s to {nameof(UnfocusableInteractable)}s.")
        ;

        UpdateGraphicColor();
    }

    private void OnValidate() => UpdateGraphicColor();


    // Methods.
    private void UpdateGraphicColor()
    {
        if (_targetGraphic)
            _targetGraphic.color = _interactable ? _colorBlock.normalColor : _colorBlock.disabledColor
        ;
    }


    // Interface Methods.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable) OnPointerClicked?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (_targetGraphic) _targetGraphic.color = _colorBlock.pressedColor;

        OnPointerDowned?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (_targetGraphic) _targetGraphic.color = _colorBlock.highlightedColor;

        OnPointerEntered?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (_targetGraphic) _targetGraphic.color = _colorBlock.normalColor;

        OnPointerExited?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) return;

        if (_targetGraphic) _targetGraphic.color = _colorBlock.selectedColor;

        OnPointerUped?.Invoke(this, EventArgs.Empty);
    }
}
