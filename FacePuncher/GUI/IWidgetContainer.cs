using System;
using System.Collections.Generic;

namespace FacePuncher.GUI
{
    interface IWidgetContainer
    {
        Dictionary<string, Widget> Children { get; set; }

        void AddChild(string name, Widget w);
        void DrawChildren();
    }
}
