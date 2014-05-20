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

using FacePuncher.CartConsole;
using Console = FacePuncher.CartConsole.CartConsole;
using ConsoleKey = SDL2.SDL.SDL_Keycode;

namespace FacePuncher
{
	static class Input
	{
		private static T ReadKey<T>(Dictionary<ConsoleKey, T> keyMap)
		{
			T result;
			while (!TryReadKey(keyMap, out result))
				;
			return result;
		}

		private static bool TryReadKey<T>(Dictionary<ConsoleKey, T> keyMap, out T result)
		{
			result = default(T);

			if (!Console.KeyAvailable)
				return false;

			ConsoleKey key = Console.ReadKey(intercept: true).Key;
			if (keyMap.ContainsKey(key)) {
				result = keyMap[key];
				return true;
			}

			return false;
		}

		public static readonly Dictionary<ConsoleKey, Direction> MovementKeys
			= new Dictionary<ConsoleKey, Direction> {
			{ ConsoleKey.SDLK_KP_7, Direction.NorthWest },
			{ ConsoleKey.SDLK_KP_8, Direction.North },
			{ ConsoleKey.SDLK_UP, Direction.North },
			{ ConsoleKey.SDLK_KP_9, Direction.NorthEast },
			{ ConsoleKey.SDLK_KP_4, Direction.West },
			{ ConsoleKey.SDLK_LEFT, Direction.West },
			{ ConsoleKey.SDLK_KP_5, Direction.None },
			{ ConsoleKey.SDLK_KP_6, Direction.East },
			{ ConsoleKey.SDLK_RIGHT, Direction.East },
			{ ConsoleKey.SDLK_KP_1, Direction.SouthWest },
			{ ConsoleKey.SDLK_KP_2, Direction.South },
			{ ConsoleKey.SDLK_DOWN, Direction.South },
			{ ConsoleKey.SDLK_KP_3, Direction.SouthEast }
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
			{ ConsoleKey.SDLK_w, UINavigation.Up },
			{ ConsoleKey.SDLK_UP, UINavigation.Up },
			{ ConsoleKey.SDLK_KP_8, UINavigation.Up },
			{ ConsoleKey.SDLK_KP_MINUS, UINavigation.Up },
			{ ConsoleKey.SDLK_s, UINavigation.Down },
			{ ConsoleKey.SDLK_DOWN, UINavigation.Down },
			{ ConsoleKey.SDLK_KP_2, UINavigation.Down },
			{ ConsoleKey.SDLK_KP_PLUS, UINavigation.Down },
			{ ConsoleKey.SDLK_RETURN, UINavigation.Select }
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