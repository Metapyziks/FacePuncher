using System;
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Root node for all widgets.
    /// </summary>
    class UIManager : IWidgetContainer
    {
        private List<Widget> _selectableWidgets;
        private int _selectedId;

        /// <summary>
        /// Creates empty manager.
        /// </summary>
        public UIManager()
        {
            Children = new Dictionary<string, Widget>();
            _selectableWidgets = new List<Widget>();
            _selectedId = 0;
        }

        public Dictionary<string, Widget> Children
        { get; set; }

        /// <summary>
        /// Function used to render widgets and check for UI input.
        /// </summary>
        public void Draw()
        {
            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;

            // Deselect currently selected item
            if (_selectableWidgets.Count > 0)
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
                    if (_selectableWidgets[_selectedId] is UsableWidget)
                    {
                        ((UsableWidget)_selectableWidgets[_selectedId]).Use();
                    }
                }
            }

            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;

            if (_selectableWidgets.Count > 0)
                _selectableWidgets[_selectedId].IsSelected = true;

            DrawChildren();
        }

        /// <summary>
        /// Used to render widgets stored in manager.
        /// </summary>
        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }

        /// <summary>
        /// Renders text
        /// </summary>
        /// <param name="pos">Position of first character.</param>
        /// <param name="text">Text to render.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public static void DrawString(Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            for (var x = 0; x < text.Length; x++)
            {
                Display.SetCell(pos.X + x, pos.Y, text[x], fc, bc);
            }
        }

        /// <summary>
        /// Calculates information about widgets that can be selected.
        /// Call only once, after adding or removing widgets.
        /// </summary>
        /// <returns>Number of widgets that can be selected.</returns>
        public int CalculateSelectableWidgets()
        {
            _selectableWidgets = new List<Widget>();

            foreach (var widget in Children)
            {
                _selectableWidgets.AddRange(widget.Value.GetSelectableWidgets());
            }

            _selectedId = 0;

            if (_selectableWidgets.Count > 0)
                _selectableWidgets[_selectedId].IsSelected = true;

            return _selectableWidgets.Count;
        }

        public void AddChild(Widget w)
        {
            Children.Add(w.Name, w);
        }
    }
}
