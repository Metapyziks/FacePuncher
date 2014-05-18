using System;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Widget used for getting user text input.
    /// </summary>
    class TextBox : UsableWidget
    {
        /// <summary>
        /// Current value.
        /// </summary>
        public string Text { get; set; }

        private bool _isEdited;

        /// <summary>
        /// Creates empty text box.
        /// </summary>
        /// <param name="name">Name of the widget</param>
        /// <param name="pos">Position of the text box</param>
        /// <param name="length">Maximum width</param>
        /// <param name="text">Value of textbox (default: empty).</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public TextBox(string name, Position pos, int length, string text = "",
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, length, 1, fc, bc)
        {
            this.Text = text;
            this._isEdited = false;

            this.Use = () =>
            {
                _isEdited = true;
                UIManager.IsInputBlocked = true;
                Text = "";
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

            if (_isEdited && Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);

                // ASCII code of pressed key
                int asciiCode = (int)info.KeyChar;

                // If pressed key is enter - stop editing
                if (info.Key == ConsoleKey.Enter)
                {
                    _isEdited = false;
                    UIManager.IsInputBlocked = false;
                }
                // If pressed key is backspace - remove last char
                else if (info.Key == ConsoleKey.Backspace)
                {
                    Text = Text.Remove(Text.Length - 1);
                }
                // If pressed key is a printable char
                // add this char to text
                else if (asciiCode >= 32 && asciiCode <= 126)
                {
                    Text += info.KeyChar.ToString();
                }
            }

            UIManager.DrawString(Position, "[" + Text + "]", fc, bc);
        }
    }
}
