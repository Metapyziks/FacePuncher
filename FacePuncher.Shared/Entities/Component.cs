using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Used to specify properties that may be set in definition files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ScriptDefinableAttribute : Attribute { }

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
            // As a default implementation, use reflection to
            // set properties marked as ScriptDefinable.
            foreach (var sub in elem.Elements()) {
                var ident = sub.Name.LocalName;
                var prop = GetType().GetProperty(ident);
                
                if (prop == null) continue;
                if (prop.GetCustomAttributes<ScriptDefinableAttribute>().Count() == 0) return;

                prop.SetValue(this, elem.Element(sub.Name, prop.PropertyType));
            }
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
        /// Called once per game step.
        /// </summary>
        /// <param name="time">Current game time.</param>
        public virtual void OnThink(ulong time) { }

        /// <summary>
        /// Called when either the component is removed from its
        /// entity or the entity is removed from the world.
        /// </summary>
        public virtual void OnRemove() { }
    }
}
