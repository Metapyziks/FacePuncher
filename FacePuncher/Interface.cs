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
using System.Reflection;
using System.Xml.Linq;

using FacePuncher.Geometry;
using FacePuncher.UI;

namespace FacePuncher
{
    public class Interface
    {
        public static bool Loaded { get { return Display != null; } }
        
        static Interface()
        {
            Definitions.RegisterType("interface", elem => {
                if (Loaded) return;
                if (!elem.HasAttribute("active")) return;
                if (!elem.Attribute("active").Value.ToLower().Equals("true")) return;

                var path = elem.Attribute("dll").Value;
                var asm = Assembly.LoadFrom(path);

                var disp = elem.Element("Display");
                var inpt = elem.Element("Input");

                Display = (Display) asm.CreateInstance(disp.Attribute("class").Value);
                Input = (Input) asm.CreateInstance(inpt.Attribute("class").Value);

                Display.OnLoadFromDefinition(disp);
                Input.OnLoadFromDefinition(inpt);
            });
        }

        public static Display Display { get; private set; }

        public static Input Input { get; private set; }

        protected virtual void OnLoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);
        }
    }

    public abstract class Display : Interface
    {
        public Rectangle Rect { get; private set; }

        /// <summary>
        /// Horizontal size of the display in characters.
        /// </summary>
        public int Width { get { return Rect.Width; } }

        /// <summary>
        /// Vertical size of the display in characters.
        /// </summary>
        public int Height { get { return Rect.Height; } }

        /// <summary>
        /// Position of the center of the display.
        /// </summary>
        public Position Center { get { return new Position(Width / 2, Height / 2); } }

        /// <summary>
        /// Prepare the display for rendering.
        /// </summary>
        /// <param name="width">Desired horizontal size of the display in characters.</param>
        /// <param name="height">Desired vertical size of the display in characters.</param>
        public void Initialize(int width, int height)
        {
            Rect = new Rectangle(0, 0, width, height);

            OnInitialize(width, height);
            Clear();
        }

        protected abstract void OnInitialize(int width, int height);

        /// <summary>
        /// Wipe the display buffer.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Set a specific character in the display buffer.
        /// </summary>
        /// <param name="x">Horizontal position of the character.</param>
        /// <param name="y">Vertical position of the character.</param>
        /// <param name="symbol">Character to display.</param>
        /// <param name="fore">Foreground color of the character.</param>
        /// <param name="back">Background color of the character.</param>
        public abstract void SetCell(int x, int y, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black);

        /// <summary>
        /// Set a specific character in the display buffer.
        /// </summary>
        /// <param name="pos">Position of the character.</param>
        /// <param name="symbol">Character to display.</param>
        /// <param name="fore">Foreground color of the character.</param>
        /// <param name="back">Background color of the character.</param>
        public void SetCell(Position pos, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            SetCell(pos.X, pos.Y, symbol, fore, back);
        }

        public abstract void Write(int x, int y, String text);

        /// <summary>
        /// Send the display buffer to the display window.
        /// </summary>
        public abstract void Refresh();
    }

    public abstract class Input : Interface
    {
        protected Dictionary<TKey, TBind> ReadKeyBindings<TKey, TBind>(XElement elem)
        {
            if (!typeof(TKey).IsEnum || !typeof(TBind).IsEnum) {
                throw new ArgumentException("Enum type arguments expected.");
            }

            var dict = new Dictionary<TKey, TBind>();

            foreach (var sub in elem.Elements("Key")) {
                var key = (TKey) Enum.Parse(typeof(TKey), sub.Attribute("name").Value);
                var bind = (TBind) Enum.Parse(typeof(TBind), sub.Value);

                dict.Add(key, bind);
            }

            return dict;
        }

        public abstract Direction ReadMovement();

        public abstract bool TryReadMovement(out Direction result);

        public abstract UINavigation ReadUINavigation();

        public abstract bool TryReadUINavigation(out UINavigation result);

        // TODO: Replace ConsoleKeyInfo with bespoke structure?
        public abstract ConsoleKeyInfo ReadKey();

        public abstract bool TryReadKey(out ConsoleKeyInfo result);
    }
}
