/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Base class for entity components.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Creates a component for the specified entity.
        /// </summary>
        /// <typeparam name="T">Component type to create an
        /// instance of.</typeparam>
        /// <param name="ent">Entity to bind the component to.</param>
        /// <returns>A component bound to the specified entity.</returns>
        internal static T Create<T>(Entity ent)
            where T : Component, new()
        {
            var comp = new T { Entity = ent };

            comp.OnInitialize();

            return comp;
        }

        /// <summary>
        /// Creates a component for the specified entity.
        /// </summary>
        /// <param name="t">Component type to create an
        /// instance of.</param>
        /// <param name="ent">Entity to bind the component to.</param>
        /// <returns>A component bound to the specified entity.</returns>
        internal static Component Create(Type t, Entity ent)
        {
            var c = t.GetConstructor(new Type[0]);

            if (c == null) {
                throw new MissingMethodException(
                    String.Format("Type {0} is missing a valid constructor.", t.FullName));
            }

            var comp = (Component) c.Invoke(new object[0]);
            comp.Entity = ent;

            comp.OnInitialize();

            return comp;
        }

        /// <summary>
        /// Gets the ID of the host entity.
        /// </summary>
        protected uint ID
        {
            get { return Entity.ID; }
        }

        /// <summary>
        /// Gets the tile the host entity currently occupies.
        /// </summary>
        protected Tile Tile
        {
            get { return Entity.Tile; }
        }

        /// <summary>
        /// Gets the room the host entity currently occupies.
        /// </summary>
        protected Room Room
        {
            get { return Entity.Room; }
        }

        /// <summary>
        /// Gets the level the host entity currently occupies.
        /// </summary>
        protected Level Level
        {
            get { return Entity.Level; }
        }

        /// <summary>
        /// Gets the current game time.
        /// </summary>
        protected double Time
        {
            get { return Level.Time; }
        }

        /// <summary>
        /// Gets the position of the host entity relative to its
        /// contatining room.
        /// </summary>
        protected Position RelativePosition
        {
            get { return Entity.RelativePosition; }
        }

        /// <summary>
        /// Gets the position of the host entity relative to the
        /// level origin.
        /// </summary>
        protected Position Position
        {
            get { return Entity.Position; }
        }

        /// <summary>
        /// Gets the host entity.
        /// </summary>
        public Entity Entity { get; private set; }
        
        /// <summary>
        /// Loads state information from an XML definition.
        /// </summary>
        /// <param name="elem">Element describing the state of
        /// the component.</param>
        public virtual void LoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);
        }

        /// <summary>
        /// Called when the component is first bound to its
        /// entity.
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// Called whenever the host entity is placed in a new 
        /// level or attached to a new parent entity.
        /// </summary>
        public virtual void OnPlace() { }

        /// <summary>
        /// Called whenever the component set of the host entity
        /// changes.
        /// </summary>
        public virtual void OnUpdateComponents() { }

        /// <summary>
        /// Called when the parent entity becomes active.
        /// </summary>
        public virtual void OnWake() { }

        /// <summary>
        /// Called when the parent entity becomes inactive.
        /// </summary>
        public virtual void OnSleep() { }

        /// <summary>
        /// Called when either the component is removed from its
        /// entity or the entity is removed from the world.
        /// </summary>
        public virtual void OnRemove() { }

        /// <summary>
        /// Adds a delayed action to the action queue of the level.
        /// </summary>
        /// <param name="delay">Amount of time units until the action
        /// is to be performed.</param>
        /// <param name="action">Action to perform.</param>
        protected void Schedule(double delay, Action action)
        {
            Level.Schedule(delay, this, action);
        }
    }
}
