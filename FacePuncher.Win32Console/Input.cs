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
using System.Xml.Linq;

using FacePuncher.Geometry;
using FacePuncher.UI;

namespace FacePuncher.Win32Console
{
    public class Input : FacePuncher.Input
    {
        private Dictionary<ConsoleKey, Direction> _movementKeys;
        private Dictionary<ConsoleKey, UINavigation> _uiNavigationKeys;

        protected override void OnLoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            base.OnLoadFromDefinition(elem);

            if (elem.HasElement("MovementKeys")) {
                _movementKeys = ReadKeyBindings<ConsoleKey, Direction>(elem.Element("MovementKeys"));
            }

            if (elem.HasElement("UINavigationKeys")) {
                _uiNavigationKeys = ReadKeyBindings<ConsoleKey, UINavigation>(elem.Element("UINavigationKeys"));
            }
        }

        private static T ReadKey<T>(Dictionary<ConsoleKey, T> keyMap)
        {
            T result;
            while (!TryReadKey(keyMap, out result)) ;
            return result;
        }

        private static bool TryReadKey<T>(Dictionary<ConsoleKey, T> keyMap, out T result)
        {
            result = default(T);

            if (!Console.KeyAvailable) return false;

            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            if (keyMap.ContainsKey(key)) {
                result = keyMap[key];
                return true;
            }

            return false;
        }
        
        public override Direction ReadMovement()
        {
            return ReadKey(_movementKeys);
        }

        public override bool TryReadMovement(out Direction result)
        {
            return TryReadKey(_movementKeys, out result);
        }

        public override UINavigation ReadUINavigation()
        {
            return ReadKey(_uiNavigationKeys);
        }

        public override bool TryReadUINavigation(out UINavigation result)
        {
            return TryReadKey(_uiNavigationKeys, out result);
        }

        public override ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey(intercept: true);
        }

        public override bool TryReadKey(out ConsoleKeyInfo result)
        {
            result = default(ConsoleKeyInfo);

            if (!Console.KeyAvailable) return false;

            result = Console.ReadKey(intercept: true);

            return true;
        }
    }
}
