namespace KGGEEP
{
    using UnityEngine;

    // TODO: 동작은 하지만 아직 구현하지 않았습니다.
    internal static class TagExtension
    {
        public static void SetTag(this GameObject self, string key, string value)
        {
            if (self == null)
            {
                return;
            }

            var featureKey = FeatureKey.Create(key);
            if (featureKey == null)
            {
                return;
            }

            StorageProvider.Set(self, featureKey, value);
        }

        public static string GetTag(this GameObject self, string key)
        {
            if (self == null)
            {
                return null;
            }

            var featureKey = FeatureKey.Create(key);
            if (featureKey == null)
            {
                return null;
            }

            return StorageProvider.Get(self, featureKey);
        }
    }
}