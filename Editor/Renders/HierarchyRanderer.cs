namespace KGGEEP
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;

    [InitializeOnLoad]
    public static class HierarchyRenderer
    {
        private const float indentWidth = 14f;
        private const float halfIndentWidth = indentWidth * 0.5f;
        private static Texture2D missingComponentIcon;
        
        // separator
        private static GUIStyle separatorStyle;

        // icon
        private const float iconSize = 16f;

        // note
        private static Texture2D noteIcon;

        static HierarchyRenderer()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            EditorApplication.hierarchyChanged -= GameObjectContainer.OnHierarchyChanged;
            EditorApplication.hierarchyChanged += GameObjectContainer.OnHierarchyChanged;

            ProjectSettings.OnChanged -= SettingsChanged;
            ProjectSettings.OnChanged += SettingsChanged;
        }

        // ---------------------------------------------------------------------------------

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ReloadScripts()
        {
            separatorStyle = null;
            GameObjectContainer.OnHierarchyChanged();
        }

        private static void SettingsChanged()
        {
            separatorStyle = null;
            GameObjectContainer.OnHierarchyChanged();
        }

        private static void OnHierarchyItemGUI(int instanceId, Rect selectionRect)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            var projectSettings = ProjectSettings.instance;
            if (projectSettings.enableAllFeatures == false ||
                projectSettings.hierarchy.enable == false)
            {
                return;
            }

            
            var setting = projectSettings.hierarchy;
            GameObject targetObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (targetObject == null)
            {
                return;
            }

            if (GameObjectContainer.TryGetValue(instanceId, out var item) == false)
            {
                return;
            }

            item.UpdateCache(selectionRect);
            // 구분선은 기타 처리를 하지 않습니다.
            if (item.IsSeperator)
            {
                DrawSeparator(item, setting);
                return;
            }

            if (setting.enableBackgroundColor)
            {
                DrawBackgroundColor(item, setting);
            }

            if (setting.enableLine)
            {
                DrawLine(item, setting);
            }

            List<Texture2D> rightIcons = new();
            if (setting.componentIconsVisiable)
            {
                var icons = GatherComponentsIcons(item, setting);
                rightIcons.AddRange(icons);
            }

            var hasNote = false;
            if (setting.noteVisiable || setting.noteIconVisiable)
            {
                var note = item.Target.GetNote();
                hasNote = string.IsNullOrEmpty(note) == false;
                if (hasNote && setting.noteVisiable)
                {
                    DrawNote(item, setting);
                }

                if (hasNote && setting.noteIconVisiable)
                {
                    if (noteIcon == null)
                    {
                        noteIcon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
                    }

                    switch (setting.noteIconOrder)
                    {
                        case HierarchySettings.NoteIconOrder.Left:
                            rightIcons.Insert(0, noteIcon);
                            break;
                        case HierarchySettings.NoteIconOrder.Right:
                            rightIcons.Add(noteIcon);
                            break;
                    }
                }
            }

            if (setting.gizmoIconVisiable)
            {
                DrawGizmoIcon(item, setting);
            }

            DrawRightIcons(item, rightIcons);
        }

        private static void DrawSeparator(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            if (separatorStyle == null)
            {
                separatorStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    clipping = TextClipping.Ellipsis,
                    normal = new GUIStyleState()
                    {
                        textColor = settings.seperatorTextColor,
                    },
                };
            }
            else
            {
                separatorStyle.normal.textColor = settings.seperatorTextColor;
            }

            EditorGUI.DrawRect(item.FullRect, settings.separatorBackgroundColor);

            string text = item.Target.name.Replace("---", string.Empty).Trim();
            if (!string.IsNullOrEmpty(text))
            {
                EditorGUI.LabelField(item.FullRect, text, separatorStyle);
            }
        }

        private static void DrawBackgroundColor(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            var alpha = item.VisiualIndex % 2 == 0 ? settings.backgroundColorAlphaEven : settings.backgroundColorAlphaOdd;
            if (item.Target.TryGetBackgroundColor(out var color) == false ||
                color == Color.clear)
            {
                if (settings.depthColor == false)
                {
                    color = settings.GetDepthColor(0);
                }
                else
                {
                    color = settings.GetDepthColor(item.Depth);
                }
            }

            color.a = alpha;
            EditorGUI.DrawRect(item.FullRect, color);
        }

        private static void DrawLine(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            var halfLineWidth = settings.lineWidth / 2f;

            // 세로선
            for (int i = item.Depth; i >= 0; --i)
            {
                // note: 시작 지점 + 중간 위치 + 들여쓰기 위치 - 라인 두께 보정
                var x = item.FullRect.x +
                    halfIndentWidth +
                    (i * indentWidth) -
                    halfLineWidth;

                var height = item.FullRect.height;
                if (i == item.Depth &&
                    item.ChildCount <= 0 &&
                    item.IsLastInParent())
                {
                    height = (height / 2) + settings.lineWidth;
                }

                var verticalLineRect = new Rect(
                    x,
                    y: item.FullRect.y,
                    width: settings.lineWidth,
                    height);

                EditorGUI.DrawRect(verticalLineRect, settings.GetDepthColor(i));
            }

            // 가로선
            var width = item.ChildCount <= 0 ? indentWidth : halfIndentWidth;
            var horizontalLineRect = new Rect(
                x: item.SelectionRect.x - (indentWidth + halfIndentWidth),
                y: item.FullRect.y + (item.FullRect.height / 2),
                width: width,
                height: settings.lineWidth);

            EditorGUI.DrawRect(horizontalLineRect, settings.GetDepthColor(item.Depth));
        }

        private static void DrawNote(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            // 노트 텍스트 가져오기
            var note = item.Target.GetNote();
            if (string.IsNullOrEmpty(note))
            {
                return;
            }

            var tooltipContent = new GUIContent(string.Empty, note);
            EditorGUI.LabelField(item.SelectionRect, tooltipContent);
            return;
        }

        private static void DrawGizmoIcon(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            var icon = item.Target.GetIcon();
            if (icon == null)
            {
                return;
            }

            var iconRect = new Rect(
                x: item.SelectionRect.x,
                y: item.SelectionRect.y,
                width: iconSize,
                height: iconSize
            );

            using (new ColorScope(Color.clear))
            {
                EditorGUI.DrawTextureTransparent(
                    iconRect,
                    icon);
            }
        }

        private static IEnumerable<Texture2D> GatherComponentsIcons(
            GameObjectContainer.Item item,
            HierarchySettings settings)
        {
            HashSet<Hash128> duplicateChecker = new();
            // note: 첫번째 컴포넌트는 (rect)Transform이므로 건너뜁니다.
            foreach (var comp in item.Target.GetComponents<Component>().Skip(1))
            {
                if (comp == null)
                {
                    if (missingComponentIcon == null)
                    {
                        missingComponentIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
                    }

                    if (duplicateChecker.Add(missingComponentIcon.imageContentsHash) == false)
                    {
                        continue;
                    }

                    yield return missingComponentIcon;
                    continue;
                }

                var overrideIcon = settings.overrideIcons.FirstOrDefault(e => e.Target == comp.GetType());
                if (overrideIcon != null)
                {
                    if (overrideIcon.Icon == null)
                    {
                        continue;
                    }

                    
                    if (duplicateChecker.Add(overrideIcon.Icon.imageContentsHash) == false)
                    {
                        continue;
                    }

                    yield return overrideIcon.Icon;
                    continue;
                }

                var icon = EditorGUIUtility.ObjectContent(comp, comp.GetType()).image as Texture2D;
                if (icon == null)
                {
                    continue;
                }

                if (duplicateChecker.Add(icon.imageContentsHash) == false)
                {
                    continue;
                }

                yield return icon;
            }
        }

        private static void DrawRightIcons(
            GameObjectContainer.Item item,
            IReadOnlyList<Texture2D> icons)
        {
            if (icons.Count <= 0)
            {
                return;
            }

            // note: prefab open icon 과 겹치지 않도록 icon.Count + 1
            var x = item.SelectionRect.x + item.SelectionRect.width - (iconSize * (icons.Count + 1));
            using (new ColorScope(Color.clear))
            {
                foreach (var icon in icons)
                {
                    x += iconSize;

                    var iconRect = new Rect(
                        x: x,
                        y: item.SelectionRect.y,
                        width: iconSize,
                        height: iconSize
                    );

                    EditorGUI.DrawTextureTransparent(iconRect, icon);
                }
            }
        }
    }
}