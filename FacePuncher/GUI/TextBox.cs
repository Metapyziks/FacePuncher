using FacePuncher.Geometry;
using System;

namespace FacePuncher.GUI
{
    class TextBox : Widget
    {
        public string Text { get; set; }

        public bool _isEdited;

        public TextBox(string name, Position pos, int length, string text = "",
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, length, 1, true, fc, bc)
        {
            this.Text = text;
            this._isEdited = false;

            this.Use = () =>
            {
                _isEdited = true;
                Text = "";

                while (_isEdited)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo info = Console.ReadKey(true);

                        int asciiCode = (int)info.KeyChar;

                        if (info.Key == ConsoleKey.Enter)
                            _isEdited = false;
                        else if (info.Key == ConsoleKey.Backspace)
                            Text = Text.Remove(Text.Length - 1);
                        else if (asciiCode >= 32 && asciiCode <= 126)
                            Text += info.KeyChar.ToString();
                    }
                }
            };
        }

        public override void Draw()
        {
            ConsoleColor fc = ForegroundColor;
            ConsoleColor bc = BackgroundColor;

            // If widget is selected
            // swap colors
            if (IsSelectable && IsSelected && !_isEdited)
            {
                fc = BackgroundColor;
                bc = ForegroundColor;
            }

            GuiManager.DrawString(Position, "[" + Text + "]", fc, bc);
        }
    }
}
