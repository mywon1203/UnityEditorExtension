namespace KGGEEP
{
    using System;

    using UnityEditor;

    [Serializable]
    public class CommonSettings
    {
        public bool enable = true;

        public bool noteVisiable = true;
        
        public virtual EditorGUI.DisabledScope OnGui()
        {
            this.enable = EditorGUILayout.BeginToggleGroup(
                Constants.EnableString,
                this.enable);

            // note: 외부에서 스코프를 컨트롤 할 수 있도록 using을 사용하지 않습니다.
            var scope = new EditorGUI.DisabledScope(!this.enable);

            this.noteVisiable = EditorGUILayout.ToggleLeft(
                Constants.ShowNoteString,
                this.noteVisiable);

            EditorGUILayout.EndToggleGroup();
            return scope;
        }
    }
}