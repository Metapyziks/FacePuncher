using System;

namespace FacePuncher.Entities
{
    class StaticDrawable : Drawable
    {
        private DrawableLayer _layer;

        [ScriptDefinable]
        public override DrawableLayer Layer { get { return _layer; } set { _layer = value; } }

        [ScriptDefinable]
        public char Symbol { get; set; }

        [ScriptDefinable]
        public ConsoleColor ForeColor { get; set; }

        [ScriptDefinable]
        public ConsoleColor BackColor { get; set; }

        public StaticDrawable()
        {
            _layer = DrawableLayer.Items;
            Symbol = '?';
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        public StaticDrawable SetLayer(DrawableLayer layer)
        {
            _layer = layer; return this;
        }

        public override char GetSymbol(DrawAttributes attribs)
        {
            return Symbol;
        }

        public StaticDrawable SetSymbol(char symbol)
        {
            Symbol = symbol; return this;
        }

        public override ConsoleColor GetForeColor(DrawAttributes attribs)
        {
            return ForeColor;
        }

        public StaticDrawable SetForeColor(ConsoleColor color)
        {
            ForeColor = color; return this;
        }

        public override ConsoleColor GetBackColor(DrawAttributes attribs)
        {
            return BackColor;
        }

        public StaticDrawable SetBackColor(ConsoleColor color)
        {
            BackColor = color; return this;
        }
    }
}
