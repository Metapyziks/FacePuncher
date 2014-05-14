using System;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Entity rendering categories, where lower layers
    /// are occluded by higher ones.
    /// </summary>
    public enum DrawableLayer
    {
        /// <summary>
        /// Lowest drawing layer, mainly for floor details.
        /// </summary>
        Debris = 0,

        /// <summary>
        /// Middle drawing layer, for items on the ground.
        /// </summary>
        Items = 1,

        /// <summary>
        /// Highest drawing layer, for players and NPCs.
        /// </summary>
        Characters = 2
    }

    /// <summary>
    /// Abstract component for entities that should be displayed.
    /// </summary>
    public abstract class Drawable : Component
    {
        /// <summary>
        /// Gets the layer the owning entity should be drawn on.
        /// </summary>
        public virtual DrawableLayer Layer { get { return DrawableLayer.Items; } set { } }

        /// <summary>
        /// Gets the symbol used to represent this entity.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>A character to be drawn to represent this entity.</returns>
        public virtual char GetSymbol(DrawAttributes attribs)
        {
            return '?';
        }

        /// <summary>
        /// Gets the color to be used when drawing this entity's symbol.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>The color of the symbol to be drawn.</returns>
        public virtual ConsoleColor GetForeColor(DrawAttributes attribs)
        {
            return ConsoleColor.Gray;
        }

        /// <summary>
        /// Gets the color of the background to be used when drawing this entity.
        /// </summary>
        /// <param name="attribs">Attributes that may be used when drawing the entity.</param>
        /// <returns>The background colour of the entity.</returns>
        public virtual ConsoleColor GetBackColor(DrawAttributes attribs)
        {
            return ConsoleColor.Black;
        }
    }
}
