using System;
using System.Collections.Generic;

namespace FacePuncher.GUI
{
    class GuiManager
    {
        public Dictionary<string, Widget> Widgets { get; set; }

        public GuiManager()
        {
            Widgets = new Dictionary<string, Widget>();
        }

        public void AddWidget(string name, Widget w)
        {
            Widgets.Add(name, w);
        }

        public void Draw()
        {
            foreach (var widget in Widgets)
            {
                widget.Value.Draw();
            }
        }
    }
}
