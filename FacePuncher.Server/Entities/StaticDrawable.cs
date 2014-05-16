using System;

using FacePuncher.Graphics;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Component for entities that should be drawn using one specific
    /// symbol, foreground color and background color.
    /// </summary>
    class StaticDrawable : Drawable
    {
        private EntityAppearance _appearance;

        /// <summary>
        /// Gets or sets the layer the owning entity should be drawn on.
        /// </summary>
        [ScriptDefinable]
        public DrawableLayer Layer { get; set; }

        /// <summary>
        /// Gets or sets the character representing this entity.
        /// </summary>
        [ScriptDefinable]
        public char Symbol
        {
            get { return _appearance[0].Symbol; }
            set
            {
                _appearance[0] = new EntityAppearance.Frame(value, ForeColor, BackColor, 1);
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of this entity.
        /// </summary>
        [ScriptDefinable]
        public ConsoleColor ForeColor
        {
            get { return _appearance[0].ForeColor; }
            set
            {
                _appearance[0] = new EntityAppearance.Frame(Symbol, value, BackColor, 1);
            }
        }

        /// <summary>
        /// Gets or sets the background color of this entity.
        /// </summary>
        [ScriptDefinable]
        public ConsoleColor BackColor
        {
            get { return _appearance[0].BackColor; }
            set
            {
                _appearance[0] = new EntityAppearance.Frame(Symbol, ForeColor, value, 1);
            }
        }

        /// <summary>
        /// Constructs a new StaticDrawable component.
        /// </summary>
        public StaticDrawable()
        {
            Layer = DrawableLayer.Items;

            _appearance = new EntityAppearance { '?' };
        }

        /// <summary>
        /// Gets the layer the owning entity should be drawn on.
        /// </summary>
        public override DrawableLayer GetLayer()
        {
            return Layer;
        }

        /// <summary>
        /// Gets appearance information for the entity.
        /// </summary>
        /// <returns>Appearance information for the entity.</returns>
        public override EntityAppearance GetAppearance()
        {
            return _appearance;
        }
    }
}
