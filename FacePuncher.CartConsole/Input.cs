/* Copyright (C) 2014 Saša Barišiæ (cartman300@net.hr)
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

        private static readonly Dictionary<SDL.SDL_Keycode, Direction> _sMovementKeys
			= new Dictionary<SDL.SDL_Keycode, Direction> {
			{ SDL.SDL_Keycode.SDLK_KP_7, Direction.NorthWest },
			{ SDL.SDL_Keycode.SDLK_KP_8, Direction.North },
			{ SDL.SDL_Keycode.SDLK_UP, Direction.North },
			{ SDL.SDL_Keycode.SDLK_KP_9, Direction.NorthEast },
			{ SDL.SDL_Keycode.SDLK_KP_4, Direction.West },
			{ SDL.SDL_Keycode.SDLK_LEFT, Direction.West },
			{ SDL.SDL_Keycode.SDLK_KP_5, Direction.None },
			{ SDL.SDL_Keycode.SDLK_KP_6, Direction.East },
			{ SDL.SDL_Keycode.SDLK_RIGHT, Direction.East },
			{ SDL.SDL_Keycode.SDLK_KP_1, Direction.SouthWest },
			{ SDL.SDL_Keycode.SDLK_KP_2, Direction.South },
			{ SDL.SDL_Keycode.SDLK_DOWN, Direction.South },
			{ SDL.SDL_Keycode.SDLK_KP_3, Direction.SouthEast }
		};

        private static readonly Dictionary<SDL.SDL_Keycode, UINavigation> _sNavigationKeys
            = new Dictionary<SDL.SDL_Keycode, UINavigation> {
			{ SDL.SDL_Keycode.SDLK_w, UINavigation.Up },
			{ SDL.SDL_Keycode.SDLK_UP, UINavigation.Up },
			{ SDL.SDL_Keycode.SDLK_KP_8, UINavigation.Up },
			{ SDL.SDL_Keycode.SDLK_KP_MINUS, UINavigation.Up },
			{ SDL.SDL_Keycode.SDLK_s, UINavigation.Down },
			{ SDL.SDL_Keycode.SDLK_DOWN, UINavigation.Down },
			{ SDL.SDL_Keycode.SDLK_KP_2, UINavigation.Down },
			{ SDL.SDL_Keycode.SDLK_KP_PLUS, UINavigation.Down },
			{ SDL.SDL_Keycode.SDLK_RETURN, UINavigation.Select }
		};

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
            throw new NotImplementedException();
        }

        public override bool TryReadKey(out ConsoleKeyInfo result)
        {
            throw new NotImplementedException();
        }
    }
}
