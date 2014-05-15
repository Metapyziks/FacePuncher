using System;
using System.Collections.Generic;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Delegate for entity constructors.
    /// </summary>
    /// <param name="ent"></param>
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
            get { return Parent != null; }
        }

        /// <summary>
        /// Gets the tile this entity is currently placed on, or the
        /// tile this entity's parent resides on if applicable.
        /// </summary>
        public Tile Tile { get { return HasParent ? Parent.Tile : _tile; } }

        public Room Room { get { return Tile.Room; } }

        public Level Level { get { return Room.Level; } }

        public bool IsValid
        {
            get { return Tile != null && Tile.Contains(this); }
        }

        public Position RelativePosition
        {
            get { return Tile.RelativePosition; }
        }

        public Position Position
        {
            get { return Tile.Position; }
        }

        private Entity(uint id)
        {
            ID = id;

            _comps = new List<Component>();
            _compDict = new Dictionary<Type, Component>();

            _children = new List<Entity>();

            _compsChanged = false;
        }

        public bool IsOfClass(String baseName)
        {
            return Extends(ClassName, baseName);
        }

        public Entity AddChild(Entity child)
        {
            if (_children.Contains(child)) return child;

            if (child.IsValid) {
                child.Remove();
            }

            _children.Add(child);

            child.Parent = this;

            foreach (var comp in child) {
                comp.OnPlace();
            }

            return child;
        }

        private Entity RemoveChild(Entity child)
        {
            if (!_children.Contains(child)) return child;

            _children.Remove(child);

            child.Parent = null;

            return child;
        }

        public T AddComponent<T>()
            where T : Component
        {
            return (T) AddComponent(typeof(T));
        }

        public Component AddComponent(Type type)
        {
            Component comp = Component.Create(type, this);

            do _compDict.Add(type, comp);
            while ((type = type.BaseType) != typeof(Component));

            _comps.Add(comp);

            _compsChanged = true;

            return comp;
        }

        public Entity RemoveComponent<T>()
            where T : Component
        {
            T comp = GetComponent<T>();
            Type type = typeof(T);

            comp.OnRemove();

            do _compDict.Remove(type);
            while ((type = type.BaseType) != typeof(Component));

            _comps.Remove(comp);

            _compsChanged = true;

            return this;
        }

        public TNew SwapComponent<TOld, TNew>()
            where TOld : Component
            where TNew : Component
        {
            RemoveComponent<TOld>();
            return AddComponent<TNew>();
        }

        public bool HasComponent<T>()
            where T : Component
        {
            return _compDict.ContainsKey(typeof(T));
        }

        public bool HasComponent(Type t)
        {
            return _compDict.ContainsKey(t);
        }

        public T GetComponent<T>()
            where T : Component
        {
            return (T) _compDict[typeof(T)];
        }

        public T GetComponentOrNull<T>()
            where T : Component
        {
            if (_compDict.ContainsKey(typeof(T))) {
                return (T) _compDict[typeof(T)];
            } else {
                return null;
            }
        }

        public Component GetComponent(Type t)
        {
            return _compDict[t];
        }

        public Component GetComponentOrNull(Type t)
        {
            if (_compDict.ContainsKey(t)) {
                return _compDict[t];
            } else {
                return null;
            }
        }
        
        private void UpdateComponents()
        {
            foreach (var other in _comps) {
                other.OnUpdateComponents();
            }

            _compsChanged = false;
        }

        public void Place(Tile tile)
        {
            Remove();

            _tile = tile;
            tile.AddEntity(this);

            foreach (var comp in _comps) {
                comp.OnPlace();
            }
        }

        public void Remove()
        {
            if (IsValid) {
                foreach (var comp in this) comp.OnRemove();
                foreach (var child in _children) child.Remove();

                if (HasParent) {
                    Parent.RemoveChild(this);
                } else {
                    var tile = Tile; _tile = null;
                    tile.RemoveEntity(this);
                }
            }
        }

        public bool CanMove(Tile dest)
        {
            return !HasParent && dest.State == TileState.Floor;
        }

        public bool CanMove(Direction dir)
        {
            return CanMove(Tile.GetNeighbour(dir));
        }

        public void Move(Tile dest)
        {
            if (!CanMove(dest)) return;

            var orig = Tile;
            _tile = dest;

            orig.RemoveEntity(this);
            Tile.AddEntity(this);
        }

        public void Move(Direction dir)
        {
            Move(Tile.GetNeighbour(dir));
        }

        public void Think(ulong time)
        {
            if (_lastThink >= time) return;
            _lastThink = time;

            if (_compsChanged) UpdateComponents();

            for (int i = _comps.Count - 1; i >= 0; --i)
                _comps[i].OnThink(time);

            foreach (var child in _children) child.Think(time);
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return _comps.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return ClassName ?? "entity";
        }
    }
}
