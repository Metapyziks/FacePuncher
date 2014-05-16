using System;

using FacePuncher.Graphics;

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
        public abstract DrawableLayer GetLayer();

        /// <summary>
        /// Gets appearance information for the entity.
        /// </summary>
        /// <returns>Appearance information for the entity.</returns>
        public abstract EntityAppearance GetAppearance();
    }
}
