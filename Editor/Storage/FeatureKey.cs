namespace KGGEEP
{
    internal readonly struct FeatureKey
    {
        private readonly FeatureType feature;
        private readonly string tagKey;

        public static FeatureKey Note => new FeatureKey(FeatureType.Note, string.Empty);
        public static FeatureKey BackgroundColor => new FeatureKey(FeatureType.BackgroundColor, string.Empty);
        public static FeatureKey Invalid => new FeatureKey(FeatureType.Tag, string.Empty);

        private FeatureKey(FeatureType feature, string tagKey)
        {
            this.feature = feature;
            this.tagKey = tagKey;
        }

        public static FeatureKey Create(string tagKey)
        {
            if (string.IsNullOrEmpty(tagKey))
            {
                return Invalid;
            }

            // TODO: 태그 기능 구현 후 주석 해제
            //List<string> keys = ProjectSettings.instance.TagKeys;
            //if (keys.Count <= 0)
            //{
            //    return Invalid;
            //}
            //
            //if (keys.Contains(tagKey) == false)
            //{
            //    return Invalid;
            //}

            return new FeatureKey(FeatureType.Tag, tagKey);
        }

        public static implicit operator string(FeatureKey key)
        {
            if (key.Equals(Invalid))
            {
                return string.Empty;
            }

            return key.feature switch
            {
                FeatureType.Note => "note",
                FeatureType.BackgroundColor => "background_color",
                FeatureType.Tag => $"tag_{key.tagKey}",
                _ => throw new System.ArgumentOutOfRangeException(nameof(key.feature), key.feature, null),
            };
        }

        public override string ToString()
        {
            return this;
        }
    }
}