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
using System.IO;
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
        private static List<Type> _sCompTypes = new List<Type>();
        private static Dictionary<Type, ushort> _sTypeIDs = new Dictionary<Type, ushort>();

        static Component()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (!typeof(Component).IsAssignableFrom(type)) continue;
                if (type.IsAbstract) continue;

                _sTypeIDs.Add(type, (ushort) _sCompTypes.Count);
                _sCompTypes.Add(type);
            }
        }

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

        internal static void WriteComponentDict(BinaryWriter writer)
        {
            writer.Write((ushort) _sCompTypes.Count);
            foreach (var type in _sCompTypes) {
                writer.Write(type.FullName);
            }
        }

        internal static void LoadComponentDict(BinaryReader reader)
        {
            _sCompTypes.Clear();
            _sTypeIDs.Clear();

            var asm = Assembly.GetExecutingAssembly();

            ushort types = reader.ReadUInt16();
            for (ushort i = 0; i < types; ++i) {
                var type = asm.GetType(reader.ReadString());

                _sTypeIDs.Add(type, i);
                _sCompTypes.Add(type);
            }
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

        public bool IsValid { get { return Entity.IsValid && Entity.Contains(this); } }

        public bool IsActive { get { return Entity.IsActive && IsValid; } }

        /// <summary>
        /// Gets the host entity.
        /// </summary>
        public Entity Entity { get; private set; }

        public ushort TypeID { get { return _sTypeIDs[GetType()]; } }
        
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
        protected virtual void OnWake() { }

        /// <summary>
        /// Called when the parent entity becomes inactive.
        /// </summary>
        protected virtual void OnSleep() { }

        /// <summary>
        /// Called when either the component is removed from its
        /// entity or the entity is removed from the world.
        /// </summary>
        public virtual void OnRemove() { }

        public async void Wake()
        {
            await Delay(0.0, true);
            OnWake();
        }

        public async void Sleep()
        {
            await Delay(0.0, true);
            OnSleep();
        }

        public virtual void Save(BinaryWriter writer) { }

        public virtual void Load(BinaryReader reader) { }

        /// <summary>
        /// Adds a delayed action to the action queue of the level.
        /// </summary>
        /// <param name="delay">Amount of time units until the action
        /// is to be performed.</param>
        /// <param name="action">Action to perform.</param>
        protected Delay Delay(double delay, bool forceYield = false)
        {
            return Level.Delay(delay, this, forceYield);
        }
    }
}
