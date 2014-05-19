/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Saša Barišiè (cartman300@net.hr)
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

#define USE_CONSOLE
#undef USE_CONSOLE // Comment to enable print to console


using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using FacePuncher.Geometry;
using SdlDotNet;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.Windows;

using Rectangle = FacePuncher.Geometry.Rectangle;
using DRect = System.Drawing.Rectangle;

namespace FacePuncher.Graphics
{
	struct ConsoleClr
	{
		public Color Foreground, Background;

		public ConsoleClr(Color FG, Color BG)
		{
			this.Foreground = FG;
			this.Background = BG;
		}

		public ConsoleClr(ConsoleColor FG, ConsoleColor BG)
		{
			Foreground = ToColor(FG);
			Background = ToColor(BG);
		}

		public static Color ToColor(ConsoleColor Clr)
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
					return Color.DarkMagenta;
				case ConsoleColor.DarkRed:
					return Color.DarkRed;
				case ConsoleColor.DarkYellow:
					return Color.Orange;
				case ConsoleColor.Gray:
					return Color.Gray;
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
			throw new Exception("Unreachable code reached in ConsoleClr"); // Shouldn't happen
		}
	}

	struct ConsoleInput
	{
		public bool Ctrl, Alt, Shift;
		public char Character;
		public Key Key;

		public ConsoleInput(char Character, Key Key, bool Ctrl = false, bool Alt = false, bool Shift = false)
		{
			this.Ctrl = Ctrl;
			this.Alt = Alt;
			this.Shift = Shift;
			this.Character = Character;
			this.Key = Key;
		}
	}

	class VideoMem
	{
		public int Len, W, H;
		public ConsoleClr CurrentColor;
		public bool Dirty;

		public int CharW, CharH, CharCountX, CharCountY;

		public char[] Text;
		public ConsoleClr[] Colors;

		//Surface FontSurface;
		Surface ScreenSurf, ScreenSurfBackground, FontSurfaceTinted, FontSurfaceMask;

		public VideoMem(int W, int H, Surface FontSurface, Surface FontSurfaceMask, int CharCountX = 16, int CharCountY = 16)
		{
		
			this.Len = W * H;
			this.W = W;
			this.H = H;
			this.CharW = FontSurface.Size.Width / CharCountX;
			this.CharH = FontSurface.Size.Height / CharCountY;
			this.CharCountX = CharCountX;
			this.CharCountY = CharCountY;

			Text = new char[Len];
			Colors = new ConsoleClr[Len];

			/*this.FontSurface = FontSurface;
			FontSurface.TransparentColor = FontSurface.GetPixel(new Point(0, 0));
			FontSurface.Transparent = true;*/

			this.FontSurfaceMask = FontSurfaceMask;
			FontSurfaceMask.SourceColorKey = Color.White;

			FontSurfaceTinted = new Surface(FontSurface);
			FontSurfaceTinted.SourceColorKey = Color.Black;

			Tint(new ConsoleClr(ConsoleColor.White, ConsoleColor.Black));

			ScreenSurf = new Surface(new Size(W * CharW, H * CharH));
			ScreenSurf.SourceColorKey = Color.Black;
			ScreenSurfBackground = new Surface(new Size(W * CharW, H * CharH));
			LockRect = new DRect(0, 0, FontSurface.Bitmap.Size.Width, FontSurface.Bitmap.Size.Height);
		}

		private void Tint(ConsoleClr Clr)
		{
			if (CurrentColor.Foreground == Clr.Foreground)
				return;
			CurrentColor = Clr;

			FontSurfaceTinted.Fill(Clr.Foreground);
			FontSurfaceTinted.Blit(FontSurfaceMask);

		}

		private DRect LockRect;

		public void Set(int X, int Y, char Chr, ConsoleClr Clr)
		{
			DRect Dest = new DRect(X * CharW, Y * CharH, CharW, CharH);
			DRect Src = new DRect(((byte)Chr % CharCountX) * CharW, ((byte)Chr / CharCountY) * CharH, CharW, CharH);

			ScreenSurfBackground.Fill(Dest, Clr.Background);
			Tint(Clr);
			ScreenSurf.Blit(FontSurfaceTinted, Dest, Src);
		}

		public void Write(int X, int Y, string S, ConsoleClr Clr)
		{
			for (int I = Y * W + X, J = I + S.Length, R = 0; I < J; I++, R++) {
				Set(I % W, I / W, S[R], Clr);
			}
		}

		public void Write(int X, int Y, string S)
		{
			Write(X, Y, S, new ConsoleClr(Color.Gray, Color.Black));
		}

		public void Draw()
		{
			if (!Dirty)
				return;
			Dirty = false;

			Video.Screen.Blit(ScreenSurfBackground);
			Video.Screen.Blit(ScreenSurf);
		}
	}

	/// <summary>
	/// Wrapper around console display functions.
	/// Adapted from http://stackoverflow.com/a/2754674
	/// </summary>
	static class Display
	{
		public static VideoMem VideoMemory;

		public static Rectangle Rect
		{
			get;
			private set;
		}

		/// <summary>
		/// Horizontal size of the display in characters.
		/// </summary>
		public static int Width
		{
			get
			{
				return Rect.Width;
			}
		}

		/// <summary>
		/// Vertical size of the display in characters.
		/// </summary>
		public static int Height
		{
			get
			{
				return Rect.Height;
			}
		}

		/// <summary>
		/// Length of display in characters
		/// </summary>
		public static int Length
		{
			get
			{
				return Rect.Width * Rect.Height;
			}
		}

		/// <summary>
		/// Position of the center of the display.
		/// </summary>
		public static Position Center
		{
			get
			{
				return new Position(Width / 2, Height / 2);
			}
		}

		/// <summary>
		/// Prepare the display for rendering.
		/// </summary>
		/// <param name="width">Desired horizontal size of the display in characters.</param>
		/// <param name="height">Desired vertical size of the display in characters.</param>
		public static void Initialize(int width, int height)
		{
			Rect = new Rectangle(0, 0, width, height);

#if USE_CONSOLE
			Console.SetBufferSize(width, height);
			Console.SetWindowSize(width, height);
#endif

			bool Initializing = true;

			Thread RenderThread = new Thread(() => {
				VideoMemory = new VideoMem(width, height,
					new Surface(Tools.GetPath("Data", "font.png")),
					new Surface(Tools.GetPath("Data", "fontmask.png")));

				Video.SetVideoMode(width * VideoMemory.CharW, height * VideoMemory.CharH, false, false, false, true, true);
				Video.WindowCaption = "FacePuncher";

				Input.Initialize();

				Events.Quit += (S, E) => {
					Events.QuitApplication();
					Environment.Exit(0);
				};

				Events.Tick += (S, E) => {
					if (VideoMemory.Dirty) {
						Video.Screen.Fill(Color.Black);
						VideoMemory.Write(0, 0, E.Fps.ToString() + "FPS", new ConsoleClr(Color.Red, Color.DarkRed));
						VideoMemory.Draw();
						Video.Update();
					}
				};

				Initializing = false;
				Events.Run();
			});

			RenderThread.Start();
			while (Initializing)
				;
			Clear();
		}

		/// <summary>
		/// Wipe the display buffer.
		/// </summary>
		public static void Clear()
		{
			if (VideoMemory != null) {
				ConsoleClr ClearClr = new ConsoleClr(ConsoleColor.Gray, ConsoleColor.Black);
				for (int x = 0; x < Width; x++)
					for (int y = 0; y < Height; y++)
						VideoMemory.Set(x, y, ' ', ClearClr);
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
		public static void SetCell(int x, int y, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
		{
			if (VideoMemory != null)
				VideoMemory.Set(x, y, symbol, new ConsoleClr(fore, back));

#if USE_CONSOLE
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = fore;
			Console.BackgroundColor = back;
			Console.Write(symbol);
#endif
		}

		/// <summary>
		/// Set a specific character in the display buffer.
		/// </summary>
		/// <param name="pos">Position of the character.</param>
		/// <param name="symbol">Character to display.</param>
		/// <param name="fore">Foreground color of the character.</param>
		/// <param name="back">Background color of the character.</param>
		public static void SetCell(Position pos, char symbol, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
		{
			SetCell(pos.X, pos.Y, symbol, fore, back);
		}

		/// <summary>
		/// Send the display buffer to the console window.
		/// </summary>
		public static void Refresh()
		{
			if (VideoMemory != null)
				VideoMemory.Dirty = true;
		}
	}
}