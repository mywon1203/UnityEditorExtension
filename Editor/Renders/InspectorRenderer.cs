namespace KGGEEP
{
    using System.Reflection;

    using UnityEditor;

    using UnityEngine;

    [InitializeOnLoad]
    internal static class InspectorRenderer
    {
        private static bool foldout = true;
        private static bool debugFoldout = true;

        static InspectorRenderer()
        {
            Editor.finishedDefaultHeaderGUI += OnAfterDefaultHeaderGui;
        }

        // ---------------------------------------------------------------------------------
        private static bool IsDebugMode(Editor editor)
        {
            PropertyInfo inspectorModeInfo = typeof(SerializedObject)
                    .GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            var inspectorMode = (InspectorMode)inspectorModeInfo.GetValue(editor.serializedObject);
            return inspectorMode == InspectorMode.Debug;
        }

        private static void OnAfterDefaultHeaderGui(Editor editor)
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating ||
                editor == null ||
                editor.target is not GameObject target)
            {
                return;
            }

            ProjectSettings projectSettings = ProjectSettings.instance;
            if (projectSettings.enableAllFeatures == false ||
                projectSettings.inspector.enable == false)
            {
                return;
            }

            var setting = projectSettings.inspector;
            var rect = GUILayoutUtility.GetRect(10000, 1);
            if (IsDebugMode(editor) || setting.alwaysShowDebugInfo)
            {
                EditorGUI.DrawRect(rect, Color.black);
                DrawDebug(target);
            }

            if (target.CompareTag(Constants.EditorOnlyTag) &&
                target.name.StartsWith(projectSettings.hierarchy.seperatorPrefix))
            {
                return;
            }

            EditorGUI.DrawRect(rect, Color.black);

            switch (setting.displayMode)
            {
                case InspectorSettings.DisplayMode.Always:
                    DrawBody(target, setting);
                    break;

                case InspectorSettings.DisplayMode.Foldout:
                    using (var _ = new EditorGUI.IndentLevelScope())
                    {
                        foldout = EditorGUILayout.Foldout(
                            foldout,
                            Constants.ShortName,
                            toggleOnLabelClick: true);
                        if (!foldout)
                        {
                            break;
                        }

                        DrawBody(target, setting);
                    }

                    break;
            }
        }

        private static void DrawDebug(GameObject target)
        {
            EditorGUILayout.Space();
            var id = StorageProvider.GetUniqueId(target);
            using var _ = new EditorGUI.IndentLevelScope();
            debugFoldout = EditorGUILayout.Foldout(
                debugFoldout,
                "KGGEEP Debug Info",
                toggleOnLabelClick: true);
            if (debugFoldout == false)
            {
                return;
            }

            var instanceId = target.GetInstanceID();
            EditorGUILayout.LabelField("InstanceId", instanceId.ToString());
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("UniqueId");

                GUIStyle style = new GUIStyle();
                var content = new GUIContent(id, "click to copy");
                var hieght = GUILayout.Height(EditorStyles.textArea.CalcHeight(
                    content,
                    EditorGUIUtility.currentViewWidth - 200));
                if (GUILayout.Button(content, EditorStyles.textArea, hieght))
                {
                    EditorGUIUtility.systemCopyBuffer = id;
                }
            }

            if (GameObjectContainer.TryGetValue(instanceId, out var item) == false)
            {
                EditorGUILayout.HelpBox("Not Found Data", MessageType.Warning);
                return;
            }

            using (new EditorGUI.DisabledGroupScope(disabled: true))
            {
                EditorGUILayout.RectField("Selection Rect", item.SelectionRect);
                EditorGUILayout.RectField("Full Rect", item.FullRect);
            }

            EditorGUILayout.LabelField("Visual Index", item.VisiualIndex.ToString());
            EditorGUILayout.LabelField("Visual Index", item.Depth.ToString());
            EditorGUILayout.LabelField("MissiongCount", item.MissingComponentCount.ToString());
            EditorGUILayout.LabelField("Child Count", item.ChildCount.ToString());
        }

        private static void DrawBody(GameObject target, InspectorSettings settings)
        {
            DrawBackground(target);
            EditorGUILayout.Space(1);
            DrawNote(target);
        }

        private static void DrawBackground(GameObject target)
        {
            var exists = target.TryGetBackgroundColor(out var color);

            using var _ = new EditorGUILayout.HorizontalScope();
            Color next = EditorGUILayout.ColorField(
                "Background Color",
                color);

            if (exists || next.a > 0)
            {
                target.SetBackgroundColor(next);
            }

            if (GUILayout.Button("Clear"))
            {
                target.SetBackgroundColor(Color.clear);
            }
        }

        private static void DrawNote(GameObject self)
        {
            var current = self.GetNote();
            using var _ = new EditorGUILayout.HorizontalScope();
            EditorGUILayout.PrefixLabel(FeatureType.Note.ToString());
            EditorGUI.BeginChangeCheck();
            var next = EditorGUILayout.TextArea(current, GUI.skin.textArea);
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            self.SetNote(next);
        }
    }
}