namespace KGGEEP
{
    using System;

    using UnityEngine;

    internal readonly struct ColorScope : IDisposable
    {
        private readonly Color previous;

        public ColorScope(Color color)
        {
            this.previous = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = this.previous;
        }
    }
}