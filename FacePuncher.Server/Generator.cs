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

namespace FacePuncher
{
    abstract class GenerationWorker
    {
        public virtual void LoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);
        }
    }

    abstract class Generator<T>
        where T : Generator<T>, new()
    {
        private GeneratorCollection<T> _generators;
        private XElement _element;

        public String Name { get; private set; }

        public String BaseName { get; private set; }

        public bool HasBase { get { return BaseName != null; } }

        public T Base { get { return _generators[BaseName]; } }

        public bool Loaded { get; private set; }

        public IEnumerable<Generator<T>> InheritanceChain
        {
            get 
            {
                if (HasBase) {
                    foreach (var prev in Base.InheritanceChain) {
                        yield return prev;
                    }
                }

                yield return this;
            }
        }

        public void Initialize(GeneratorCollection<T> generators, XElement elem)
        {
            _generators = generators;
            _element = elem;

            Name = elem.Attribute("name").Value;
            BaseName = elem.HasAttribute("base")
                ? elem.Attribute("base").Value : null;

            Loaded = false;
        }

        public void Load()
        {
            foreach (var prev in InheritanceChain) {
                OnLoadFromDefinition(prev._element);
            }

            Loaded = true;
        }

        protected T LoadWorkerFromDefinition<T>(XElement elem, T fallback)
            where T : GenerationWorker
        {
            var subName = typeof(T).Name;
            if (!elem.HasElement(subName)) return fallback;

            var sub = elem.Element(subName);

            if (sub.HasAttribute("class")) {
                var name = sub.Attribute("class").Value;
                var typeName = String.Format("FacePuncher.Geometry.{0}s.{1}", subName, name);

                var type = Assembly.GetEntryAssembly().GetType(typeName);
                if (type == null) throw new Exception("Invalid RoomPlacement type specified.");

                if (fallback.GetType() != type) {
                    var ctor = type.GetConstructor(new Type[0]);
                    if (ctor == null) throw new Exception(String.Format("RoomPlacement type {0} has no valid constructor.", name));

                    fallback = (T) ctor.Invoke(new Object[0]);
                }
            }

            fallback.LoadFromDefinition(sub);

            return fallback;
        }

        protected virtual void OnLoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);
        }
    }

    class GeneratorCollection<T>
        : IEnumerable<T>
        where T : Generator<T>, new()
    {
        private Dictionary<String, T> _generators;

        public T this[String type]
        {
            get
            {
                var gen = _generators[type];
                if (!gen.Loaded) gen.Load();
                return gen;
            }
        }

        public GeneratorCollection()
        {
            _generators = new Dictionary<string, T>();
        }

        public void Add(XElement elem)
        {
            var info = new T();
            info.Initialize(this, elem);
            _generators.Add(info.Name, info);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _generators.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _generators.Values.GetEnumerator();
        }
    }
}
