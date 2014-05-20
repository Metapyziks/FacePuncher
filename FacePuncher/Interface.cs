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
using System.Reflection;

namespace FacePuncher
{
    public static class Interface
    {
        public static bool Loaded { get { return Display != null; } }
        
        static Interface()
        {
            Definitions.RegisterType("interface", elem => {
                if (Loaded) return;

                var path = elem.Attribute("dll").Value;
                var asm = Assembly.LoadFrom(path);

                Display = (Display) asm.CreateInstance(elem.Element("Display").Value);
                Input = (Input) asm.CreateInstance(elem.Element("Input").Value);
            });
        }

        public static Display Display { get; private set; }

        public static Input Input { get; private set; }
    }
}
