using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
// Auto-updates TextMeshProUGUI's size as it changes during runtime.
public class TextMeshProUGUIAutoSizer : MonoBehaviour
{
    // Variables.
    private TMPro.TextMeshProUGUI _tmProUGUI;
    private RectTransform rectTransform;
    private float _currentPreferredWidth;
    private float _currentPreferredHeight;


    // MonoBehaviour.
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _tmProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void OnEnable() => SetSize();
    private void Update() => SetSize();


    // Non-MonoBehaviour.
    private void SetSize()
    {
        if (_currentPreferredWidth != _tmProUGUI.preferredWidth || 
            _currentPreferredHeight != _tmProUGUI.preferredHeight
        )
        {
            if (_currentPreferredWidth != _tmProUGUI.preferredWidth) SetWidth();
            if (_currentPreferredHeight != _tmProUGUI.preferredHeight) SetHeight();

            UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
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
