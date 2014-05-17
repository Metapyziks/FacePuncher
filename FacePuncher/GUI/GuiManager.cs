using System;
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class GuiManager : IWidgetContainer
    {
        private List<Widget> _selectableWidgets;
        private int _selectedId;

        public GuiManager()
        {
            Children = new Dictionary<string, Widget>();
            _selectableWidgets = new List<Widget>();
            _selectedId = 0;
        }

        public Dictionary<string, Widget> Children
        { get; set; }

        public void AddChild(string name, Widget w)
        {
            Children.Add(name, w);
        }

        public void Draw()
        {
            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;
            
            _selectableWidgets[_selectedId].IsSelected = false;

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.W)
                {
                    _selectedId--;
                }
                else if (info.Key == ConsoleKey.S)
                {
                    _selectedId++;
                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    _selectableWidgets[_selectedId].Use();
                }
            }

            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;

            _selectableWidgets[_selectedId].IsSelected = true;

            DrawChildren();
        }

        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }

        public static void DrawString(Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            for (var x = 0; x < text.Length; x++)
            {
                Display.SetCell(pos.X + x, pos.Y, text[x], fc, bc);
            }
        }

        public int CalculateSelectableWidgets()
        {
            _selectableWidgets = new List<Widget>();

            foreach (var widget in Children)
            {
                _selectableWidgets.AddRange(widget.Value.GetSelectableWidgets());
            }

            _selectedId = 0;
            _selectableWidgets[_selectedId].IsSelected = true;

            return _selectableWidgets.Count;
        }
    }
}
