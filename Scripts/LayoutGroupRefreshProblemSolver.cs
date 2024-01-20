using UnityEngine;
using UnityEngine.UI;

/*
The Layout Group Refresh Problem refers to an issue where the expected refreshing and rebuilding of a
Layout Group component due to the change of its child content does not happen at the current frame of 
which said change occured, causing the UI to briefly "flash" with an incorrect look for a frame before 
correcting itself.
Solution discussed and found in: 
https://forum.unity.com/threads/layoutgroup-does-not-refresh-in-its-current-frame.458446/
*/
public static class LayoutGroupRefreshProblemSolver
{
    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject layoutGroupRoot)
    {
        var componentsInChildren = layoutGroupRoot.GetComponentsInChildren<LayoutGroup>();

        foreach (var layoutGroup in componentsInChildren)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        var parent = layoutGroupRoot.GetComponent<LayoutGroup>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }
}
