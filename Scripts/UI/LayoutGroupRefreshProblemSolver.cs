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
    private static LayoutGroup FindRootLayoutGroup(GameObject alteredGameObject)
    {
        LayoutGroup[] parentLayoutGroups = alteredGameObject.GetComponentsInParent<LayoutGroup>();
        return parentLayoutGroups[^1];
    }

    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject alteredGameObject)
    {
        if (!alteredGameObject.GetComponentInParent<LayoutGroup>())
        {
            Debug.LogWarning(alteredGameObject + " does not belong to any LayoutGroup. Returning method.");
            return;
        }

        LayoutGroup rootLayoutGroup = FindRootLayoutGroup(alteredGameObject);

        LayoutGroup[] layoutGroupsInRootChildren = rootLayoutGroup.GetComponentsInChildren<LayoutGroup>();

        foreach (LayoutGroup layoutGroup in layoutGroupsInRootChildren)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>())
        ;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootLayoutGroup.GetComponent<RectTransform>());
    }
}
