using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using FacePuncher.Entities;

namespace FacePuncher
{
    /// <summary>
    /// Used to specify which definition entries should be
    /// accepted when loading from files with mixed server
    /// and client definitions.
    /// </summary>
    public enum DefinitionsNamespace
    {
        /// <summary>
        /// Accept definitions for clients.
        /// </summary>
        Client = 1,

        /// <summary>
        /// Accept definitions for servers.
        /// </summary>
        Server = 2,

        /// <summary>
        /// Accept all definitions (why would you need this?).
        /// </summary>
        Shared = 3
    }

    /// <summary>
    /// Utility class for loading and handling definitions
    /// loaded from XML files.
    /// </summary>
    public static class Definitions
    {
        /// <summary>
        /// Dictionary of definition type names mapped to their handlers.
        /// </summary>
        private static Dictionary<String, Action<XElement>> _typeHandlers
            = new Dictionary<string, Action<XElement>>();

        /// <summary>
        /// Buffer for unhandled definitions.
        /// </summary>
        private static Dictionary<String, List<XElement>> _unhandled
            = new Dictionary<string,List<XElement>>();

        /// <summary>
        /// Static constructor for Definitions.
        /// </summary>
        static Definitions()
        {
            // Handler for entity definitions.
            RegisterType("entity", elem => {
                var components = new List<Tuple<Type, XElement>>();

                // Find all components specified by the definition.
                foreach (var sub in elem.Elements()) {
                    var typeName = String.Format("FacePuncher.Entities.{0}", sub.Name.LocalName);
                    var type = Assembly.GetEntryAssembly().GetType(typeName);

                    // Make sure the component actually exists.
                    if (type == null) continue;

                    // Record the component along with its element.
                    components.Add(Tuple.Create(type, sub));
                }

                // Entity constructor.
                EntityBuilderDelegate ctor = ent => {
                    foreach (var type in components) {
                        // For each component type, either get and existing
                        // component instance or create one.
                        var comp = ent.GetComponentOrNull(type.Item1)
                            ?? ent.AddComponent(type.Item1);
                        
                        // Initialize the component with the XML element
                        // loaded from the definition file.
                        comp.LoadFromDefinition(type.Item2);
                    }
                };

                // Get an array of component types to be used to find entity
                // classes that contain certain components.
                var compTypes = components.Select(x => x.Item1).ToArray();

                // Register the entity class.
                if (elem.Attributes("base").Count() > 0) {
                    Entity.Register(elem.Attribute("name").Value, elem.Attribute("base").Value, ctor, compTypes);
                } else {
                    Entity.Register(elem.Attribute("name").Value, ctor, compTypes);
                }
            });
        }

        /// <summary>
        /// Attaches a handler to be invoked on definition elements
        /// of the specified type.
        /// </summary>
        /// <param name="name">Definition type name to register.</param>
        /// <param name="handler">Hander to be invoked on elements of
        /// the specified type.</param>
        public static void RegisterType(String name, Action<XElement> handler)
        {
            _typeHandlers.Add(name, handler);

            // Check to see if any unhandled elements of this type are waiting.
            if (_unhandled.ContainsKey(name)) {
                foreach (var elem in _unhandled[name]) {
                    handler(elem);
                }

                // Clear the newly handled elements.
                _unhandled[name].Clear();
            }
        }

        /// <summary>
        /// Removes all attributes and elements from the specified
        /// element recursively that have the given namespace.
        /// </summary>
        /// <param name="elem">Parent element to cull the children of.</param>
        /// <param name="ns">Namespace to purge.</param>
        static void PurgeNamespace(XElement elem, XNamespace ns)
        {
            // Find attributes that use the given namespace.
            var invalidAttribs = elem.Attributes()
                .Where(x => x.Name.Namespace == ns)
                .ToArray();

            // Remove those attributes.
            foreach (var attrib in invalidAttribs) {
                attrib.Remove();
            }

            // Now find matching elements.
            var invalidElems = elem.Elements()
                .Where(x => x.Name.Namespace == ns)
                .ToArray();

            // Kill the children.
            foreach (var subElem in invalidElems) {
                subElem.Remove();
            }

            // Now to recurse.
            foreach (var subElem in elem.Elements()) {
                PurgeNamespace(subElem, ns);
            }
        }

        /// <summary>
        /// Loads all definition files found in the specified directory,
        /// and optionally in all sub-directories too.
        /// </summary>
        /// <param name="path">Path to the directory.</param>
        /// <param name="ns">Namespace filter for definitions.</param>
        /// <param name="recursive">If true, will recurse into sub-directories.</param>
        public static void LoadFromDirectory(String path, DefinitionsNamespace ns, bool recursive = true)
        {
            // Find and load all XML files.
            foreach (var file in Directory.GetFiles(path)) {
                if (Path.GetExtension(file) != ".xml") continue;

                LoadFromFile(file, ns);
            }

            // Recurse into sub-directories if required.
            if (recursive) {
                foreach (var dir in Directory.GetDirectories(path)) {
                    LoadFromDirectory(dir, ns, recursive);
                }
            }
        }

        /// <summary>
        /// Loads all definitions found in a single XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ns"></param>
        public static void LoadFromFile(String path, DefinitionsNamespace ns)
        {
            var server = (XNamespace) "/server";
            var client = (XNamespace) "/client";

            // Wrap a root object and namespace definitions around the document.
            var xml = String.Format("<definitions xmlns:server=\"{0}\" "
                + "xmlns:client=\"{1}\">{2}{3}{2}</definitions>",
                server, client, Environment.NewLine, File.ReadAllText(path));

            XDocument doc;
            using (var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml))) {
                // Some weird characters may be used as entity symbols, so
                // don't check characters.
                var settings = new XmlReaderSettings { CheckCharacters = false };

                // Parse the file into a document object.
                using (var reader = XmlReader.Create(stream, settings)) {
                    reader.MoveToContent();
                    doc = XDocument.Load(reader);
                }
            }

            var definitions = doc.Element("definitions");

            // Remove server-specific elements if required.
            if (!ns.HasFlag(DefinitionsNamespace.Server)) {
                PurgeNamespace(definitions, server);
            }

            // Remove client-specific elements if required.
            if (!ns.HasFlag(DefinitionsNamespace.Client)) {
                PurgeNamespace(definitions, client);
            }

            foreach (var elem in definitions.Elements()) {
                var name = elem.Name.LocalName;

                // If an existing handler exists for this element type,
                // invoke it on the element.
                if (_typeHandlers.ContainsKey(name)) {
                    _typeHandlers[name](elem);

                // Otherwise, buffer it for handling later.
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
