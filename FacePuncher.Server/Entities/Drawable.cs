/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

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
