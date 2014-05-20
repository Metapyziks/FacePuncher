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
using System.Collections.Generic;

using FacePuncher.Geometry;
using FacePuncher.UI;

using SDL2;

namespace FacePuncher.CartConsole
{
	class Input : FacePuncher.Input
	{
		private Dictionary<SDL.SDL_Keycode, Direction> _movementKeys;
		private Dictionary<SDL.SDL_Keycode, UINavigation> _uiNavigationKeys;

		protected override void OnLoadFromDefinition(System.Xml.Linq.XElement elem)
		{
			base.OnLoadFromDefinition(elem);

			if (elem.HasElement("MovementKeys")) {
				_movementKeys = ReadKeyBindings<SDL.SDL_Keycode, Direction>(elem.Element("MovementKeys"));
			}

			if (elem.HasElement("UINavigationKeys")) {
				_uiNavigationKeys = ReadKeyBindings<SDL.SDL_Keycode, UINavigation>(elem.Element("UINavigationKeys"));
			}
		}

		private static T ReadKey<T>(Dictionary<SDL.SDL_Keycode, T> keyMap)
		{
			T result;
			while (!TryReadKey(keyMap, out result))
				;
			return result;
		}

		private static bool TryReadKey<T>(Dictionary<SDL.SDL_Keycode, T> keyMap, out T result)
		{
			result = default(T);

			if (!CartConsole.KeyAvailable)
				return false;

			SDL.SDL_Keycode key = CartConsole.ReadKey(intercept: true).Key;
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
			return CartConsole.ReadKey().ToConsoleKeyInfo();
		}

		public override bool TryReadKey(out ConsoleKeyInfo result)
		{
			if (CartConsole.KeyAvailable == false) {
				result = new ConsoleKeyInfo();
				return false;
			}
			result = ReadKey();
			return true;
		}
	}
}