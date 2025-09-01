namespace KGGEEP
{
    using System.IO;
    using System.Reflection;

    using UnityEditor;
    using UnityEditor.SceneManagement;

    using UnityEngine;

    internal static class StorageProvider
    {
        private static IKeyValueStorage storage;
        static StorageProvider()
        {
            GlobalObjectIdNullString = GlobalObjectId.GetGlobalObjectIdSlow(null).ToString();
            EditorApplication.delayCall += () => {
                var settings = ProjectSettings.instance;
                if (settings.enableAllFeatures)
                {
                    Enable();
                }
            };
        }

        private static string GlobalObjectIdNullString { get; }

        public static void Set(GameObject target, FeatureKey featureKey, string value)
        {
            if (storage == null)
            {
                return; // 비활성화 상태
            }

            var key = GetKey(target, featureKey);
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            storage.Put(key, value);
        }

        public static string Get(GameObject target, FeatureKey featureKey)
        {
            if (storage == null)
            {
                return string.Empty;
            }

            var key = GetKey(target, featureKey);
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return storage.TryGet(key, out var value) ? value : string.Empty;
        }

        public static void Enable()
        {
            if (storage != null)
            {
                return;
            }

            var dir = Path.Combine(Directory.GetCurrentDirectory(), $".{Constants.ShortName}");
            storage = new RocksDbStore(dir);
            EditorApplication.quitting += QuitCallback;
        }

        public static void Disable()
        {
            Application.quitting -= QuitCallback;

            storage?.Dispose();
            storage = null;
        }

        public static string GetUniqueId(GameObject target)
        {
            string id = string.Empty;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                // 프리팹 수정 중
                SerializedObject serializedObject = new SerializedObject(target);
                PropertyInfo inspectorModeInfo = typeof(SerializedObject)
                    .GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

                inspectorModeInfo?.SetValue(serializedObject, InspectorMode.Debug, null);

                SerializedProperty localIdProp = serializedObject.FindProperty(
                    "m_LocalIdentfierInFile");
                if (localIdProp != null)
                {
                    var guid = AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
                    id = $"{guid}:{localIdProp.longValue}";
                }
            }
            else if (PrefabUtility.IsPartOfPrefabAsset(target) ||
                PrefabUtility.IsPartOfPrefabInstance(target))
            {
                id = GetPrefabId();
            }

            if (string.IsNullOrEmpty(id))
            {
                var globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(target).ToString();
                if (string.IsNullOrEmpty(globalObjectId) == false &&
                    globalObjectId != GlobalObjectIdNullString)
                {
                    id = globalObjectId;
                }
            }

            return id;

            string GetPrefabId()
            {
                var findObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target) ??
                    PrefabUtility.GetCorrespondingObjectFromSource(target) ??
                    target;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
                    findObject,
                    out var assetGuid,
                    out var fileId) == false ||
                    string.IsNullOrEmpty(assetGuid))
                {
                    return string.Empty;
                }

                if (fileId == 0)
                {
                    return string.Empty;
                }

                return $"{assetGuid}:{fileId}";
            }
        }

        // ---------------------------------------------------------------------------------
        private static string GetKey(GameObject target, FeatureKey featureKey)
        {
            string debugName = $"[{target.name}/{featureKey}]";
            if (target == null || featureKey == FeatureKey.Invalid)
            {
                return string.Empty;
            }

            var id = GetUniqueId(target);
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError($"{debugName} invalid uniqueId");
                return string.Empty;
            }

            var result = $"{featureKey}:{id}";
            return $"{result}";
        }

        private static void QuitCallback()
        {
            storage?.Dispose();
        }
    }
}