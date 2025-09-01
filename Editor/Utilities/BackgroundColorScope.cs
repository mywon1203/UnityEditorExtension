namespace KGGEEP
{
    using System;

    using UnityEngine;

    internal readonly struct BackgroundColorScope : IDisposable
    {
        private readonly Color previous;

        public BackgroundColorScope(Color color)
        {
            this.previous = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        public void Dispose()
        {
            GUI.backgroundColor = this.previous;
        }
    }
}