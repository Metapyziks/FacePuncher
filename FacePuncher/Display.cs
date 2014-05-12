using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace FacePuncher
{
    static class Display
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
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

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
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
        public struct CharInfo
        {
            [FieldOffset(0)]
            public char Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        static SafeFileHandle _sHandle;
        static CharInfo[] _sBuffer;
        static SmallRect _sRect;

        public static int Width { get { return _sRect.Right; } }
        public static int Height { get { return _sRect.Bottom; } }

        public static void Initialize(int width, int height)
        {
            _sHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            Console.SetWindowSize(Math.Min(width, Console.WindowWidth), Math.Min(height, Console.WindowHeight));

            Console.SetBufferSize(width, height);
            Console.SetWindowSize(width, height);

            Console.CursorVisible = false;

            _sBuffer = new CharInfo[width * height];
            _sRect = new SmallRect { Left = 0, Top = 0, Right = (short) width, Bottom = (short) height };

            Clear();
        }

        public static void Clear()
        {
            for (int i = 0; i < _sBuffer.Length; ++i) {
                _sBuffer[i].Char = ' ';
            }
        }

        public static void SetCell(int x, int y, char symbol, ConsoleColor back, ConsoleColor fore)
        {
            if (x < 0 || y < 0 || x >= _sRect.Right || y >= _sRect.Bottom) return;

            _sBuffer[x + y * _sRect.Right].Char = symbol;
            _sBuffer[x + y * _sRect.Right].Attributes = (short) ((int) fore | ((int) back << 4));
        }

        public static void Refresh()
        {
            var rect = _sRect;

            WriteConsoleOutput(_sHandle, _sBuffer,
                new Coord(_sRect.Right, _sRect.Bottom),
                new Coord(0, 0), ref rect);
        }
    }
}
