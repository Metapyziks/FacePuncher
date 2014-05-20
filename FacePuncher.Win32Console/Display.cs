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
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

using FacePuncher.Geometry;

namespace FacePuncher.Win32Console
{
    public class Display : FacePuncher.Display
    {
        #region Nasty Windows Specific Stuff
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [StructLayout(LayoutKind.Sequential)]
        struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        struct CharInfo
        {
            [FieldOffset(0)]
            public char Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static short GetAttributes(ConsoleColor fore, ConsoleColor back)
        {
            return (short) ((int) fore | ((int) back << 4));
        }

        SafeFileHandle _handle;
        CharInfo[] _buffer;
        SmallRect _rect;
        #endregion

        /// <summary>
        /// Prepare the display for rendering.
        /// </summary>
        /// <param name="width">Desired horizontal size of the display in characters.</param>
        /// <param name="height">Desired vertical size of the display in characters.</param>
        protected override void OnInitialize(int width, int height)
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero) {
                AllocConsole();
            } else {
                ShowWindow(handle, SW_SHOW);
            }

            _handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            Console.SetWindowSize(Math.Min(width, Console.WindowWidth), Math.Min(height, Console.WindowHeight));

            Console.SetBufferSize(width, height);
            Console.SetWindowSize(width, height);

            Console.CursorVisible = false;

            _buffer = new CharInfo[width * height];
            _rect = new SmallRect { Left = 0, Top = 0, Right = (short) width, Bottom = (short) height };
        }

        /// <summary>
        /// Wipe the display buffer.
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < _buffer.Length; ++i) {
                _buffer[i].Char = ' ';
                _buffer[i].Attributes = GetAttributes(ConsoleColor.Black, ConsoleColor.Black);
            }
        }

        /// <summary>
        /// Set a specific character in the display buffer.
        /// </summary>
        /// <param name="x">Horizontal position of the character.</param>
        /// <param name="y">Vertical position of the character.</param>
        /// <param name="symbol">Character to display.</param>
        /// <param name="fore">Foreground color of the character.</param>
        /// <param name="back">Background color of the character.</param>
        public override void SetCell(int x, int y, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            if (x < 0 || y < 0 || x >= _rect.Right || y >= _rect.Bottom) return;

            int index = x + y * _rect.Right;

            _buffer[index].Char = symbol;
            _buffer[index].Attributes = GetAttributes(fore, back);
        }

        /// <summary>
        /// Send the display buffer to the console window.
        /// </summary>
        public override void Refresh()
        {
            var rect = _rect;

            WriteConsoleOutput(_handle, _buffer,
                new Coord(_rect.Right, _rect.Bottom),
                new Coord(0, 0), ref rect);
        }

        public override void Write(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }
    }
}
