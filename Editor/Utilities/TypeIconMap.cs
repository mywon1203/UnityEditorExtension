#nullable enable
namespace KGGEEP
{
    using System;

    using UnityEditor;

    using UnityEngine;

    internal static class TypeIconMap
    {
        private const string ScriptIconPath = "cs Script Icon";
        private const string ScriptableObjectIconPath = "ScriptableObject Icon";
        private const string InterfaceIconPath = "interface icon";
        private const string EnumIconPath = "enum script icon";

        public static Texture2D? Get(Type? type)
        {
            if (type == null)
            {
                return null;
            }

            var content = EditorGUIUtility.ObjectContent(obj: null, type);
            if (content?.image != null)
            {
                return content.image as Texture2D;
            }

            if (type.Namespace.StartsWith("UnityEngine"))
            {
                return null;
            }

            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return EditorGUIUtility.IconContent(ScriptIconPath).image as Texture2D;
            }

            if (type.IsSubclassOf(typeof(ScriptableObject)))
            {
                return EditorGUIUtility.IconContent(ScriptableObjectIconPath).image as Texture2D;
            }

            if (type.IsInterface)
            {
                return EditorGUIUtility.IconContent(InterfaceIconPath).image as Texture2D;
            }

            if (type.IsEnum)
            {
                return EditorGUIUtility.IconContent(EnumIconPath).image as Texture2D;
            }

            return EditorGUIUtility.IconContent(ScriptIconPath).image as Texture2D;
        }
    }
}
