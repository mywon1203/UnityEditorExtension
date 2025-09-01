namespace KGGEEP
{
    using System;

    using UnityEditor;

    [Serializable]
    public sealed class InspectorSettings : CommonSettings
    {
        public enum DisplayMode
        {
            Always,
            Foldout,
        }

        public bool alwaysShowDebugInfo;
        public DisplayMode displayMode = DisplayMode.Always;

        public override EditorGUI.DisabledScope OnGui()
        {
            var scope = base.OnGui();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            this.alwaysShowDebugInfo = EditorGUILayout.Toggle(
                "Always Show Debug Info",
                this.alwaysShowDebugInfo);

            this.displayMode = (DisplayMode)EditorGUILayout.EnumPopup(
                "Display Mode",
                this.displayMode);
            return scope;
        }
    }
}