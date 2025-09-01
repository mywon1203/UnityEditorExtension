namespace KGGEEP
{
    using UnityEngine;

    internal static class HierarchyBackgroundColorExtension
    {
        public static void SetBackgroundColor(this GameObject self, Color color)
        {
            if (self == null)
            {
                return;
            }

            if (color.a > 0.3f)
            {
                color.a = 0.3f;
            }

            StorageProvider.Set(self, FeatureKey.BackgroundColor, $"#{ColorUtility.ToHtmlStringRGBA(color)}");
        }

        public static bool TryGetBackgroundColor(this GameObject self, out Color color)
        {
            if (self == null)
            {
                color = default;
                return false;
            }

            string colorStr = StorageProvider.Get(self, FeatureKey.BackgroundColor);
            if (string.IsNullOrEmpty(colorStr))
            {
                color = default;
                return false;
            }

            if (ColorUtility.TryParseHtmlString(colorStr, out color))
            {
                return true;
            }

            return false;
        }
    }
}