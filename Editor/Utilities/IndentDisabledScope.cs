namespace KGGEEP
{
    using System;

    using UnityEditor;

    internal readonly struct IndentDisabledScope : IDisposable
    {
        private readonly EditorGUI.DisabledScope disabledScope;
        private readonly EditorGUI.IndentLevelScope indentLevelScope;

        public IndentDisabledScope(bool disabled)
        {
            this.disabledScope = new EditorGUI.DisabledScope(disabled);
            this.indentLevelScope = new EditorGUI.IndentLevelScope();
        }

        public void Dispose()
        {
            this.indentLevelScope.Dispose();
            this.disabledScope.Dispose();
        }
    }
}
