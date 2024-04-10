using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
// Auto-updates TextMeshProUGUI's size as it changes during runtime. Does not work with 'stretch' anchors.
public class TextMeshProUGUIAutoSizer : MonoBehaviour
{
    // Variables.
    private TMPro.TextMeshProUGUI _tmProUGUI;
    private RectTransform rectTransform;
    private float _currentPreferredWidth;
    private float _currentPreferredHeight;
    private bool _belongsToALayoutGroup;


    // MonoBehaviour Methods.
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _tmProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
        _belongsToALayoutGroup = GetComponentInParent<LayoutGroup>();
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

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

            if (_belongsToALayoutGroup)
                LayoutGroupRefreshProblemSolver.RefreshLayoutGroupsImmediateAndRecursive(gameObject)
            ;
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
