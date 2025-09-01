#nullable enable
namespace KGGEEP
{
    using System;
    using System.Collections.Generic;

    using UnityEditor;

    using UnityEngine;

    public static class TypeField
    {
        private readonly static Dictionary<int, Type> pendingSelections = new Dictionary<int, Type>();

        public static Type? Draw(string label, Type selectedType, Type baseType)
        {
            return Draw(EditorGUILayout.GetControlRect(), label, selectedType, baseType);
        }

        public static Type? Draw(Type selectedType, Type baseType)
        {
            return Draw(EditorGUILayout.GetControlRect(), string.Empty, selectedType, baseType);
        }

        public static Type? Draw(Rect position, string label, Type selectedType, Type baseType)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);

            if (!string.IsNullOrEmpty(label))
            {
                position = EditorGUI.PrefixLabel(position, new GUIContent(label));
            }

            if (pendingSelections.TryGetValue(id, out Type pendingType))
            {
                selectedType = pendingType;
                pendingSelections.Remove(id);
                GUI.changed = true;
            }

            var content = selectedType == null ? 
                new GUIContent("None", TypeIconMap.Get(type: null)) :
                new GUIContent(selectedType.Name, TypeIconMap.Get(selectedType));

            if (GUI.Button(position, content, EditorStyles.objectField))
            {
                var screenPos = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.yMax));
                TypeSelectorWindow.Show(baseType, type =>
                {
                    pendingSelections[id] = type;
                }, screenPos);
            }

            return selectedType;
        }
    }
}
