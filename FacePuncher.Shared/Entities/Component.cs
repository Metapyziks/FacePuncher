using System;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    public abstract class Component
    {
        public static T Create<T>(Entity ent)
            where T : Component
        {
            var t = typeof(T);
            var c = t.GetConstructor(new Type[0]);

            if (c == null) {
                throw new MissingMethodException(
                    String.Format("Type {0} is missing a valid constructor.", t.FullName));
            }

            var comp = (T) c.Invoke(new object[0]);
            comp.Entity = ent;

            return comp;
        }

        protected uint ID
        {
            get { return Entity.ID; }
        }

        protected Tile Tile
        {
            get { return Entity.Tile; }
        }

        protected Room Room
        {
            get { return Entity.Room; }
        }

        protected Level Level
        {
            get { return Entity.Level; }
        }

        protected Position RelativePosition
        {
            get { return Entity.RelativePosition; }
        }

        protected Position Position
        {
            get { return Entity.Position; }
        }

        public Entity Entity { get; private set; }

        public virtual void OnInitialize() { }

        public virtual void OnUpdateComponents() { }

        public virtual void OnThink(ulong time) { }

        public virtual void OnRemove() { }
    }
}
