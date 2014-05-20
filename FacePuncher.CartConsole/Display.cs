/* Copyright (C) 2014 Saša Barišić (cartman300@net.hr)
 * Copyright (C) 2014 James King (metapyziks@gmail.com)
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
using System.Drawing;

namespace FacePuncher.CartConsole
{
    public class Display : FacePuncher.Display
    {
        private Color[] _palette;

        [ScriptDefinable]
        public String Tileset { get; set; }

        public Display()
        {
            Tileset = "font.png";

            _palette = new Color[16];
        }

        protected override void OnLoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            base.OnLoadFromDefinition(elem);

            if (elem.HasElement("Palette")) {
                foreach (var clr in elem.Element("Palette").Elements("Color")) {
                    int index;
                    if (!clr.HasAttribute("index") ||
                        !int.TryParse(clr.Attribute("index").Value, out index) ||
                        index < 0 ||
                        index >= 16) continue;

                    int rgb;
                    if (clr.HasAttribute("name")) {
                        _palette[index] = Color.FromName(clr.Attribute("name").Value);
                    } else if (int.TryParse(clr.Attribute("rgb").Value, System.Globalization.NumberStyles.HexNumber, null, out rgb)) {
                        _palette[index] = Color.FromArgb((rgb >> 16) & 0xff, (rgb >> 8) & 0xff, rgb & 0xff);
                    } else {
                        throw new Exception(String.Format("Bad color #{0} in palette.", index));
                    }
                }
            }
        }

        protected override void OnInitialize(int width, int height)
        {
            CartConsole.Initialize(String.Format("Data/{0}", Tileset));
            CartConsole.Title = "FacePuncher";

            CartConsole.SetSize(width, height);
            CartConsole.FontWatcher();
        }

        public override void Clear()
        {
            CartConsole.Clear();
        }

        public override void SetCell(int x, int y, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            CartConsole.Set(x, y, symbol, _palette[(int) fore], _palette[(int) back]);
        }

        public override void Write(int x, int y, string text)
        {
            CartConsole.Write(x, y, text, Color.Gray, Color.Black);
        }

        public override void Refresh()
        {
            CartConsole.Refresh();
        }
    }
}
