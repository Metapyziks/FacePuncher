using System;

namespace FacePuncher.Entities
{
    public enum DrawableLayer
    {
        Debris = 0,
        Items = 1,
        Characters = 2
    }

    public abstract class Drawable : Component
    {
        public virtual DrawableLayer Layer { get { return DrawableLayer.Items; } set { } }

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
