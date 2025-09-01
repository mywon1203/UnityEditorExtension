namespace KGGEEP
{
    using Unity.Hierarchy;

    using UnityEditor;

    using UnityEngine;

    internal static class HierarchyContextMenu
    {
        [MenuItem("GameObject/Create Seperator", isValidateFunction: false, priority: 0)]
        private static void CreateSeperator()
        {
            var settings = ProjectSettings.instance.hierarchy;

            var name = GameObjectUtility.GetUniqueNameForSibling(parent: null, $"{settings.seperatorPrefix} Seperator");

            var seperator = new GameObject(name);

            seperator.transform.position = Vector3.zero;
            seperator.tag = Constants.EditorOnlyTag;
            Selection.activeGameObject = seperator;
            Undo.RegisterCreatedObjectUndo(seperator, "Create Seperator");
        }
    }
}