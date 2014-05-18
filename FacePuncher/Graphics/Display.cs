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

#define USE_CONSOLE_INPUT
#undef USE_CONSOLE_INPUT // Comment to enable console input

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using FacePuncher.Geometry;

//using Microsoft.Win32.SafeHandles;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace FacePuncher.Graphics
{
	struct ConsoleClr
	{
		public ConsoleColor Foreground, Background;
		public ConsoleClr(ConsoleColor FG, ConsoleColor BG)
		{
			Foreground = FG;
			Background = BG;
		}
	}

	static class Console
	{
		private static RenderWindow RWind = null;
		private static bool Ctrl, Shift, Alt;
		private static bool KeyDirty;
		private static char KeyChar;
		private static ConsoleKey KeyConsole;

		public static bool KeyAvailable
		{
			get
			{
#if USE_CONSOLE_INPUT
				return System.Console.KeyAvailable;
#else
				return KeyDirty;
#endif
			}
		}

		public static int CursorLeft
		{
			get
			{
				return System.Console.CursorLeft;
			}
			set
			{
				System.Console.CursorLeft = value;
			}
		}

		public static int CursorTop
		{
			get
			{
				return System.Console.CursorTop;
			}
			set
			{
				System.Console.CursorTop = value;
			}
		}

		public static void Initialize(RenderWindow RWind)
		{
			Console.RWind = RWind;

			RWind.KeyPressed += (S, E) => OnKey(S, E, true);
			RWind.KeyReleased += (S, E) => OnKey(S, E, false);
			RWind.SetKeyRepeatEnabled(true);

			RWind.TextEntered += (S, E) => {
				KeyChar = E.Unicode[0];
			};
		}

		private static void OnKey(object Sender, KeyEventArgs E, bool Pressed)
		{
			if (Pressed) {
				KeyDirty = true;
			}

			KeyConsole = ConsoleKey.NoName;

			// TODO Add keys as required
			switch (E.Code) {
				// Control keys
				case Keyboard.Key.LControl:
				case Keyboard.Key.RControl:
					Ctrl = Pressed;
					break;
				case Keyboard.Key.LShift:
				case Keyboard.Key.RShift:
					Shift = Pressed;
					break;
				case Keyboard.Key.LAlt:
				case Keyboard.Key.RAlt:
					Alt = Pressed;
					break;
				// Arrow keys
				case Keyboard.Key.Up:
				case Keyboard.Key.Down:
				case Keyboard.Key.Left:
				case Keyboard.Key.Right:
					KeyConsole = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), E.Code.ToString() + "Arrow");
					break;
				// Keys
				case Keyboard.Key.Space:
					KeyConsole = ConsoleKey.Spacebar;
					break;
				case Keyboard.Key.Return:
					KeyConsole = ConsoleKey.Enter;
					break;
				case Keyboard.Key.Back:
					KeyConsole = ConsoleKey.Backspace;
					break;

				default: {
						int Code = (int)E.Code;
						if (Code > (int)Keyboard.Key.A && Code < (int)Keyboard.Key.Z)
							KeyConsole = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), E.Code.ToString());
						break;
					}
			}
		}

		public static ConsoleKeyInfo ReadKey(bool intercept = false)
		{
#if USE_CONSOLE_INPUT
			return System.Console.ReadKey(intercept);
#else
			while (!KeyDirty)
				;
			KeyDirty = false;
			return new ConsoleKeyInfo(KeyChar, KeyConsole, Shift, Alt, Ctrl);
#endif
		}

		public static void Write(object Format, params object[] Args)
		{
			System.Console.Write(Format.ToString(), Args);
		}
	}

	class VideoMem
	{
		public int Len, W, H;
		public Sprite FontSprite;
		public RectangleShape Background;
		public ConsoleClr CurrentColor;

		public int CharW, CharH, CharCountX;

		public char[] Text;
		public ConsoleClr[] Colors;

		Shader TextShader;

		Image Data, Palette;
		Texture DataTex, PaletteTex;
		VertexArray Display;

		public VideoMem(int W, int H, Sprite FontSprite, int CharCountX = 16, int CharCountY = 16)
		{
			this.Len = W * H;
			this.W = W;
			this.H = H;
			this.FontSprite = FontSprite;
			this.CharW = ((int)FontSprite.Texture.Size.X) / CharCountX;
			this.CharH = ((int)FontSprite.Texture.Size.Y) / CharCountY;
			this.CharCountX = CharCountX;
			Background = new RectangleShape(new Vector2f(CharW, CharH));
			Text = new char[Len];
			Colors = new ConsoleClr[Len];

			// HACK for color mask (
			Image Img = FontSprite.Texture.CopyToImage();
			Color MaskClr = Img.GetPixel(0, 0);
			for (uint x = 0; x < Img.Size.X; x++)
				for (uint y = 0; y < Img.Size.Y; y++) {
					Color CurClr = Img.GetPixel(x, y);
					if (CurClr.R == MaskClr.R && CurClr.G == MaskClr.G && CurClr.B == MaskClr.B && CurClr.A == MaskClr.A)
						Img.SetPixel(x, y, Color.Black);
				}
			FontSprite.Texture = new Texture(Img);

			// Using rohans' texter shaders.
			// TODO Replace with own
			// TODO Fix color masks with non-while colors in font
			string Vert = File.ReadAllText(Tools.GetPath("Data", "text.vert"));
			string Frag = File.ReadAllText(Tools.GetPath("Data", "text.frag"));
			Frag = Frag.Replace("__WIDTH__", CharW.ToString()).Replace("__HEIGHT__", CharH.ToString());
			TextShader = Shader.FromString(Vert, Frag);

			Data = new Image((uint)(W), (uint)(H));
			DataTex = new Texture(Data);

			Palette = new Image(Tools.GetPath("Data", "palette.png"));
			PaletteTex = new Texture(Palette);

			Display = new VertexArray(PrimitiveType.Quads, 4);
			Display[0] = new Vertex(new Vector2f(0, 0), new Vector2f(0, 0));
			Display[1] = new Vertex(new Vector2f(W * CharW, 0), new Vector2f(1, 0));
			Display[2] = new Vertex(new Vector2f(W * CharW, H * CharH), new Vector2f(1, 1));
			Display[3] = new Vertex(new Vector2f(0, H * CharH), new Vector2f(0, 1));

			TextShader.SetParameter("data", DataTex);
			TextShader.SetParameter("dataSize", DataTex.Size.X, DataTex.Size.Y);
			TextShader.SetParameter("font", FontSprite.Texture);
			TextShader.SetParameter("palette", PaletteTex);

			CurrentColor = new ConsoleClr(ConsoleColor.Gray, ConsoleColor.Black);
		}

		public void Set(int X, int Y, char Chr)
		{
			Data.SetPixel((uint)X, (uint)Y, new Color((byte)Chr, (byte)CurrentColor.Foreground, (byte)CurrentColor.Background));
		}

		public char GetChar(int X, int Y)
		{
			return (char)Data.GetPixel((uint)X, (uint)Y).R;
		}

		public void Set(int X, int Y, ConsoleClr Clr)
		{
			Color Pxl = Data.GetPixel((uint)X, (uint)Y);
			Data.SetPixel((uint)X, (uint)Y, new Color(Pxl.R, (byte)Clr.Foreground, (byte)Clr.Background));
		}

		public ConsoleClr GetClr(int X, int Y)
		{
			Color Pxl = Data.GetPixel((uint)X, (uint)Y);
			return new ConsoleClr((ConsoleColor)Pxl.G, (ConsoleColor)Pxl.B);
		}

		public void Draw(RenderTarget RT)
		{
			PaletteTex.Update(Palette);
			DataTex.Update(Data);

			RenderStates RS = RenderStates.Default;
			RS.Shader = TextShader;

			Texture.Bind(null);
			RT.Draw(Display, RS);
		}
	}

	/// <summary>
	/// Wrapper around console display functions.
	/// Adapted from http://stackoverflow.com/a/2754674
	/// </summary>
	static class Display
	{
		public static RenderWindow RWind;
		public static VideoMem VideoMemory;
		public static bool Dirty = true;

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
			VideoMemory = new VideoMem(width, height, new Sprite(new Texture(Tools.GetPath("Data", "font.png"))));

#if USE_CONSOLE
			Console.SetBufferSize(width, height);
			Console.SetWindowSize(width, height);
#endif

			Thread RenderThread = new Thread(() => {
				RWind = new RenderWindow(new VideoMode(
					(uint)width * (uint)VideoMemory.CharW,
					(uint)height * (uint)VideoMemory.CharH),
					"FacePuncher", Styles.Close);

				RWind.SetVerticalSyncEnabled(true);
				Console.Initialize(RWind);

				RenderTexture RTex = new RenderTexture(RWind.Size.X, RWind.Size.Y);
				Sprite RSprite = new Sprite(RTex.Texture);

				RWind.Closed += (S, E) => {
					RWind.Close();
				};

				while (RWind.IsOpen()) {
					RWind.DispatchEvents();

					if (Dirty) {
						Dirty = false;
						RTex.Clear();
						VideoMemory.Draw(RTex);
						RTex.Display();
					}

					RSprite.Draw(RWind, RenderStates.Default);
					RWind.Display();
				}

				Environment.Exit(0);
			});

			Clear();
			RenderThread.Start();

			/*SetCell(0, 0, 'H', ConsoleColor.White, ConsoleColor.Black);
			SetCell(1, 0, 'e', ConsoleColor.Red, ConsoleColor.DarkRed);
			SetCell(2, 0, 'l', ConsoleColor.DarkBlue, ConsoleColor.Blue);
			SetCell(3, 0, 'l');
			SetCell(4, 0, 'o');
			SetCell(5, 0, ' ');
			SetCell(6, 0, 'W');
			SetCell(7, 0, 'o');
			SetCell(8, 0, 'r');
			SetCell(9, 0, 'l');
			SetCell(10, 0, 'd');
			while (true)
				; //*/
		}

		/// <summary>
		/// Wipe the display buffer.
		/// </summary>
		public static void Clear()
		{
			ConsoleClr ClearClr = new ConsoleClr(ConsoleColor.Gray, ConsoleColor.Black);
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++) {
					VideoMemory.Set(x, y, ' ');
					VideoMemory.Set(x, y, ClearClr);
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
			VideoMemory.Set(x, y, symbol);
			VideoMemory.Set(x, y, new ConsoleClr(fore, back));

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
			Dirty = true;
		}
	}
}