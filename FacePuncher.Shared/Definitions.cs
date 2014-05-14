using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FacePuncher.Entities;

namespace FacePuncher
{
    public enum DefinitionsNamespace
    {
        Client = 1,
        Server = 2,
        Shared = 3
    }

    public static class Definitions
    {
        private static Dictionary<String, Action<XElement>> _typeHandlers
            = new Dictionary<string, Action<XElement>>();

        private static Dictionary<String, List<XElement>> _unhandled
            = new Dictionary<string,List<XElement>>();

        static Definitions()
        {
            RegisterType("entity", elem => {
                var components = new List<Tuple<Type, XElement>>();

                foreach (var sub in elem.Elements()) {
                    var typeName = String.Format("FacePuncher.Entities.{0}", sub.Name.LocalName);
                    var type = Assembly.GetEntryAssembly().GetType(typeName);

                    if (type == null) continue;

                    components.Add(Tuple.Create(type, sub));
                }

                EntityBuilderDelegate ctor = ent => {
                    foreach (var type in components) {
                        var comp = ent.GetComponentOrNull(type.Item1)
                            ?? ent.AddComponent(type.Item1);
                        
                        comp.LoadFromDefinition(type.Item2);
                    }
                };

                if (elem.Attributes("base").Count() > 0) {
                    Entity.Register(elem.Attribute("name").Value, elem.Attribute("base").Value, ctor);
                } else {
                    Entity.Register(elem.Attribute("name").Value, ctor);
                }
            });
        }

        public static void RegisterType(String name, Action<XElement> handler)
        {
            _typeHandlers.Add(name, handler);

            if (_unhandled.ContainsKey(name)) {
                foreach (var elem in _unhandled[name]) {
                    handler(elem);
                }

                _unhandled[name].Clear();
            }
        }

        static void PurgeNamespace(XElement elem, XNamespace ns)
        {
            var invalidAttribs = elem.Attributes()
                .Where(x => x.Name.Namespace == ns)
                .ToArray();

            foreach (var attrib in invalidAttribs) {
                attrib.Remove();
            }

            var invalidElems = elem.Elements()
                .Where(x => x.Name.Namespace == ns)
                .ToArray();

            foreach (var subElem in invalidElems) {
                subElem.Remove();
            }

            foreach (var subElem in elem.Elements()) {
                PurgeNamespace(subElem, ns);
            }
        }

        public static void Load(String path, DefinitionsNamespace ns)
        {
            var xml = File.ReadAllText(path);
            
            var server = (XNamespace) "/server";
            var client = (XNamespace) "/client";

            xml = String.Format("<definitions xmlns:server=\"{0}\" "
                + "xmlns:client=\"{1}\">{2}{3}{2}</definitions>",
                server, client, Environment.NewLine, xml);

            var definitions = XDocument.Parse(xml).Element("definitions");

            foreach (var elem in definitions.Elements()) {
                if (elem.Name.Namespace == server && !ns.HasFlag(DefinitionsNamespace.Server)) {
                    continue;
                }

                if (elem.Name.Namespace == client && !ns.HasFlag(DefinitionsNamespace.Client)) {
                    continue;
                }

                if (!ns.HasFlag(DefinitionsNamespace.Server)) {
                    PurgeNamespace(elem, server);
                }

                if (!ns.HasFlag(DefinitionsNamespace.Client)) {
                    PurgeNamespace(elem, client);
                }

                var name = elem.Name.LocalName;
                if (_typeHandlers.ContainsKey(name)) {
                    _typeHandlers[name](elem);
                } else {
                    if (!_unhandled.ContainsKey(name)) {
                        _unhandled.Add(name, new List<XElement>());
                    }

                    _unhandled[name].Add(elem);
                }
            }
        }
    }
}
