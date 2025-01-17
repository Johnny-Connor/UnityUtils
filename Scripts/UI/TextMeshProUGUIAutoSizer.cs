using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically adjusts the size of a <see cref="TMPro.TextMeshProUGUI"/> component to match its text 
/// content at runtime. Does not work with 'stretch' anchors.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class TextMeshProUGUIAutoSizer : MonoBehaviour
{
    // Variables.
    private TMPro.TextMeshProUGUI _tmProUGUI;
    private RectTransform rectTransform;
    private float _currentPreferredWidth;
    private float _currentPreferredHeight;


    // MonoBehaviour Methods.
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _tmProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void OnEnable() => SetSize();
    private void Update() => SetSize();


    // Methods.
    private void SetSize()
    {
        if (_currentPreferredWidth != _tmProUGUI.preferredWidth || 
            _currentPreferredHeight != _tmProUGUI.preferredHeight
        )
        {
            if (_currentPreferredWidth != _tmProUGUI.preferredWidth) SetWidth();
            if (_currentPreferredHeight != _tmProUGUI.preferredHeight) SetHeight();

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    private void SetWidth()
    {
        rectTransform.sizeDelta = new Vector2(_tmProUGUI.preferredWidth, rectTransform.sizeDelta.y);
        _currentPreferredWidth = _tmProUGUI.preferredWidth;
    }

    private void SetHeight()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _tmProUGUI.preferredHeight);
        _currentPreferredHeight = _tmProUGUI.preferredHeight;
    }
}
