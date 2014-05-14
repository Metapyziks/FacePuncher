using System;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Component for entities that should be drawn using one specific
    /// symbol, foreground color and background color.
    /// </summary>
    class StaticDrawable : Drawable
    {
        private DrawableLayer _layer;

        /// <summary>
        /// Gets or sets the layer the owning entity should be drawn on.
        /// </summary>
        [ScriptDefinable]
        public override DrawableLayer Layer { get { return _layer; } set { _layer = value; } }

        /// <summary>
        /// Gets or sets the character representing this entity.
        /// </summary>
        [ScriptDefinable]
        public char Symbol { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of this entity.
        /// </summary>
        [ScriptDefinable]
        public ConsoleColor ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of this entity.
        /// </summary>
        [ScriptDefinable]
        public ConsoleColor BackColor { get; set; }

        /// <summary>
        /// Constructs a new StaticDrawable component.
        /// </summary>
        public StaticDrawable()
        {
            Layer = DrawableLayer.Items;
            Symbol = '?';
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Gets the symbol used to represent this entity.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>A character to be drawn to represent this entity.</returns>
        public override char GetSymbol(DrawAttributes attribs)
        {
            return Symbol;
        }

        /// <summary>
        /// Gets the color to be used when drawing this entity's symbol.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>The color of the symbol to be drawn.</returns>
        public override ConsoleColor GetForeColor(DrawAttributes attribs)
        {
            return ForeColor;
        }

        /// <summary>
        /// Gets the color of the background to be used when drawing this entity.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>The background colour of the entity.</returns>
        public override ConsoleColor GetBackColor(DrawAttributes attribs)
        {
            return BackColor;
        }
    }
}
