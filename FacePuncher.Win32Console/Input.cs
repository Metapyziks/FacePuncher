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

namespace FacePuncher.Win32Console
{
    public class Input : FacePuncher.Input
    {
        private static readonly Dictionary<ConsoleKey, Direction> _sMovementKeys
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
        
        private static readonly Dictionary<ConsoleKey, UINavigation> _sNavigationKeys
            = new Dictionary<ConsoleKey, UINavigation> {
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
            return ReadKey(_sMovementKeys);
        }

        public override bool TryReadMovement(out Direction result)
        {
            return TryReadKey(_sMovementKeys, out result);
        }

        public override UINavigation ReadUINavigation()
        {
            return ReadKey(_sNavigationKeys);
        }

        public override bool TryReadUINavigation(out UINavigation result)
        {
            return TryReadKey(_sNavigationKeys, out result);
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
