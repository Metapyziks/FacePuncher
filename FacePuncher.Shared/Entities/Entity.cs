using System;
using System.Collections.Generic;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    public delegate void EntityBuilderDelegate(Entity ent);

    public sealed class Entity : IEnumerable<Component>
    {
        private static uint _sNextID = 0;

        private struct BuilderInfo
        {
            public readonly String Name;
            public readonly String Base;
            public readonly EntityBuilderDelegate Builder;

            public BuilderInfo(String name, EntityBuilderDelegate builder)
            {
                Name = name;
                Base = null;
                Builder = builder;
            }

            public BuilderInfo(String name, String baseName, EntityBuilderDelegate builder)
            {
                Name = name;
                Base = baseName;
                Builder = builder;
            }
        }

        private static Dictionary<String, BuilderInfo> _sEntBuilders
            = new Dictionary<string, BuilderInfo>();

        public static void Register(String name, EntityBuilderDelegate builder)
        {
            if (!_sEntBuilders.ContainsKey(name))
                _sEntBuilders.Add(name, new BuilderInfo(name, builder));
            else
                _sEntBuilders[name] = new BuilderInfo(name, builder);
        }

        public static void Register(String name, String baseName, EntityBuilderDelegate builder)
        {
            if (!_sEntBuilders.ContainsKey(name))
                _sEntBuilders.Add(name, new BuilderInfo(name, baseName, builder));
            else
                _sEntBuilders[name] = new BuilderInfo(name, baseName, builder);
        }

        public static Entity Create()
        {
            return new Entity();
        }

        public static Entity Create(String type)
        {
            BuilderInfo info = _sEntBuilders[type];
            Entity ent = (info.Base != null ? Create(info.Base) : Create());
            ent.PushClassName(type);
            info.Builder(ent);
            return ent;
        }

        private Stack<String> _classNames;

        private List<Component> _comps;
        private Dictionary<Type, Component> _compDict;
        private List<Entity> _children;
        private bool _compsChanged;
        private ulong _lastThink;

        public readonly uint ID;

        public String ClassName { get { return _classNames.LastOrDefault(); } }

        public Entity Parent { get; private set; }

        public IEnumerable<Entity> Children { get { return _children; } }

        public bool HasParent
        {
            get { return Parent != null; }
        }

        public Tile Tile { get; private set; }

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

        private Entity()
        {
            ID = _sNextID++;

            _classNames = new Stack<string>();

            _comps = new List<Component>();
            _compDict = new Dictionary<Type, Component>();

            _children = new List<Entity>();

            _compsChanged = false;
        }

        private void PushClassName(String className)
        {
            _classNames.Push(className);
        }

        public bool IsOfClass(String className)
        {
            return _classNames.Contains(className);
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
                comp.OnInitialize();
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
            T comp = Component.Create<T>(this);
            Type type = typeof(T);

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

            Tile = tile;
            tile.AddEntity(this);

            foreach (var comp in _comps) {
                comp.OnInitialize();
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
                    var tile = Tile; Tile = null;
                    tile.RemoveEntity(this);
                }
            }
        }

        public void Move(Tile dest)
        {
            if (dest.State == TileState.Void || dest.State == TileState.Wall)
                return;

            var orig = Tile;
            Tile = dest;

            orig.RemoveEntity(this);
            Tile.AddEntity(this);
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
