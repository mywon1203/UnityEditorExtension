namespace KGGEEP
{
    using UnityEngine;

    internal static class NoteExtension
    {
        public static void SetNote(this GameObject self, string note)
        {
            if (self == null)
            {
                return;
            }

            StorageProvider.Set(self, FeatureKey.Note, note);
        }

        public static string GetNote(this GameObject self)
        {
            if (self == null)
            {
                return null;
            }

            return StorageProvider.Get(self, FeatureKey.Note);
        }
    }
}