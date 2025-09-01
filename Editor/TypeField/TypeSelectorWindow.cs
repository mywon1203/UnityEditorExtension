namespace KGGEEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.U2D;
    using UnityEngine.VFX;

    public class TypeSelectorWindow : EditorWindow
    {
        private const string SearchFieldName = "KGGEEP_TypeSelectorWindowSearchField";
        private static readonly Vector2 WindowRect = new Vector2(320, 400);
        private static readonly Color SelectionColor = new Color(0.3f, 0.5f, 1f, 0.2f);
        private const int scrollStartX = 2;
        private const int rowHeight = 20;
        private const int groupHeaderHeight = 24; // = search field hight
        private const int groupRowHeight = rowHeight + groupHeaderHeight;

        private static TypeSelectorWindow currentWindow;

        private Type baseType;
        private string searchText = string.Empty;
        private readonly List<Item> items = new();
        private Action<Type> onSelected;

        private Vector2 scrollPos;
        private int selectedIndex = 0;

        public static void Show(Type baseType, Action<Type> onSelected, Vector2 position)
        {
            if (currentWindow != null)
            {
                currentWindow.Close();
            }

            currentWindow = CreateInstance<TypeSelectorWindow>();
            currentWindow.Initialize(baseType, onSelected);
            currentWindow.ShowAsDropDown(new Rect(position, Vector2.zero), WindowRect);
        }

        private void Initialize(Type baseType, Action<Type> onSelected)
        {
            this.baseType = baseType;
            this.onSelected = onSelected;

            this.RefreshTypes();
        }

        private void OnGUI()
        {
            if (GUI.GetNameOfFocusedControl() != SearchFieldName)
            {
                GUI.FocusControl(SearchFieldName);
            }

            this.HandleKeyboard();
            this.DrawSearchField();
            this.DrawTypeList();
        }

        private void DrawSearchField()
        {
            EditorGUI.BeginChangeCheck();

            GUI.SetNextControlName(SearchFieldName);
            this.searchText = EditorGUILayout.TextField(this.searchText, EditorStyles.toolbarSearchField);

            if (EditorGUI.EndChangeCheck())
            {
                this.RefreshTypes();
            }
        }

        private void DrawTypeList()
        {
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);

            for (int i = 0; i < this.items.Count; i++)
            {
                var item = this.items[i];
                if (item.GroupStart)
                {
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField(item.GroupName, EditorStyles.boldLabel);
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    if (this.DrawTypeItem(item, i))
                    {
                        this.onSelected?.Invoke(item.Type);
                        this.Close();
                        break;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private bool DrawTypeItem(Item current, int currentIndex)
        {
            var rect = EditorGUILayout.GetControlRect();

            // 현재 아이템이 선택된 항목인 경우
            if (this.selectedIndex == currentIndex || rect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(rect, SelectionColor);
            }

            var guiContent = new GUIContent(current.Name, TypeIconMap.Get(current.Type));
            return GUI.Button(rect, guiContent, EditorStyles.label);
        }

        private void HandleKeyboard()
        {
            if (Event.current.type != EventType.KeyDown)
            {
                return;
            }

            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    this.Close();
                    Event.current.Use();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (this.items.Count > 0)
                    {
                        this.onSelected?.Invoke(this.items[this.selectedIndex].Type);
                        this.Close();
                        Event.current.Use();
                    }
                
                    break;
                case KeyCode.DownArrow:
                    {
                        this.selectedIndex = Mathf.Min(this.selectedIndex + 1, this.items.Count - 1);
                        var rectHeight = this.items.Take(this.selectedIndex + 1)
                            .Sum(e => e.GroupStart ? groupRowHeight : rowHeight) + groupRowHeight + scrollStartX;
                        var scrollEnd = this.scrollPos.y + this.position.height;
                        if (rectHeight > scrollEnd)
                        {
                            this.scrollPos.y = rectHeight - this.position.height;
                        }

                        Event.current.Use();
                    }
                    
                    break;
                case KeyCode.UpArrow:
                    {
                        this.selectedIndex = Mathf.Max(this.selectedIndex - 1, 0);
                        var rectHeight = this.items.Take(this.selectedIndex)
                            .Sum(e => e.GroupStart ? groupRowHeight : rowHeight) + scrollStartX;
                        if (rectHeight < this.scrollPos.y)
                        {
                            this.scrollPos.y = rectHeight;
                        }

                        Event.current.Use();
                    }
                    
                    break;
            }
        }

        private void RefreshTypes()
        {
            List<Type> filteredTypes = new();
            foreach (var type in TypeCache.GetTypesDerivedFrom(this.baseType))
            {
                if (type.IsAbstract && type.IsInterface && type.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (type.CustomAttributes.Any(e => e.AttributeType == typeof(ObsoleteAttribute)))
                {
                    continue;
                }

                // component specific
                var ignoreComponentTypes = new HashSet<Type>
                    {
                        typeof(Behaviour),
                        typeof(MonoBehaviour),
                        typeof(AudioBehaviour),
                        typeof(PhysicsUpdateBehaviour2D),
                        typeof(Transform),
                        typeof(Rigidbody),
                        typeof(AnchoredJoint2D),
                        typeof(Renderer),
                        typeof(ParticleSystemRenderer),
                        typeof(VFXRenderer),
                        typeof(Effector2D),
                        typeof(Joint),
                        typeof(Joint2D),
                        typeof(Collider),
                        typeof(Collider2D),
                        typeof(GridLayout),
                        typeof(Light2DBase),
                    };

                if (ignoreComponentTypes.Contains(type))
                {
                    continue;
                }

                if (type.Namespace?.StartsWith("UnityEngine.TestTools") == true)
                {
                    continue;
                }

                if (string.Compare(type.FullName, "UnityEngine.MultiplayerRolesData") == 0)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(this.searchText))
                {
                    filteredTypes.Add(type);
                }
                else if (MatchesSearch(type, this.searchText))
                {
                    filteredTypes.Add(type);
                }
            }

            var groups = filteredTypes.GroupBy(t => t.Namespace ?? "Global").OrderBy(g => g.Key);
            this.items.Clear();
            this.items.Add(new Item(null, groupStart: false));
            foreach (var group in groups)
            {
                bool groupStart = true;
                foreach (var type in group.OrderBy(t => t.Name))
                {
                    this.items.Add(new Item(type, groupStart));
                    if (groupStart)
                    {
                        groupStart = false;
                    }
                }
            }

            static bool MatchesSearch(Type type, string search)
            {
                return type.Name.ToLower().Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                       (type.FullName?.ToLower().Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                       (type.Namespace?.ToLower().Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false);
            }
        }

        private void OnDestroy()
        {
            currentWindow = null;
        }

        private sealed class Item
        {
            public Item(Type type, bool groupStart)
            {
                this.Type = type;
                this.GroupStart = groupStart;
            }

            public Type Type { get; }
            public bool GroupStart { get; }

            public string Name => this.Type?.Name ?? "None";
            public string GroupName => this.Type?.Namespace ?? "Global";
        }
    }
}