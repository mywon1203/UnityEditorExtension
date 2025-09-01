namespace KGGEEP
{
    using System;

    using UnityEditor;

    [FilePath(Constants.SettingsAssetPath, FilePathAttribute.Location.ProjectFolder)]
    public class ProjectSettings : ScriptableSingleton<ProjectSettings>
    {
        public static event Action OnChanged;

        public bool enableAllFeatures;

        public HierarchySettings hierarchy = new();
        public InspectorSettings inspector = new();
        public GizmoSettings gizmo = new();

        public void SaveChanged()
        {
            this.Save(saveAsText: true);
            if (this.enableAllFeatures)
            {
                StorageProvider.Enable();
            }
            else
            {
                StorageProvider.Disable();
            }

            OnChanged?.Invoke();
        }
    }
}