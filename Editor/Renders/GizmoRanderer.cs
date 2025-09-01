namespace KGGEEP
{
    using UnityEditor;

    using UnityEngine;

    [InitializeOnLoad]
    public static class GizmoRenderer
    {
        // gizmo icon size * offset factor
        private const float Constant = 32 * 0.0065f;

        private static GUIStyle noteStyle;
        private static Vector3 rightVector;

        static GizmoRenderer()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        // ---------------------------------------------------------------------------------

        private static void OnSceneGUI(SceneView sceneView)
        {
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        private static void DrawGizmoIcon(Transform transform, GizmoType _)
        {
            var projectSettings = ProjectSettings.instance;
            if (projectSettings.enableAllFeatures == false || 
                projectSettings.gizmo.enable == false)
            {
                return;
            }

            var settings = ProjectSettings.instance.gizmo;
            UpdateVector();

            if (settings.useHierarchyColorForOutline)
            {
                DrawOutline(transform, projectSettings.hierarchy);
            }

            if (settings.noteVisiable)
            {
                DrawNote(transform, projectSettings.gizmo);
            }

            static void UpdateVector()
            {
                rightVector = Vector3.right;
                if (Camera.current != null)
                {
                    rightVector = Camera.current.transform.right;
                }
                else if (SceneView.currentDrawingSceneView?.camera != null)
                {
                    rightVector = SceneView.currentDrawingSceneView.camera.transform.right;
                }
            }
        }

        private static void DrawOutline(Transform transform, HierarchySettings settings)
        {
            var parentCount = 0;
            var parent = transform.parent;
            while (parent != null)
            {
                parentCount++;
                parent = parent.parent;
            }

            var color = settings.GetDepthColor(parentCount);
            Handles.DrawOutline(new[] { transform.gameObject }, color);
        }

        private static void DrawNote(Transform transform, GizmoSettings settings)
        {
            var note = transform.gameObject.GetNote();
            if (string.IsNullOrEmpty(note))
            {
                return;
            }

            if (noteStyle == null)
            {
                noteStyle = new GUIStyle
                {
                    normal = { textColor = settings.noteTextColor, background = Texture2D.grayTexture },
                    fontSize = settings.noteTextSize,
                    padding = new(settings.notePaddingLeft, settings.notePaddingRight, settings.notePaddingTop, settings.notePaddingBottom),
                    wordWrap = true, // 텍스트 줄바꿈 활성화
                    imagePosition = ImagePosition.TextOnly,
                    richText = true,
                };
            }
            else
            {
                noteStyle.normal.textColor = settings.noteTextColor;
                noteStyle.fontSize = settings.noteTextSize;
                noteStyle.padding = new(settings.notePaddingLeft, settings.notePaddingRight, settings.notePaddingTop, settings.notePaddingBottom);
            }

            var position = transform.position + (Constant * rightVector);
            Vector3 guiPosition = HandleUtility.WorldToGUIPoint(position);
            Handles.BeginGUI();
            using (new BackgroundColorScope(settings.noteBackgroundColor))
            {
                Vector2 size = noteStyle.CalcSize(new GUIContent(note));
                size.x += settings.notePaddingLeft + settings.notePaddingRight;
                size.y += settings.notePaddingTop + settings.notePaddingBottom;
                GUI.Label(new Rect(guiPosition.x, guiPosition.y, size.x, size.y), note, noteStyle);
            }

            Handles.EndGUI();
        }
    }
}