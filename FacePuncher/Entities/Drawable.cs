using System;

using FacePuncher.Graphics;

namespace FacePuncher.Entities
{
    public abstract class Drawable : Component
    {
        public virtual char GetSymbol(DrawAttributes attribs)
        {
            return '?';
        }

        public virtual ConsoleColor GetForeColor(DrawAttributes attribs)
        {
            return ConsoleColor.Gray;
        }

        public virtual ConsoleColor GetBackColor(DrawAttributes attribs)
        {
            return ConsoleColor.Black;
        }
    }
}
