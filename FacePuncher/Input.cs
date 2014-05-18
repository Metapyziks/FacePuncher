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

using FacePuncher.Geometry;
using FacePuncher.UI;

namespace FacePuncher
{
    static class Input
    {
        private static T ReadKey<T>(Dictionary<ConsoleKey, T> keyMap)
        {
            T result;
            while (!TryReadKey(keyMap, out result)) ;
            return result;
        }

        private static bool TryReadKey<T>(Dictionary<ConsoleKey, T> keyMap, out T result)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            if (keyMap.ContainsKey(key)) {
                result = keyMap[key];
                return true;
            } else {
                result = default(T);
                return false;
            }
        }

        public static readonly Dictionary<ConsoleKey, Direction> MovementKeys
            = new Dictionary<ConsoleKey, Direction> {
            { ConsoleKey.NumPad7, Direction.NorthWest },
            { ConsoleKey.NumPad8, Direction.North },
            { ConsoleKey.UpArrow, Direction.North },
            { ConsoleKey.NumPad9, Direction.NorthEast },
            { ConsoleKey.NumPad4, Direction.West },
            { ConsoleKey.LeftArrow, Direction.West },
            { ConsoleKey.NumPad5, Direction.None },
            { ConsoleKey.NumPad6, Direction.East },
            { ConsoleKey.RightArrow, Direction.East },
            { ConsoleKey.NumPad1, Direction.SouthWest },
            { ConsoleKey.NumPad2, Direction.South },
            { ConsoleKey.DownArrow, Direction.South },
            { ConsoleKey.NumPad3, Direction.SouthEast }
        };

        public static Direction ReadMovement()
        {
            return ReadKey(MovementKeys);
        }

        public static bool TryReadMovement(out Direction result)
        {
            return TryReadKey(MovementKeys, out result);
        }

        public static readonly Dictionary<ConsoleKey, UINavigation> NavigationKeys = new Dictionary<ConsoleKey, UINavigation> {
            { ConsoleKey.W, UINavigation.Up },
            { ConsoleKey.UpArrow, UINavigation.Up },
            { ConsoleKey.NumPad8, UINavigation.Up },
            { ConsoleKey.Subtract, UINavigation.Up },

            { ConsoleKey.S, UINavigation.Down },
            { ConsoleKey.DownArrow, UINavigation.Down },
            { ConsoleKey.NumPad2, UINavigation.Down },
            { ConsoleKey.Add, UINavigation.Down },

            { ConsoleKey.Enter, UINavigation.Select }
        };

        public static UINavigation ReadUINavigation()
        {
            return ReadKey(NavigationKeys);
        }

        public static bool TryReadUINavigation(out UINavigation result)
        {
            return TryReadKey(NavigationKeys, out result);
        }
    }
}
