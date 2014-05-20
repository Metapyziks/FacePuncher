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
using System.Drawing;

namespace FacePuncher.CartConsole
{
    public class Display : FacePuncher.Display
    {
        protected override void OnInitialize(int width, int height)
        {
            CartConsole.Initialize("Data/font.png");
            CartConsole.Title = "FacePuncher";

            CartConsole.SetSize(width, height);
            CartConsole.FontWatcher();
        }

        public override void Clear()
        {
            CartConsole.Clear();
        }

        public override void SetCell(int x, int y, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            CartConsole.Set(x, y, symbol, fore, back);
        }

        public override void Write(int x, int y, string text)
        {
            CartConsole.Write(x, y, text, Color.Gray, Color.Black);
        }

        public override void Refresh()
        {
            CartConsole.Refresh();
        }
    }
}
