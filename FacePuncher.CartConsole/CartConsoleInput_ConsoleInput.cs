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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeyCode = SDL2.SDL.SDL_Keycode;

using SDL = SDL2.SDL;
using SDLi = SDL2.SDL_image;
using SDLm = SDL2.SDL_mixer;

using SDLClr = SDL2.SDL.SDL_Color;
using CONCLR = SDL2.SDL.SDL_Color;

namespace FacePuncher.CartConsole
{
	public struct CartConsoleInput
	{
		public bool Ctrl, Shift, Alt;
		public char Chr;
		public KeyCode Key;

		public char KeyChar
		{
			get
			{
				return Chr;
			}
		}

		public CartConsoleInput(char Chr, KeyCode Key, bool Ctrl = false, bool Shift = false, bool Alt = false)
		{
			this.Chr = Chr;
			this.Key = Key;
			this.Ctrl = Ctrl;
			this.Alt = Alt;
			this.Shift = Shift;
		}
	}

	public static class CartConsoleInputConverters
	{
		public static ConsoleKeyInfo ToConsoleKeyInfo(this CartConsoleInput CCI)
		{
			ConsoleKey CKey = ConsoleKey.NoName;

			switch (CCI.Key) {
				case SDL.SDL_Keycode.SDLK_0:
					CKey = ConsoleKey.D0;
					break;
				case SDL.SDL_Keycode.SDLK_1:
					CKey = ConsoleKey.D1;
					break;
				case SDL.SDL_Keycode.SDLK_2:
					CKey = ConsoleKey.D2;
					break;
				case SDL.SDL_Keycode.SDLK_3:
					CKey = ConsoleKey.D3;
					break;
				case SDL.SDL_Keycode.SDLK_4:
					CKey = ConsoleKey.D4;
					break;
				case SDL.SDL_Keycode.SDLK_5:
					CKey = ConsoleKey.D5;
					break;
				case SDL.SDL_Keycode.SDLK_6:
					CKey = ConsoleKey.D6;
					break;
				case SDL.SDL_Keycode.SDLK_7:
					CKey = ConsoleKey.D7;
					break;
				case SDL.SDL_Keycode.SDLK_8:
					CKey = ConsoleKey.D8;
					break;
				case SDL.SDL_Keycode.SDLK_9:
					CKey = ConsoleKey.D9;
					break;
				case SDL.SDL_Keycode.SDLK_APPLICATION:
					CKey = ConsoleKey.Applications;
					break;
				case SDL.SDL_Keycode.SDLK_AUDIOMUTE:
					CKey = ConsoleKey.VolumeMute;
					break;
				case SDL.SDL_Keycode.SDLK_AUDIONEXT:
					CKey = ConsoleKey.MediaNext;
					break;
				case SDL.SDL_Keycode.SDLK_AUDIOPLAY:
					CKey = ConsoleKey.MediaPlay;
					break;
				case SDL.SDL_Keycode.SDLK_AUDIOPREV:
					CKey = ConsoleKey.MediaPrevious;
					break;
				case SDL.SDL_Keycode.SDLK_AUDIOSTOP:
					CKey = ConsoleKey.MediaStop;
					break;
				case SDL.SDL_Keycode.SDLK_BACKSPACE:
					CKey = ConsoleKey.Backspace;
					break;
				case SDL.SDL_Keycode.SDLK_CANCEL:
					CKey = ConsoleKey.Escape;
					break;
				case SDL.SDL_Keycode.SDLK_CLEAR:
					CKey = ConsoleKey.Clear;
					break;
				case SDL.SDL_Keycode.SDLK_DELETE:
					CKey = ConsoleKey.Delete;
					break;
				case SDL.SDL_Keycode.SDLK_DOWN:
					CKey = ConsoleKey.DownArrow;
					break;
				case SDL.SDL_Keycode.SDLK_END:
					CKey = ConsoleKey.End;
					break;
				case SDL.SDL_Keycode.SDLK_ESCAPE:
					CKey = ConsoleKey.Escape;
					break;
				case SDL.SDL_Keycode.SDLK_EXSEL:
					CKey = ConsoleKey.ExSel;
					break;
				case SDL.SDL_Keycode.SDLK_F1:
					CKey = ConsoleKey.F1;
					break;
				case SDL.SDL_Keycode.SDLK_F10:
					CKey = ConsoleKey.F10;
					break;
				case SDL.SDL_Keycode.SDLK_F11:
					CKey = ConsoleKey.F11;
					break;
				case SDL.SDL_Keycode.SDLK_F12:
					CKey = ConsoleKey.F12;
					break;
				case SDL.SDL_Keycode.SDLK_F13:
					CKey = ConsoleKey.F13;
					break;
				case SDL.SDL_Keycode.SDLK_F14:
					CKey = ConsoleKey.F14;
					break;
				case SDL.SDL_Keycode.SDLK_F15:
					CKey = ConsoleKey.F15;
					break;
				case SDL.SDL_Keycode.SDLK_F16:
					CKey = ConsoleKey.F16;
					break;
				case SDL.SDL_Keycode.SDLK_F17:
					CKey = ConsoleKey.F17;
					break;
				case SDL.SDL_Keycode.SDLK_F18:
					CKey = ConsoleKey.F18;
					break;
				case SDL.SDL_Keycode.SDLK_F19:
					CKey = ConsoleKey.F19;
					break;
				case SDL.SDL_Keycode.SDLK_F2:
					CKey = ConsoleKey.F2;
					break;
				case SDL.SDL_Keycode.SDLK_F20:
					CKey = ConsoleKey.F20;
					break;
				case SDL.SDL_Keycode.SDLK_F21:
					CKey = ConsoleKey.F21;
					break;
				case SDL.SDL_Keycode.SDLK_F22:
					CKey = ConsoleKey.F22;
					break;
				case SDL.SDL_Keycode.SDLK_F23:
					CKey = ConsoleKey.F23;
					break;
				case SDL.SDL_Keycode.SDLK_F24:
					CKey = ConsoleKey.F24;
					break;
				case SDL.SDL_Keycode.SDLK_F3:
					CKey = ConsoleKey.F3;
					break;
				case SDL.SDL_Keycode.SDLK_F4:
					CKey = ConsoleKey.F4;
					break;
				case SDL.SDL_Keycode.SDLK_F5:
					CKey = ConsoleKey.F5;
					break;
				case SDL.SDL_Keycode.SDLK_F6:
					CKey = ConsoleKey.F6;
					break;
				case SDL.SDL_Keycode.SDLK_F7:
					CKey = ConsoleKey.F7;
					break;
				case SDL.SDL_Keycode.SDLK_F8:
					CKey = ConsoleKey.F8;
					break;
				case SDL.SDL_Keycode.SDLK_F9:
					CKey = ConsoleKey.F9;
					break;
				case SDL.SDL_Keycode.SDLK_HOME:
					CKey = ConsoleKey.Home;
					break;
				case SDL.SDL_Keycode.SDLK_INSERT:
					CKey = ConsoleKey.Insert;
					break;
				case SDL.SDL_Keycode.SDLK_KP_0:
				case SDL.SDL_Keycode.SDLK_KP_00:
				case SDL.SDL_Keycode.SDLK_KP_000:
					CKey = ConsoleKey.NumPad0;
					break;
				case SDL.SDL_Keycode.SDLK_KP_1:
					CKey = ConsoleKey.NumPad1;
					break;
				case SDL.SDL_Keycode.SDLK_KP_2:
					CKey = ConsoleKey.NumPad2;
					break;
				case SDL.SDL_Keycode.SDLK_KP_3:
					CKey = ConsoleKey.NumPad3;
					break;
				case SDL.SDL_Keycode.SDLK_KP_4:
					CKey = ConsoleKey.NumPad4;
					break;
				case SDL.SDL_Keycode.SDLK_KP_5:
					CKey = ConsoleKey.NumPad5;
					break;
				case SDL.SDL_Keycode.SDLK_KP_6:
					CKey = ConsoleKey.NumPad6;
					break;
				case SDL.SDL_Keycode.SDLK_KP_7:
					CKey = ConsoleKey.NumPad7;
					break;
				case SDL.SDL_Keycode.SDLK_KP_8:
					CKey = ConsoleKey.NumPad8;
					break;
				case SDL.SDL_Keycode.SDLK_KP_9:
					CKey = ConsoleKey.NumPad9;
					break;
				case SDL.SDL_Keycode.SDLK_KP_BACKSPACE:
					CKey = ConsoleKey.Backspace;
					break;
				case SDL.SDL_Keycode.SDLK_KP_DIVIDE:
					CKey = ConsoleKey.Divide;
					break;
				case SDL.SDL_Keycode.SDLK_KP_ENTER:
					CKey = ConsoleKey.Enter;
					break;
				case SDL.SDL_Keycode.SDLK_KP_MINUS:
					CKey = ConsoleKey.Subtract;
					break;
				case SDL.SDL_Keycode.SDLK_KP_MULTIPLY:
					CKey = ConsoleKey.Multiply;
					break;
				case SDL.SDL_Keycode.SDLK_KP_PERIOD:
					CKey = ConsoleKey.OemPeriod;
					break;
				case SDL.SDL_Keycode.SDLK_KP_PLUS:
					CKey = ConsoleKey.Add;
					break;
				case SDL.SDL_Keycode.SDLK_KP_SPACE:
					CKey = ConsoleKey.Spacebar;
					break;
				case SDL.SDL_Keycode.SDLK_KP_TAB:
					CKey = ConsoleKey.Tab;
					break;
				case SDL.SDL_Keycode.SDLK_LEFT:
					CKey = ConsoleKey.LeftArrow;
					break;
				case SDL.SDL_Keycode.SDLK_MINUS:
					CKey = ConsoleKey.Subtract;
					break;
				case SDL.SDL_Keycode.SDLK_MUTE:
					CKey = ConsoleKey.VolumeMute;
					break;
				case SDL.SDL_Keycode.SDLK_PAGEDOWN:
					CKey = ConsoleKey.PageDown;
					break;
				case SDL.SDL_Keycode.SDLK_PAGEUP:
					CKey = ConsoleKey.PageUp;
					break;
				case SDL.SDL_Keycode.SDLK_PAUSE:
					CKey = ConsoleKey.Pause;
					break;
				case SDL.SDL_Keycode.SDLK_PERIOD:
					CKey = ConsoleKey.OemPeriod;
					break;
				case SDL.SDL_Keycode.SDLK_PLUS:
					CKey = ConsoleKey.Add;
					break;
				case SDL.SDL_Keycode.SDLK_PRINTSCREEN:
					CKey = ConsoleKey.PrintScreen;
					break;
				case SDL.SDL_Keycode.SDLK_RETURN:
					CKey = ConsoleKey.Enter;
					break;
				case SDL.SDL_Keycode.SDLK_RETURN2:
					CKey = ConsoleKey.Enter;
					break;
				case SDL.SDL_Keycode.SDLK_RIGHT:
					CKey = ConsoleKey.RightArrow;
					break;
				case SDL.SDL_Keycode.SDLK_SPACE:
					CKey = ConsoleKey.Spacebar;
					break;
				case SDL.SDL_Keycode.SDLK_TAB:
					CKey = ConsoleKey.Tab;
					break;
				case SDL.SDL_Keycode.SDLK_UNDO:
					CKey = ConsoleKey.Backspace;
					break;
				case SDL.SDL_Keycode.SDLK_UP:
					CKey = ConsoleKey.UpArrow;
					break;
				case SDL.SDL_Keycode.SDLK_VOLUMEDOWN:
					CKey = ConsoleKey.VolumeDown;
					break;
				case SDL.SDL_Keycode.SDLK_VOLUMEUP:
					CKey = ConsoleKey.VolumeUp;
					break;
				case SDL.SDL_Keycode.SDLK_a:
					CKey = ConsoleKey.A;
					break;
				case SDL.SDL_Keycode.SDLK_b:
					CKey = ConsoleKey.B;
					break;
				case SDL.SDL_Keycode.SDLK_c:
					CKey = ConsoleKey.C;
					break;
				case SDL.SDL_Keycode.SDLK_d:
					CKey = ConsoleKey.D;
					break;
				case SDL.SDL_Keycode.SDLK_e:
					CKey = ConsoleKey.E;
					break;
				case SDL.SDL_Keycode.SDLK_f:
					CKey = ConsoleKey.F;
					break;
				case SDL.SDL_Keycode.SDLK_g:
					CKey = ConsoleKey.G;
					break;
				case SDL.SDL_Keycode.SDLK_h:
					CKey = ConsoleKey.H;
					break;
				case SDL.SDL_Keycode.SDLK_i:
					CKey = ConsoleKey.I;
					break;
				case SDL.SDL_Keycode.SDLK_j:
					CKey = ConsoleKey.J;
					break;
				case SDL.SDL_Keycode.SDLK_k:
					CKey = ConsoleKey.K;
					break;
				case SDL.SDL_Keycode.SDLK_l:
					CKey = ConsoleKey.L;
					break;
				case SDL.SDL_Keycode.SDLK_m:
					CKey = ConsoleKey.M;
					break;
				case SDL.SDL_Keycode.SDLK_n:
					CKey = ConsoleKey.N;
					break;
				case SDL.SDL_Keycode.SDLK_o:
					CKey = ConsoleKey.O;
					break;
				case SDL.SDL_Keycode.SDLK_p:
					CKey = ConsoleKey.P;
					break;
				case SDL.SDL_Keycode.SDLK_q:
					CKey = ConsoleKey.Q;
					break;
				case SDL.SDL_Keycode.SDLK_r:
					CKey = ConsoleKey.R;
					break;
				case SDL.SDL_Keycode.SDLK_s:
					CKey = ConsoleKey.S;
					break;
				case SDL.SDL_Keycode.SDLK_t:
					CKey = ConsoleKey.T;
					break;
				case SDL.SDL_Keycode.SDLK_u:
					CKey = ConsoleKey.U;
					break;
				case SDL.SDL_Keycode.SDLK_v:
					CKey = ConsoleKey.V;
					break;
				case SDL.SDL_Keycode.SDLK_w:
					CKey = ConsoleKey.W;
					break;
				case SDL.SDL_Keycode.SDLK_x:
					CKey = ConsoleKey.X;
					break;
				case SDL.SDL_Keycode.SDLK_y:
					CKey = ConsoleKey.Y;
					break;
				case SDL.SDL_Keycode.SDLK_z:
					CKey = ConsoleKey.Z;
					break;
				default:
					throw new Exception("Unknown key: " + CCI.Key.ToString());
			}

			return new ConsoleKeyInfo(CCI.Chr, CKey, CCI.Shift, CCI.Alt, CCI.Ctrl);
		}
	}
}