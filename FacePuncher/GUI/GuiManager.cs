using System;
using System.Collections.Generic;

namespace FacePuncher.GUI
{
    class GuiManager : IWidgetContainer
    {
        public GuiManager()
        {
            Children = new Dictionary<string, Widget>();
        }

        public Dictionary<string, Widget> Children
        { get; set; }

        public void AddChild(string name, Widget w)
        {
            Children.Add(name, w);
        }

        public void Draw()
        {
            DrawChildren();
        }

        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }
    }
}
