using System;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    class GeneratorInfo
    {
        public String Name { get; private set; }

        public String BaseName { get; private set; }

        public bool HasBase { get { return BaseName != null; } }

        public void Initialize(XElement elem)
        {
            Name = elem.Attribute("name").Value;
            BaseName = elem.HasAttribute("base")
                ? elem.Attribute("base").Value : null;

            OnLoadFromDefinition(elem);
        }

        protected abstract void OnLoadFromDefinition(XElement elem);
    }
}
