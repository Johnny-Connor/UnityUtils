using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Layout Group Refresh Problem refers to an issue where the expected refreshing and rebuilding of a
/// <see cref="LayoutGroup"/> due to the change of its child content does not happen at the current frame 
/// of which said change occured, causing the UI to briefly "flash" with an incorrect look for a frame 
/// before correcting itself.
/// <para>
/// Solution discussed and found in: 
/// https://forum.unity.com/threads/layoutgroup-does-not-refresh-in-its-current-frame.458446/
/// </para>
/// </summary>
public static class LayoutGroupRefreshProblemSolver
{
    public static void RefreshLayoutGroupsImmediateAndRecursive(LayoutGroup alteredLayoutGroup)
    {
        LayoutGroup rootLayoutGroup = FindRootLayoutGroup(alteredLayoutGroup);

        LayoutGroup[] layoutGroupsInRootChildren = rootLayoutGroup.GetComponentsInChildren<LayoutGroup>();

        for (int i = layoutGroupsInRootChildren.Length - 1; i >= 0; i--)
        {
            LayoutGroup layoutGroup = layoutGroupsInRootChildren[i];
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        static LayoutGroup FindRootLayoutGroup(LayoutGroup alteredLayoutGroup)
        {
            LayoutGroup[] parentLayoutGroups = 
                alteredLayoutGroup.gameObject.GetComponentsInParent<LayoutGroup>()
            ;
            return parentLayoutGroups[^1];
        }
    }
}
