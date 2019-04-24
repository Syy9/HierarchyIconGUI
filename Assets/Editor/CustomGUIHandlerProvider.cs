using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syy.Tools.HierarchyIconGUI
{
    public partial class GUIHandlerProvider : GUIHandlerProviderBase
    {
        public override List<IGUIHandler> CreateGUIHandler()
        {
            var list = new List<IGUIHandler>();
            list.Add(new SimpleGUIHandler(typeof(BoxCollider), "HierarchyIconGUI/HierarchyIconGUI_Red.png"));
            list.Add(new SimpleGUIHandler(typeof(Light), "HierarchyIconGUI/HierarchyIconGUI_Blue.png"));
            list.Add(new SimpleGUIHandler(typeof(Camera), "HierarchyIconGUI/HierarchyIconGUI_Green.png"));
            return list;
        }
    }
}
