namespace KGGEEP
{
    using System;

    using UnityEditor;

    using UnityEngine;

    public static class ProjectSettingsProvider
    {
        private static readonly string[] TabNames = new string[]
        {
            "Hierarchy",
            "Inspector",
            "Gizmo",
        };

        private static int SelectedTab = 0;

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider(
                Constants.SettingsProviderPath,
                SettingsScope.Project)
            {
                label = Constants.FullName,
                guiHandler = GuiHandler,
                deactivateHandler = Close,
            };
        }

        // ---------------------------------------------------------------------------------
        private static void Close()
        {
            if (ProjectSettings.instance == null)
            {
                return;
            }

            ProjectSettings.instance.SaveChanged();
        }

        private static void GuiHandler(string _)
        {
            var settings = ProjectSettings.instance;
            // 마스터 스위치
            settings.enableAllFeatures = EditorGUILayout.ToggleLeft(
                "Enable All Features",
                settings.enableAllFeatures);

            using (var scope = new EditorGUI.DisabledScope(!settings.enableAllFeatures))
            {
                EditorGUILayout.Space();
                using (var tabVirtialScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SelectedTab = GUILayout.Toolbar(SelectedTab, TabNames, EditorStyles.toolbarButton);
                    EditorGUILayout.Space();

                    EditorGUI.DisabledScope tabScope;
                    switch (SelectedTab)
                    {
                        case 0:
                            tabScope = settings.hierarchy.OnGui();
                            break;
                        case 1:
                            tabScope = settings.inspector.OnGui();
                            break;
                        case 2:
                            tabScope = settings.gizmo.OnGui();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(SelectedTab), "Invalid tab selected.");
                    }

                    tabScope.Dispose();
                }
            }
        }
    }
}