/* Copyright (C) 2014 Saša Barišić (cartman300@net.hr)
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
using System.Drawing;

namespace FacePuncher.CartConsole
{
	public static class ConsoleColorToColor
	{
		public static Color ToColor(this ConsoleColor Clr)
		{
			switch (Clr) {
				case ConsoleColor.Black:
					return Color.Black;
				case ConsoleColor.Blue:
					return Color.Blue;
				case ConsoleColor.Cyan:
					return Color.Cyan;
				case ConsoleColor.DarkBlue:
					return Color.DarkBlue;
				case ConsoleColor.DarkCyan:
					return Color.DarkCyan;
				case ConsoleColor.DarkGray:
					return Color.DarkGray;
				case ConsoleColor.DarkGreen:
					return Color.DarkGreen;
				case ConsoleColor.DarkMagenta:
					return Color.Magenta;
				case ConsoleColor.DarkRed:
					return Color.DarkRed;
				case ConsoleColor.DarkYellow:
					return Color.Orange;
				case ConsoleColor.Gray:
					return Color.LightGray;
				case ConsoleColor.Green:
					return Color.Green;
				case ConsoleColor.Magenta:
					return Color.Magenta;
				case ConsoleColor.Red:
					return Color.Red;
				case ConsoleColor.White:
					return Color.White;
				case ConsoleColor.Yellow:
					return Color.Yellow;
				default:
					return Color.LightPink;
			}
		}
	}
}
