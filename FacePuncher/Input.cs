/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Saša Barišić (cartman300@net.hr)
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

using SdlDotNet;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.Windows;

using ConsoleKey = SdlDotNet.Input.Key;

namespace FacePuncher
{
    static class Input
    {
        static Queue<ConsoleKey> Keys;

        public static void Initialize()
        {
            Keys = new Queue<ConsoleKey>();
            Events.KeyboardDown += KeyEvent;
            Events.KeyboardUp += KeyEvent;
        }

        private static void KeyEvent(object S, KeyboardEventArgs E)
        {
            if (E.Down)
                Keys.Enqueue(E.Key);
        }

        public static ConsoleKey SDLReadKey()
        {
            while (Keys.Count < 1)
                ;
            return Keys.Dequeue();
        }

        private static T ReadKey<T>(Dictionary<ConsoleKey, T> keyMap)
        {
            T result;
            while (!TryReadKey(keyMap, out result)) ;
            return result;
        }

        private static bool TryReadKey<T>(Dictionary<ConsoleKey, T> keyMap, out T result)
        {
            //ConsoleKey key = Console.ReadKey(intercept: true).Key;
            ConsoleKey key = SDLReadKey();
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
            { ConsoleKey.Keypad7, Direction.NorthWest },
            { ConsoleKey.Keypad8, Direction.North },
            { ConsoleKey.UpArrow, Direction.North },
            { ConsoleKey.Keypad9, Direction.NorthEast },
            { ConsoleKey.Keypad4, Direction.West },
            { ConsoleKey.LeftArrow, Direction.West },
            { ConsoleKey.Keypad5, Direction.None },
            { ConsoleKey.Keypad6, Direction.East },
            { ConsoleKey.RightArrow, Direction.East },
            { ConsoleKey.Keypad1, Direction.SouthWest },
            { ConsoleKey.Keypad2, Direction.South },
            { ConsoleKey.DownArrow, Direction.South },
            { ConsoleKey.Keypad3, Direction.SouthEast }
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
            { ConsoleKey.Keypad8, UINavigation.Up },
            { ConsoleKey.KeypadMinus, UINavigation.Up },

            { ConsoleKey.S, UINavigation.Down },
            { ConsoleKey.DownArrow, UINavigation.Down },
            { ConsoleKey.Keypad2, UINavigation.Down },
            { ConsoleKey.KeypadPlus, UINavigation.Down },

            { ConsoleKey.Return, UINavigation.Select }
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
