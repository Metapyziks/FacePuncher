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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Delegate for entity constructors.
    /// </summary>
    /// <param name="ent">Entity to construct.</param>
    public delegate void EntityConstructorDelegate(Entity ent);

    /// <summary>
    /// Represents a game object, with functionality defined
    /// by a collection of components.
    /// </summary>
    public sealed class Entity : IEnumerable<Component>
    {
        private static uint _sNextID = 0;

        /// <summary>
        /// Holds information to be used when constructing an
        /// entity of a registered class, or to find entity
        /// classes with certain components.
        /// </summary>
        private sealed class ClassInfo
        {
            /// <summary>
            /// Class name for this type of entity.
            /// </summary>
            public String Name { get; private set; }

            /// <summary>
            /// Class name for the base class of this entity,
            /// or null if this entity has no base class.
            /// </summary>
            public String Base { get; private set; }

            /// <summary>
            /// Constructor for this entity class.
            /// </summary>
            public EntityConstructorDelegate Constructor { get; private set; }

            /// <summary>
            /// A list of components this entity class is expected to
            /// possess by default, for use in finding entity classes
            /// with a specified set of components.
            /// </summary>
            public Type[] Components { get; private set; }

            /// <summary>
            /// Constructs a new Entity.ClassInfo with no specified
            /// base class.
            /// </summary>
            /// <param name="name">Class name for this type of entity.</param>
            /// <param name="ctor">Constructor for this entity class.</param>
            /// <param name="components">Components this class contains by
            /// default.</param>
            public ClassInfo(String name, EntityConstructorDelegate ctor, Type[] components)
            {
                Name = name;
                Base = null;
                Constructor = ctor;
                Components = components;
            }

            /// <summary>
            /// Constructs a new Entity.ClassInfo with a specified
            /// base class.
            /// </summary>
            /// <param name="name">Class name for this type of entity.</param>
            /// <param name="baseName">Class name for the base class for this
            /// type of entity.</param>
            /// <param name="ctor">Constructor for this entity class.</param>
            /// <param name="components">Components this class contains by
            /// default.</param>
            public ClassInfo(String name, String baseName, EntityConstructorDelegate ctor, Type[] components)
            {
                Name = name;
                Base = baseName;
                Constructor = ctor;
                Components = components;
            }
        }

        private static Dictionary<String, ClassInfo> _sEntCtors
            = new Dictionary<string, ClassInfo>();

        /// <summary>
        /// Static constructor for Entity.
        /// </summary>
        static Entity()
        {
            Definitions.RegisterType("entity", elem => {
                var components = new List<Tuple<Type, XElement>>();

                foreach (var sub in elem.Elements()) {
                    var typeName = String.Format("FacePuncher.Entities.{0}", sub.Name.LocalName);
                    var type = Assembly.GetEntryAssembly().GetType(typeName);

                    if (type == null) continue;

                    components.Add(Tuple.Create(type, sub));
                }

                EntityConstructorDelegate ctor = ent => {
                    foreach (var type in components) {
                        var comp = ent.GetComponentOrNull(type.Item1)
                            ?? ent.AddComponent(type.Item1);

                        comp.LoadFromDefinition(type.Item2);
                    }
                };

                var compTypes = components.Select(x => x.Item1).ToArray();

                if (elem.Attributes("base").Count() > 0) {
                    Entity.Register(elem.Attribute("name").Value, elem.Attribute("base").Value, ctor, compTypes);
                } else {
                    Entity.Register(elem.Attribute("name").Value, ctor, compTypes);
                }
            });
        }

        /// <summary>
        /// Register an entity class.
        /// </summary>
        /// <param name="name">Entity class name to register.</param>
        /// <param name="ctor">Constructor to be invoked on new
        /// instances of the class.</param>
        /// <param name="componentTypes">Types of components this entity
        /// is expected to contain, to be used to find entity classes with
        /// certain components. Not required.</param>
        public static void Register(String name, EntityConstructorDelegate ctor, params Type[] componentTypes)
        {
            _sEntCtors.Add(name, new ClassInfo(name, ctor, componentTypes));
        }

        /// <summary>
        /// Register an entity class that extends a named base class.
        /// </summary>
        /// <param name="name">Entity class name to register.</param>
        /// <param name="baseName">Base class name to extend.</param>
        /// <param name="ctor">Constructor to be invoked on new
        /// instances of the class.</param>
        /// <param name="componentTypes">Types of components this entity
        /// is expected to contain, to be used to find entity classes with
        /// certain components. Not required.</param>
        public static void Register(String name, String baseName, EntityConstructorDelegate ctor, params Type[] componentTypes)
        {
            _sEntCtors.Add(name, new ClassInfo(name, baseName, ctor, componentTypes));
        }

        /// <summary>
        /// Gets the base class name of an existing specified entity class,
        /// or null if that entity class has no base.
        /// </summary>
        /// <param name="className">Name of the entity class to find the
        /// base class of.</param>
        /// <returns>Base class name if one exists, otherwise null.</returns>
        public static String GetBaseName(String className)
        {
            return _sEntCtors[className].Base;
        }

        /// <summary>
        /// Gets a set of component types for the specified entity class.
        /// </summary>
        /// <param name="className">Name of the class to find component
        /// types for.</param>
        /// <returns>An array of component types.</returns>
        public static Type[] GetComponentTypes(String className)
        {
            var info = _sEntCtors[className];

            if (info.Base == null) return info.Components;

            // This will be quite inefficient, maybe consider caching
            // if performance is an issue.
            else return info.Components
                .Union(GetComponentTypes(info.Base))
                .ToArray();
        }

        /// <summary>
        /// Tests to see if the given class extends or is equal to the
        /// specified base class.
        /// </summary>
        /// <param name="className">Name of the class to test the inheritance of.</param>
        /// <param name="baseName">Name of the prospective base class.</param>
        /// <returns>True if the given class either extends or is equal to
        /// the specified base class, and false otherwise.</returns>
        public static bool Extends(String className, String baseName)
        {
            if (className == null) return false;
            if (className == baseName) return true;

            return Extends(_sEntCtors[className].Base, baseName);
        }

        /// <summary>
        /// Gets the names of all registered entity classes.
        /// </summary>
        /// <returns></returns>
        public static String[] GetClassNames()
        {
            return _sEntCtors.Select(x => x.Key).ToArray();
        }

        /// <summary>
        /// Gets the names of all registered entity classes that extend
        /// the specified base class. Can optionally filter to only return
        /// classes that have no extending classes of their own.
        /// </summary>
        /// <param name="baseName">Base name to find extending classes of.</param>
        /// <param name="onlyLeaves">If true, will only return classes with no
        /// extending classes of their own.</param>
        /// <returns>An array of class names.</returns>
        public static String[] GetClassNames(String baseName, bool onlyLeaves)
        {
            var names = _sEntCtors.Select(x => x.Key)
                .Where(x => Extends(x, baseName));

            if (onlyLeaves) {
                return names
                    .Where(x => !_sEntCtors.Any(y => y.Value.Base == x))
                    .ToArray();
            } else {
                return names.ToArray();
            }
        }

        /// <summary>
        /// Creates a basic entity, upon which components may be
        /// attached to flesh out functionality.
        /// </summary>
        /// <returns>An empty entity.</returns>
        public static Entity Create()
        {
            return new Entity(_sNextID++);
        }

        /// <summary>
        /// Creates an entity as an instance of a named entity class.
        /// </summary>
        /// <param name="type">Class name of the entity.</param>
        /// <returns>An entity instance of the specified class.</returns>
        public static Entity Create(String type)
        {
            ClassInfo info = _sEntCtors[type];
            Entity ent = (info.Base != null ? Create(info.Base) : Create());
            ent.ClassName = info.Name;
            info.Constructor(ent);
            return ent;
        }

        /// <summary>
        /// Creates a basic entity, upon which components may be
        /// attached to flesh out functionality. The entity uses a
        /// specified ID rather than automatically aquiring one.
        /// </summary>
        /// <param name="id">Numeric identifier for this entity.</param>
        /// <returns>An empty entity using the specified ID.</returns>
        public static Entity Create(uint id)
        {
            return new Entity(id);
        }

        /// <summary>
        /// Creates an entity as an instance of a named entity class.
        /// The entity uses a specified ID rather than automatically
        /// aquiring one.
        /// </summary>
        /// <param name="id">Numeric identifier for this entity.</param>
        /// <param name="type">Class name of the entity.</param>
        /// <returns>An entity instance of the specified class using a
        /// given ID.</returns>
        public static Entity Create(uint id, String type)
        {
            ClassInfo info = _sEntCtors[type];
            Entity ent = (info.Base != null ? Create(id, info.Base) : Create(id));
            ent.ClassName = info.Name;
            info.Constructor(ent);
            return ent;
        }

        private Tile _tile;
        private List<Component> _comps;
        private Dictionary<Type, Component> _compDict;
        private List<Entity> _children;
        private bool _compsChanged;
        private ulong _lastThink;
        private Component _thinkProbe;

        /// <summary>
        /// Gets the numeric identifier for this entity.
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Gets the name of the lowest-level class this entity is an
        /// instance of.
        /// </summary>
        public String ClassName { get; private set; }

        /// <summary>
        /// Gets the parent instance of this entity if it has one, and
        /// null otherwise.
        /// </summary>
        public Entity Parent { get; private set; }

        /// <summary>
        /// Get the set of entities attached to this one.
        /// </summary>
        public IEnumerable<Entity> Children { get { return _children; } }

        /// <summary>
        /// Gets whether this entity is attached to a parent.
        /// </summary>
        public bool HasParent
        {
            get { return Parent != null && Parent.Children.Contains(this); }
        }

        /// <summary>
        /// Gets the tile this entity is currently placed on, or the
        /// tile this entity's parent resides on if applicable.
        /// </summary>
        public Tile Tile { get { return HasParent ? Parent.Tile : _tile; } }

        /// <summary>
        /// Gets the room this entity currently resides in.
        /// </summary>
        public Room Room { get { return Tile.Room; } }

        /// <summary>
        /// Gets the level this entity currently resides in.
        /// </summary>
        public Level Level { get { return Room.Level; } }

        /// <summary>
        /// If true, the entity currently exists in the world.
        /// </summary>
        public bool IsValid
        {
            get { return (HasParent && Parent.IsValid) || (Tile != null && Tile.Contains(this)); }
        }

        /// <summary>
        /// If true, this entity contains a component that overrides OnThink().
        /// </summary>
        public bool CanThink
        {
            get { return _thinkProbe != null || _children.Any(x => x.CanThink); }
        }

        /// <summary>
        /// Gets the position of this entity relative to its
        /// containing room.
        /// </summary>
        public Position RelativePosition
        {
            get { return Tile.RelativePosition; }
        }

        /// <summary>
        /// Gets the position of this entity relative to the
        /// level origin.
        /// </summary>
        public Position Position
        {
            get { return Tile.Position; }
        }

        /// <summary>
        /// Constructs an empty entity instance with the
        /// specified numeric identifier.
        /// </summary>
        /// <param name="id">Numeric identifier for this instance.</param>
        private Entity(uint id)
        {
            ID = id;

            _comps = new List<Component>();
            _compDict = new Dictionary<Type, Component>();

            _children = new List<Entity>();

            _compsChanged = false;
        }

        /// <summary>
        /// Tests to see if this entity's class extends or is
        /// equal to the specified base class.
        /// </summary>
        /// <param name="baseName">Prospective base class to test.</param>
        /// <returns>True if this entity is an instance of the specified
        /// class, false otherwise.</returns>
        public bool IsOfClass(String baseName)
        {
            return Extends(ClassName, baseName);
        }

        /// <summary>
        /// Attaches a child entity to this one.
        /// </summary>
        /// <param name="child">Entity to adopt.</param>
        /// <returns>The adopted entity for convenience.</returns>
        public Entity AddChild(Entity child)
        {
            if (_children.Contains(child)) return child;

            // Make sure the child doesn't have two parents.
            if (child.IsValid) child.Remove();

            _children.Add(child);

            child.Parent = this;

            child.UpdateComponents();

            foreach (var comp in child) {
                comp.OnPlace();
            }

            // For convenience.
            return child;
        }

        /// <summary>
        /// Removes a child entity that was previously parented to this one.
        /// </summary>
        /// <param name="child">Child to disown.</param>
        private void RemoveChild(Entity child)
        {
            if (!_children.Contains(child)) return;

            _children.Remove(child);

            child.Parent = null;
        }

        /// <summary>
        /// Adds a newly created component to this entity's component
        /// list and dictionary.
        /// </summary>
        /// <param name="comp">Component to add.</param>
        /// <param name="type">Type of the component.</param>
        /// <returns>The component, for convenience.</returns>
        private Component AddComponent(Component comp, Type type)
        {
            if (_thinkProbe == null && comp.CanThink) {
                _thinkProbe = comp;
            }

            do _compDict.Add(type, comp);
            while ((type = type.BaseType) != typeof(Component));

            _comps.Add(comp);

            _compsChanged = true;

            return comp;
        }

        /// <summary>
        /// Add a component of the specified type to this entity.
        /// </summary>
        /// <typeparam name="T">Type of the component to add.</typeparam>
        /// <returns>The newly added component.</returns>
        public T AddComponent<T>()
            where T : Component, new()
        {
            return (T) AddComponent(Component.Create<T>(this), typeof(T));
        }

        /// <summary>
        /// Add a component of the specified type to this entity.
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <returns>The newly added component.</returns>
        public Component AddComponent(Type type)
        {
            return AddComponent(Component.Create(type, this), type);
        }

        /// <summary>
        /// Removes the specified component from this entity's component
        /// list and dictionary.
        /// </summary>
        /// <param name="comp">Component to remove.</param>
        /// <param name="type">Type of the component.</param>
        private void RemoveComponent(Component comp, Type type)
        {
            comp.OnRemove();

            do _compDict.Remove(type);
            while ((type = type.BaseType) != typeof(Component));

            _comps.Remove(comp);

            if (_thinkProbe == comp) {
                _thinkProbe = _comps.FirstOrDefault(x => x.CanThink);
            }

            _compsChanged = true;
        }

        /// <summary>
        /// Removes a component from this entity.
        /// </summary>
        /// <typeparam name="T">Type of the component to remove.</typeparam>
        public void RemoveComponent<T>()
            where T : Component
        {
            RemoveComponent(GetComponent<T>(), typeof(T));
        }

        /// <summary>
        /// Removes a component from this entity.
        /// </summary>
        /// <param name="type">Type of the component to remove.</param>
        public void RemoveComponent(Type type)
        {
            RemoveComponent(GetComponent(type), type);
        }

        /// <summary>
        /// Tests to see whether a component instance of the
        /// specified type is currently used by this entity.
        /// </summary>
        /// <typeparam name="T">Type of the component to look for.</typeparam>
        /// <returns>True if this entity contains a component of
        /// the specified type, and false otherwise.</returns>
        public bool HasComponent<T>()
            where T : Component
        {
            return _compDict.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Tests to see whether a component instance of the
        /// specified type is currently used by this entity.
        /// </summary>
        /// <param name="type">Type of the component to look for.</param>
        /// <returns>True if this entity contains a component of
        /// the specified type, and false otherwise.</returns>
        public bool HasComponent(Type type)
        {
            return _compDict.ContainsKey(type);
        }

        /// <summary>
        /// Gets a component of the specified type used by this entity.
        /// </summary>
        /// <typeparam name="T">Type of the component to look for.</typeparam>
        /// <returns>A component instance of the specified type.</returns>
        public T GetComponent<T>()
            where T : Component
        {
            return (T) _compDict[typeof(T)];
        }

        /// <summary>
        /// Gets a component of the specified type used by this entity if one
        /// exists, and otherwise returns null.
        /// </summary>
        /// <typeparam name="T">Type of the component to look for.</typeparam>
        /// <returns>A component instance of the specified type if one is
        /// found, and otherwise null.</returns>
        public T GetComponentOrNull<T>()
            where T : Component
        {
            if (_compDict.ContainsKey(typeof(T))) {
                return (T) _compDict[typeof(T)];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets a component of the specified type used by this entity.
        /// </summary>
        /// <param name="type">Type of the component to look for.</param>
        /// <returns>A component instance of the specified type.</returns>
        public Component GetComponent(Type type)
        {
            return _compDict[type];
        }

        /// <summary>
        /// Gets a component of the specified type used by this entity if one
        /// exists, and otherwise returns null.
        /// </summary>
        /// <param name="type">Type of the component to look for.</param>
        /// <returns>A component instance of the specified type if one is
        /// found, and otherwise null.</returns>
        public Component GetComponentOrNull(Type type)
        {
            if (_compDict.ContainsKey(type)) {
                return _compDict[type];
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Lets each component know that some components have recently
        /// been added or removed from this entity.
        /// </summary>
        private void UpdateComponents()
        {
            if (!_compsChanged) return;

            foreach (var other in _comps) {
                other.OnUpdateComponents();
            }

            Room.UpdateThinkProbe(this);

            _compsChanged = false;
        }

        /// <summary>
        /// Places the entity on a tile, while removing it from an
        /// existing tile or parent entity if applicable.
        /// </summary>
        /// <param name="tile">Destination tile to place this entity on.</param>
        public void Place(Tile tile)
        {
            Remove();

            _tile = tile;
            tile.AddEntity(this);

            UpdateComponents();

            foreach (var comp in _comps) {
                comp.OnPlace();
            }

            Room.UpdateThinkProbe(this);
        }

        /// <summary>
        /// Tells each component of this entity that it is about to be removed
        /// from the world, and recursively tells each child too.
        /// </summary>
        private void OnRemove()
        {
            Room.UpdateThinkProbe(this);

            foreach (var comp in this) comp.OnRemove();
            foreach (var child in _children) child.OnRemove();
        }

        /// <summary>
        /// Removes this entity from either the tile it is placed on
        /// or its parent entity.
        /// </summary>
        public void Remove()
        {
            if (IsValid) {
                OnRemove();

                if (HasParent) {
                    Parent.RemoveChild(this);
                } else {
                    var tile = Tile; _tile = null;
                    tile.RemoveEntity(this);
                }
            }
        }

        /// <summary>
        /// Tests to see whether this entity can move to the specified tile.
        /// </summary>
        /// <param name="dest">Destination tile to move to.</param>
        /// <returns>True if the entity can move, and false otherwise.</returns>
        public bool CanMove(Tile dest)
        {
            return !HasParent && dest.State == TileState.Floor;
        }

        /// <summary>
        /// Tests to see whether this entity can move one tile in the specified direction.
        /// </summary>
        /// <param name="dir">Direction to move in.</param>
        /// <returns>True if the entity can move, and false otherwise.</returns>
        public bool CanMove(Direction dir)
        {
            return CanMove(Tile.GetNeighbour(dir));
        }

        /// <summary>
        /// Attempts to move this entity to the specified tile.
        /// </summary>
        /// <param name="dest">Destination tile to move to.</param>
        public void Move(Tile dest)
        {
            if (!CanMove(dest)) return;

            var orig = Tile;
            _tile = dest;

            if (dest.Room != orig.Room) {
                Room.UpdateThinkProbe(this);
            }

            orig.RemoveEntity(this);
            Tile.AddEntity(this);
            
            if (dest.Room != orig.Room) {
                Room.UpdateThinkProbe(this);
            }
        }

        /// <summary>
        /// Attempts to move this entity one tile in the specified direction.
        /// </summary>
        /// <param name="dir">Direction to move in.</param>
        public void Move(Direction dir)
        {
            Move(Tile.GetNeighbour(dir));
        }

        /// <summary>
        /// Gives each component of this entity an opportunity to perform some
        /// actions, while also recursively invoking this method on any child
        /// entities.
        /// </summary>
        public void Think()
        {
            if (!CanThink || _lastThink >= Level.Time) return;

            _lastThink = Level.Time;

            UpdateComponents();

            for (int i = _comps.Count - 1; i >= 0; --i)
                _comps[i].OnThink();

            foreach (var child in _children) child.Think();
        }

        /// <summary>
        /// Gets an enumerator that iterates through each component in this entity.
        /// </summary>
        /// <returns>An enumerator to iterate through each component.</returns>
        public IEnumerator<Component> GetEnumerator()
        {
            return _comps.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through each component in this entity.
        /// </summary>
        /// <returns>An enumerator to iterate through each component.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents this entity.
        /// </summary>
        /// <returns>A string that represents this entity.</returns>
        public override string ToString()
        {
            return ClassName ?? "entity";
        }
    }
}
