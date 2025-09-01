#nullable enable
namespace KGGEEP
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal sealed class GameObjectContainer
    {
        private readonly Dictionary<int, Item> items;
        private GameObjectContainer() 
        {
            this.items = new();
        }

        public static GameObjectContainer instance { get; } = new GameObjectContainer();

        public static void OnHierarchyChanged()
        {
            instance.items.Clear();
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            var visualIndex = 0;
            foreach (var rootObject in rootObjects)
            {
                instance.Add(rootObject, ref visualIndex, depth: 0);
            }
        }

        public static bool TryGetValue(
            int instanceId,
            [MaybeNullWhen(returnValue: false)] out Item item)
        {
            return instance.items.TryGetValue(instanceId, out item);
        }

        // ---------------------------------------------------------------------------------
        private void Add(GameObject self, ref int visualIndex, int depth)
        {
            var instanceId = self.GetInstanceID();
            if (this.items.TryGetValue(instanceId, out Item item) == false)
            {
                item = new Item(self);
                this.items.Add(instanceId, item);
            }

            item.RebuildCache(visualIndex++, depth++);
            for (int i = 0; i < self.transform.childCount; i++)
            {
                var child = self.transform.GetChild(i);
                this.Add(child.gameObject, ref visualIndex, depth);
            }
        }

        internal sealed class Item
        {
            private const float HierarchyStartX = 32f;
            public Item(GameObject self)
            {
                this.Target = self;
            }

            public GameObject Target { get; }
            public Rect SelectionRect { get; private set; }
            public Rect FullRect { get; private set; }
            public int VisiualIndex { get; private set; }
            public int Depth { get; private set; }

            public bool IsSeperator => this.Target.CompareTag(Constants.EditorOnlyTag) &&
                this.Target.name.StartsWith(ProjectSettings.instance.hierarchy.seperatorPrefix);
            public int MissingComponentCount => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(this.Target);
            public bool HasMissingComponent => this.MissingComponentCount > 0;
            public int ChildCount => this.Target.transform.childCount;

            public void UpdateCache(Rect lastRect)
            {
                this.SelectionRect = lastRect;
                this.FullRect = new Rect(
                    x: HierarchyStartX,
                    y: lastRect.y,
                    width: lastRect.width + lastRect.x,
                    height: lastRect.height
                    );
            }

            public void RebuildCache(int visualIndex, int depth)
            {
                this.VisiualIndex = visualIndex;
                this.Depth = depth;
            }

            public bool IsLastInParent()
            {
                var parent = this.Target.transform.parent;
                if (parent == null)
                {
                    var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                    return rootObjects.LastOrDefault() == this.Target;
                }

                return parent.childCount - 1 == this.Target.transform.GetSiblingIndex();
            }
        }
    }
}
