
namespace KGGEEP
{
    using UnityEditor;

    using UnityEngine;

    internal static class IconExtension
    {
        public static void SetIcon(this GameObject self, Texture2D iconTexture)
        {
            if (self == null)
            {
                return;
            }

            EditorGUIUtility.SetIconForObject(self, iconTexture);
        }

        public static Texture2D GetIcon(this GameObject self)
        {
            if (self == null)
            {
                return null;
            }

            return EditorGUIUtility.GetIconForObject(self);
        }
    }
}