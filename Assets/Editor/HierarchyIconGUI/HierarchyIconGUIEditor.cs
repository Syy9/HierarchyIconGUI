using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools.HierarchyIconGUI
{
    public class HierarchyIconGUIEditor : EditorWindow
    {
        [MenuItem("Window/HierarchyIconGUI")]
        public static void Open()
        {
            GetWindow<HierarchyIconGUIEditor>("HierarchyIconGUI");
        }

        List<IGUIHandler> _guiHandlers;
        Vector2 _scrollPos;

        void OnEnable()
        {
            _guiHandlers = new GUIHandlerProvider().CreateGUIHandler();
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Hierarchy Icon GUIHandlers");
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _scrollPos = scroll.scrollPosition;
                foreach (var guiHandler in _guiHandlers)
                {
                    guiHandler.Enable = EditorGUILayout.ToggleLeft(guiHandler.Title, guiHandler.Enable);
                }
                if (check.changed)
                {
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var target = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (target == null)
            {
                return;
            }

            var components = target.GetComponents<Component>();
            var currentX = 0f;
            foreach (var guiHandler in _guiHandlers)
            {
                if (guiHandler.Enable && guiHandler.Validate(target, components))
                {
                    currentX += guiHandler.OnGUI(target, components, selectionRect, currentX);
                    currentX += 1; //space
                }
            }
        }
    }

    public interface IGUIHandler
    {
        bool Enable { get; set; }
        string Title { get; set; }
        bool Validate(GameObject target, Component[] components);
        /// <summary>
        /// Draw Custom GUI. Return after GUI position x;
        /// </summary>
        float OnGUI(GameObject target, Component[] components, Rect selectionRect, float currentX);
    }

    public class SimpleGUIHandler : IGUIHandler
    {
        Type _type;
        Texture _icon;
        public bool Enable { get; set; } = true;
        public string Title { get; set; }

        static readonly Vector2 IconSize = new Vector2(16, 16);

        public SimpleGUIHandler(Type type, string iconPath) : this(type, EditorGUIUtility.Load(iconPath) as Texture)
        {
        }

        public SimpleGUIHandler(Type type, Texture icon)
        {
            Title = type.ToString();
            _type = type;
            _icon = icon;
        }

        public bool Validate(GameObject target, Component[] components)
        {
            return components.Any(value => value.GetType() == _type);
        }

        public float OnGUI(GameObject target, Component[] components, Rect selectionRect, float currentX)
        {
            selectionRect.x = selectionRect.width - currentX - IconSize.x;
            selectionRect.width = IconSize.x;
            selectionRect.height = IconSize.y;
            GUI.DrawTexture(selectionRect, _icon, ScaleMode.ScaleToFit);
            return IconSize.x;
        }
    }

    public abstract class GUIHandlerProviderBase
    {
        public virtual List<IGUIHandler> CreateGUIHandler()
        {
            return new List<IGUIHandler>();
        }
    }

    public partial class GUIHandlerProvider : GUIHandlerProviderBase
    {
        // Please create own ValidateProvider with partial keyward. And override function of CreateValidater.
#if false
            public override List<IGUIHandler> CreateGUIHandler()
            {
                var list = new List<IGUIHandler>();
                list.Add(new SimpleGUIHandler(typeof(BoxCollider), "HierarchyIconGUI/HierarchyIconGUI_Red.png"));
                list.Add(new SimpleGUIHandler(typeof(Light), "HierarchyIconGUI/HierarchyIconGUI_Blue.png"));
                list.Add(new SimpleGUIHandler(typeof(Camera), "HierarchyIconGUI/HierarchyIconGUI_Green.png"));
                return list;
            }
#endif
    }
}
