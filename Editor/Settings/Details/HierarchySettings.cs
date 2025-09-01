namespace KGGEEP
{
    using System;
    using System.Collections.Generic;

    using UnityEditor;

    using UnityEngine;

    [Serializable]
    public sealed class HierarchySettings : CommonSettings
    {
        public enum NoteIconOrder
        {
            Left,
            Right,
        }

        public bool noteIconVisiable = true;
        public NoteIconOrder noteIconOrder = NoteIconOrder.Left;
        public bool gizmoIconVisiable = true;
        public bool componentIconsVisiable = true;

        public bool enableBackgroundColor = true;
        public float backgroundColorAlphaOdd = 0.05f;
        public float backgroundColorAlphaEven = 0.1f;

        public bool enableLine = true;
        public float lineWidth = 2f;
        public bool depthColor = true;
        public List<Color> depthColors = new() { Color.white };

        public bool enableSeparator = true;
        public string seperatorPrefix = "---";
        public Color separatorBackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        public Color seperatorTextColor = new Color(0f, 0f, 0f, 1f);

        public List<OverrideIconRecord> overrideIcons = new();

        public Color GetDepthColor(int depth)
        {
            if (this.depthColor == false || depth < 0)
            {
                return this.depthColors[0];
            }

            if (depth < this.depthColors.Count)
            {
                return this.depthColors[depth];
            }

            while (this.depthColors.Count <= depth)
            {
                var newColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, a: 1f);
                this.depthColors.Add(newColor);
            }

            return this.depthColors[depth];
        }

        public override EditorGUI.DisabledScope OnGui()
        {
            var scope = base.OnGui();
            EditorGUILayout.Space();

            this.noteIconVisiable = EditorGUILayout.ToggleLeft(
                "Show Note Icon",
                this.noteIconVisiable);

            using (new IndentDisabledScope(!this.noteIconVisiable))
            {
                this.noteIconOrder = (NoteIconOrder)EditorGUILayout.EnumPopup(
                    "Note Icon Order",
                    this.noteIconOrder);
            }

            this.gizmoIconVisiable = EditorGUILayout.ToggleLeft(
                "Show Gizmo Icon",
                this.gizmoIconVisiable);

            this.componentIconsVisiable = EditorGUILayout.ToggleLeft(
                "Show Component Icons",
                this.componentIconsVisiable);

            this.enableLine = EditorGUILayout.ToggleLeft(
                "Hierarchy Line",
                this.enableLine);

            using (new IndentDisabledScope(!this.enableLine))
            {
                this.lineWidth = EditorGUILayout.Slider(
                    "Line Width",
                    this.lineWidth,
                    leftValue: 1f,
                    rightValue: 5f);
            }

            this.enableBackgroundColor = EditorGUILayout.ToggleLeft(
                "Background Color",
                this.enableBackgroundColor);

            using (new IndentDisabledScope(!this.enableBackgroundColor))
            {
                this.backgroundColorAlphaOdd = EditorGUILayout.Slider(
                    "Odd Row Alpha",
                    this.backgroundColorAlphaOdd,
                    leftValue: 0f,
                    rightValue: 1f);
                this.backgroundColorAlphaEven = EditorGUILayout.Slider(
                    "Even Row Alpha",
                    this.backgroundColorAlphaEven,
                    leftValue: 0f,
                    rightValue: 1f);
            }

            this.depthColor = EditorGUILayout.ToggleLeft(
                    "Hierarchy Depth Color",
                    this.depthColor);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
                if (this.depthColor == false)
                {
                    this.depthColors[0] = EditorGUILayout.ColorField(
                        "Color",
                        this.depthColors[0]);
                }
                else
                {
                    int removeColorIndex = -1;
                    for (int i = 0; i < this.depthColors.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        this.depthColors[i] = EditorGUILayout.ColorField($"Depth {i}", this.depthColors[i]);

                        if (i > 0)
                        {
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                removeColorIndex = i;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (removeColorIndex >= 0)
                    {
                        this.depthColors.RemoveAt(removeColorIndex);
                    }

                    if (GUILayout.Button("Add Color"))
                    {
                        this.depthColors.Add(Color.clear);
                    }
                }
            }

            this.enableSeparator = EditorGUILayout.ToggleLeft(
                "Seperator",
                this.enableSeparator);

            using (new IndentDisabledScope(!this.enableSeparator))
            {
                this.seperatorPrefix = EditorGUILayout.TextField(
                    "Name Prefix",
                    this.seperatorPrefix);

                this.separatorBackgroundColor = EditorGUILayout.ColorField(
                    "Background Color",
                    this.separatorBackgroundColor);

                this.seperatorTextColor = EditorGUILayout.ColorField(
                    "Text Color",
                    this.seperatorTextColor);
            }

            EditorGUILayout.LabelField("Override Icons", EditorStyles.boldLabel);
            int removeOverrideIconIndex = -1;
            for (int i = 0; i < this.overrideIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                this.overrideIcons[i].Target = TypeField.Draw("Target", this.overrideIcons[i].Target, typeof(Component));
                this.overrideIcons[i].Icon = (Texture2D)EditorGUILayout.ObjectField("Icon", this.overrideIcons[i].Icon, typeof(Texture2D), allowSceneObjects: false);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    removeOverrideIconIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeOverrideIconIndex >= 0)
            {
                this.overrideIcons.RemoveAt(removeOverrideIconIndex);
            }

            if (GUILayout.Button("Add"))
            {
                this.overrideIcons.Add(new OverrideIconRecord());
            }

            return scope;
        }

        [Serializable]
        public sealed class OverrideIconRecord
        {
            public TypeSerializableReference Target;
            public Texture2D Icon;
        }
    }
}