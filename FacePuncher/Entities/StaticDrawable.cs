using System;

namespace FacePuncher.Entities
{
    class StaticDrawable : Drawable
    {
        private DrawableLayer _layer;

        public override DrawableLayer Layer { get { return _layer; } }

        public char Symbol { get; set; }

        public ConsoleColor ForeColor { get; set; }

        public ConsoleColor BackColor { get; set; }

        public StaticDrawable()
        {
            _layer = DrawableLayer.Items;
            Symbol = '?';
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        public override void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            if (elem.HasElement("Layer")) {
                _layer = elem.Element<DrawableLayer>("Layer");
            }

            if (elem.HasElement("Symbol")) {
                Symbol = elem.Element<char>("Symbol");
            }

            if (elem.HasElement("ForeColor")) {
                ForeColor = elem.Element<ConsoleColor>("ForeColor");
            }

            if (elem.HasElement("BackColor")) {
                BackColor = elem.Element<ConsoleColor>("BackColor");
            }
        }

        public StaticDrawable SetLayer(DrawableLayer layer)
        {
            _layer = layer; return this;
        }

        public override char GetSymbol(Graphics.DrawAttributes attribs)
        {
            return Symbol;
        }

        public StaticDrawable SetSymbol(char symbol)
        {
            Symbol = symbol; return this;
        }

        public override ConsoleColor GetForeColor(Graphics.DrawAttributes attribs)
        {
            return ForeColor;
        }

        public StaticDrawable SetForeColor(ConsoleColor color)
        {
            ForeColor = color; return this;
        }

        public override ConsoleColor GetBackColor(Graphics.DrawAttributes attribs)
        {
            return BackColor;
        }

        public StaticDrawable SetBackColor(ConsoleColor color)
        {
            BackColor = color; return this;
        }
    }
}
