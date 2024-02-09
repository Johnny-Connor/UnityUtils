using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
[RequireComponent(typeof(Image))]
// An interactable that does not take the focus from a selected Selectable upon interaction.
public class UnfocusableInteractable :
    MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler
{
    // Variables.
    private Image _interactableImage;
    private bool _interactable = true;
 
 
    // Properties.
    public bool Interactable
    {
        get => _interactable;
        set
        {
            _interactableImage.color = value ?
                ColorBlock.defaultColorBlock.normalColor :
                ColorBlock.defaultColorBlock.disabledColor
            ;
            _interactable = value;
        }
    }
 
 
    // Events.
    public Action OnClick;
 
 
    // MonoBehaviour.
    private void Awake() => _interactableImage = GetComponent<Image>();
 
 
    // Interface Methods.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable) OnClick?.Invoke();
    }
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactable) _interactableImage.color = ColorBlock.defaultColorBlock.pressedColor;
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Interactable) _interactableImage.color = ColorBlock.defaultColorBlock.highlightedColor;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Interactable) _interactableImage.color = ColorBlock.defaultColorBlock.normalColor;
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Interactable) _interactableImage.color = ColorBlock.defaultColorBlock.selectedColor;
    }
}
