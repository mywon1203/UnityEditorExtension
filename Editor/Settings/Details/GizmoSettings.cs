namespace KGGEEP
{
    using System;

    using UnityEditor;

    using UnityEngine;

    [Serializable]
    public sealed class GizmoSettings : CommonSettings
    {
        public bool useHierarchyColorForOutline = true;
        public float iconSize = 64;
        public int noteTextSize = 12;
        public Color noteTextColor = Color.white;
        public Color noteBackgroundColor = Color.black;
        public int notePaddingLeft = 2;
        public int notePaddingRight = 2;
        public int notePaddingTop = 1;
        public int notePaddingBottom = 1;

        public override EditorGUI.DisabledScope OnGui()
        {
            var scope = base.OnGui();
            EditorGUILayout.Space();

            this.useHierarchyColorForOutline = EditorGUILayout.ToggleLeft(
                "Use Hierarchy Color For Outline",
                this.useHierarchyColorForOutline);

            this.iconSize = EditorGUILayout.Slider("Icon Size", this.iconSize, leftValue: 16, rightValue: 512);
            this.noteTextSize = EditorGUILayout.IntField("Note Text Size", this.noteTextSize);
            this.noteTextColor = EditorGUILayout.ColorField("Note Text Color", this.noteTextColor);
            this.noteBackgroundColor = EditorGUILayout.ColorField("Note Background Color", this.noteBackgroundColor);

            EditorGUILayout.LabelField("Note Padding", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Left");
                    EditorGUILayout.LabelField("Right");
                    EditorGUILayout.LabelField("Top");
                    EditorGUILayout.LabelField("Bottom");
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    this.notePaddingLeft = EditorGUILayout.IntField(string.Empty, this.notePaddingLeft);
                    this.notePaddingRight = EditorGUILayout.IntField(string.Empty, this.notePaddingRight);
                    this.notePaddingTop = EditorGUILayout.IntField(string.Empty, this.notePaddingTop);
                    this.notePaddingBottom = EditorGUILayout.IntField(string.Empty, this.notePaddingBottom);
                }
            }
            
            return scope;
        }
    }
}