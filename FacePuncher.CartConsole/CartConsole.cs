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

// Using SDL2-CS https://github.com/flibitijibibo/SDL2-CS and original SDL2 and SDL2_image files
// Using tileset from http://dwarffortresswiki.org/index.php/Tileset_repository

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.IO;

using KeyCode = SDL2.SDL.SDL_Keycode;

using SDL = SDL2.SDL;
using SDLi = SDL2.SDL_image;
using SDLm = SDL2.SDL_mixer;

using SDLClr = SDL2.SDL.SDL_Color;
using CONCLR = SDL2.SDL.SDL_Color;

namespace FacePuncher.CartConsole
{
	public static class CartConsole
	{
		private static Stopwatch SWatch = new Stopwatch();

		private static IntPtr Wind, Rend, FontTex, PixelFormat;

		private static bool Open = false, Ctrl = false, Shift = false, Alt = false, Initialized = false, DoRefresh = true;
		private static bool FontDirty = false;
		private static KeyCode KC;
		private static Queue<CartConsoleInput> InputQueue;

		private static SDL.SDL_Event Event;
		private static StringBuilder Input;

		private static int W = 80, H = 40, CharW = 8, CharH = 8, FontW = 0, FontH = 0, CharCount, CharCountX, CharCountY;
		private static string FontPath, WindTitle = "[NULL]";
		private static bool ReloadSDL = false;
		private static SDL.SDL_Rect POS, TEXPOS;

		private static char[] TEXT;
		private static CONCLR[] FORE_C, BACK_C;
		private static bool[] DIRTY;

		public static long FrameTime;

		private static void CreateSDL()
		{
			if (Rend != IntPtr.Zero) {
				SDL.SDL_DestroyRenderer(Rend);
				Rend = IntPtr.Zero;
			}
			if (Wind != IntPtr.Zero) {
				SDL.SDL_DestroyWindow(Wind);
				Wind = IntPtr.Zero;
			}
			if (FontTex != IntPtr.Zero) {
				SDL.SDL_FreeSurface(FontTex);
				FontTex = IntPtr.Zero;
			}

			Wind = SDL.SDL_CreateWindow(WindTitle, 100, 100, 0, 0, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
			Rend = SDL.SDL_CreateRenderer(Wind, -1,
				(uint)(SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC));

			PixelFormat = SDL.SDL_AllocFormat(SDL.SDL_GetWindowPixelFormat(Wind));

			SetFont(FontPath);
		}

		private unsafe static void Main()
		{
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) != 0)
				throw new Exception(SDL.SDL_GetError());
			if (SDLi.IMG_Init(SDLi.IMG_InitFlags.IMG_INIT_PNG) == 0)
				throw new Exception(SDL.SDL_GetError());

			CreateSDL();

			Open = true;
			Initialized = true;
			bool Changed = false;

			SWatch.Start();

			while (Open) {
				while (SDL.SDL_PollEvent(out Event) != 0) {
					switch (Event.type) {
						case SDL.SDL_EventType.SDL_QUIT:
							Open = false;
							break;
						case SDL.SDL_EventType.SDL_TEXTINPUT:
							fixed (byte* T = Event.text.text) {
								char Chr = (char)T[0];
								InputQueue.Enqueue(new CartConsoleInput(Chr, KC, Ctrl, Shift, Alt));
							}
							break;

						case SDL.SDL_EventType.SDL_KEYDOWN: {
								KeyCode KC = Event.key.keysym.sym;

								if ((KC == KeyCode.SDLK_LCTRL) || (KC == KeyCode.SDLK_RCTRL))
									Ctrl = true;
								if ((KC == KeyCode.SDLK_LALT) || (KC == SDL.SDL_Keycode.SDLK_RALT))
									Alt = true;
								if ((KC == KeyCode.SDLK_LSHIFT) || (KC == KeyCode.SDLK_RSHIFT))
									Shift = true;

								CartConsole.KC = KC;

								switch (KC) {
									case KeyCode.SDLK_UP:
									case KeyCode.SDLK_DOWN:
									case KeyCode.SDLK_LEFT:
									case KeyCode.SDLK_RIGHT:
									case KeyCode.SDLK_RETURN:
									case KeyCode.SDLK_RETURN2:
									case KeyCode.SDLK_BACKSPACE:
										InputQueue.Enqueue(new CartConsoleInput('\n', KC, Ctrl, Shift, Alt));
										break;
									default:
										break;
								}
								break;
							}

						case SDL.SDL_EventType.SDL_KEYUP: {
								KeyCode KC = Event.key.keysym.sym;
								if ((KC == KeyCode.SDLK_LCTRL) || (KC == KeyCode.SDLK_RCTRL))
									Ctrl = false;
								if ((KC == KeyCode.SDLK_LALT) || (KC == KeyCode.SDLK_RALT))
									Alt = false;
								if ((KC == KeyCode.SDLK_LSHIFT) || (KC == KeyCode.SDLK_RSHIFT))
									Shift = false;
								break;
							}

						default:
							break;

					}
				}

				if (ReloadSDL) {
					ReloadSDL = false;
					CreateSDL();
				}

				SDL.SDL_SetRenderDrawBlendMode(Rend, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
				byte CurChar = 0;

				if (DoRefresh) {
					DoRefresh = Changed = false;

					for (int i = 0; i < TEXT.Length; i++) {
						if (!DIRTY[i])
							continue;
						DIRTY[i] = false;
						Changed = true;

						POS.x = (i % W) * CharW;
						POS.y = (i / W) * CharH;
						CurChar = (byte)TEXT[i];
						TEXPOS.x = (CurChar % CharCountX) * CharW;
						TEXPOS.y = (CurChar / CharCountY) * CharH;

						fixed (void* TP = &TEXPOS, P = &POS) {
							SDL.SDL_SetRenderDrawColor(Rend, BACK_C[i].r, BACK_C[i].g, BACK_C[i].b, BACK_C[i].a);
							SDL.SDL_RenderFillRect(Rend, ref POS);

							SDL.SDL_SetTextureColorMod(FontTex, FORE_C[i].r, FORE_C[i].g, FORE_C[i].b);
							SDL.SDL_SetTextureAlphaMod(FontTex, FORE_C[i].a);
							SDL.SDL_RenderCopy(Rend, FontTex, new IntPtr(TP), new IntPtr(P));
						}
					}

					if (Changed)
						SDL.SDL_RenderPresent(Rend);
				}

				SDL.SDL_Delay(10);

				SWatch.Stop();
				FrameTime = SWatch.ElapsedMilliseconds;
				SWatch.Restart();

				if (FontDirty) {
					FontDirty = false;
					SetFont(CartConsole.FontPath, CharCountX, CharCountY);
				}
			}

			SDL.SDL_DestroyRenderer(Rend);
			SDL.SDL_DestroyWindow(Wind);
			Environment.Exit(0);
		}

		public static void Set(int X, int Y, char Chr, Color Fore, Color Back)
		{
			int i = Y * W + X;

			if (TEXT[i] != Chr ||
				FORE_C[i].r != Fore.R || FORE_C[i].g != Fore.G || FORE_C[i].b != Fore.B || FORE_C[i].a != Fore.A ||
				BACK_C[i].r != Back.R || BACK_C[i].g != Back.G || BACK_C[i].b != Back.B || BACK_C[i].a != Back.A) {

				TEXT[i] = Chr;

				FORE_C[i].r = Fore.R;
				FORE_C[i].g = Fore.G;
				FORE_C[i].b = Fore.B;
				FORE_C[i].a = Fore.A;

				BACK_C[i].r = Back.R;
				BACK_C[i].g = Back.G;
				BACK_C[i].b = Back.B;
				BACK_C[i].a = Back.A;

				DIRTY[i] = true;
			}
		}

		// TODO, switch buffers here/proper double buffering
		public static void Refresh()
		{
			DoRefresh = true;
		}

		public static void Initialize(string FontPath)
		{
			CartConsole.FontPath = FontPath;

			InputQueue = new Queue<CartConsoleInput>();
			Input = new StringBuilder();

			Thread CartRuntime = new Thread(Main);
			CartRuntime.Start();

			while (!Initialized)
				;
		}

		public static string Title
		{
			get
			{
				return SDL.SDL_GetWindowTitle(Wind);
			}
			set
			{
				SDL.SDL_SetWindowTitle(Wind, value);
				WindTitle = value;
			}
		}

		public static bool KeyAvailable
		{
			get
			{
				return InputQueue.Count > 0;
			}
		}

		public static int Width
		{
			get
			{
				int w = 0, h = 0;
				SDL.SDL_GetWindowSize(Wind, out w, out h);
				return w / CharW;
			}
			set
			{
				int w = 0, h = 0;
				SDL.SDL_GetWindowSize(Wind, out w, out h);
				SetSize(value, h / CharH);
			}
		}

		public static int Height
		{
			get
			{
				int w = 0, h = 0;
				SDL.SDL_GetWindowSize(Wind, out w, out h);
				return h / CharH;
			}
			set
			{
				int w = 0, h = 0;
				SDL.SDL_GetWindowSize(Wind, out w, out h);
				SetSize(w / CharW, value);
			}
		}

		public static int Length
		{
			get
			{
				return CharCount;
			}
		}

		public static void Clear()
		{
			for (int i = 0; i < CharCount; i++)
				Set(i, 0, ' ', Color.LightGray, Color.Black);
		}

		/// <summary>
		/// Set window size in characters
		/// </summary>
		/// <param name="W">Width in chars</param>
		/// <param name="H">Height in chars</param>
		public static void SetSize(int W, int H)
		{
			CartConsole.W = W;
			CartConsole.H = H;
			SDL.SDL_SetWindowSize(Wind, W * CharW, H * CharH);
			CharCount = W * H;

			char[] OTEXT = TEXT;
			TEXT = new char[CharCount];

			CONCLR[] OFORE_C = FORE_C, OBACK_C = BACK_C;
			FORE_C = new CONCLR[CharCount];
			BACK_C = new CONCLR[CharCount];

			DIRTY = new bool[CharCount];

			if (OTEXT != null && OFORE_C != null && OBACK_C != null) {
				int L = Math.Min(TEXT.Length, OTEXT.Length);
				for (int i = 0; i < L; i++) {
					TEXT[i] = OTEXT[i];
					FORE_C[i] = OFORE_C[i];
					BACK_C[i] = OBACK_C[i];
					DIRTY[i] = true;
				}
			}
		}

		private static FileSystemWatcher FntWatcher;

		public static void FontWatcher(bool Enable = true)
		{
			if (Enable) {
				FntWatcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(CartConsole.FontPath)));
				FntWatcher.Changed += (S, E) => {
					if (E.Name == "font.png") {
						if (E.ChangeType != WatcherChangeTypes.Changed)
							throw new Exception("Font file has been moved/renamed/deleted!");
						FontDirty = true;
					}
				};
				FntWatcher.EnableRaisingEvents = true;
			} else if (FntWatcher != null) {
				FntWatcher.EnableRaisingEvents = false;
				FntWatcher.Dispose();
				FntWatcher = null;
			}
		}

		public static void SetFont(string Path, int CharCountX = 16, int CharCountY = 16)
		{
			CartConsole.FontPath = Path;
			CartConsole.CharCountX = CharCountX;
			CartConsole.CharCountY = CharCountY;

			if (FontTex != IntPtr.Zero) {
				SDL.SDL_FreeSurface(FontTex);
				FontTex = IntPtr.Zero;
			}

			FontTex = SDLi.IMG_Load(Path);
			if (FontTex == IntPtr.Zero)
				throw new Exception("Could not load font:\n" + Path);

			SDL.SDL_SetColorKey(FontTex, 1, SDL.SDL_MapRGB(PixelFormat, 255, 0, 255));
			SDL.SDL_SetSurfaceBlendMode(FontTex, SDL.SDL_BlendMode.SDL_BLENDMODE_NONE);
			var FontTexTmp = SDL.SDL_CreateTextureFromSurface(Rend, FontTex);
			SDL.SDL_FreeSurface(FontTex);
			FontTex = FontTexTmp;

			uint Format = 0;
			int Access = 0;

			if (SDL.SDL_QueryTexture(FontTex, out Format, out Access, out FontW, out FontH) != 0)
				throw new Exception(SDL.SDL_GetError());


			POS = new SDL.SDL_Rect();
			TEXPOS = new SDL.SDL_Rect();

			POS.x = POS.y = 0;

			TEXPOS.w = POS.w = CharW = FontW / CharCountX;
			TEXPOS.h = POS.h = CharH = FontH / CharCountY;

			SetSize(W, H);
		}

		public static void Write(int X, int Y, string S, Color FG, Color BG)
		{
			int I = Y * W + X;
			for (int i = I, j = 0; i < I + S.Length; i++, j++) {
				int XX = i % W;
				int YY = i / W;
				Set(XX, YY, S[j], FG, BG);
			}
		}

		public static CartConsoleInput ReadKey(bool intercept = false)
		{
			while (!KeyAvailable)
				;
			return InputQueue.Dequeue();
		}

		public static string ReadLine()
		{
			Input.Clear();

			CartConsoleInput CCI = ReadKey();
			while ((CCI.Key != KeyCode.SDLK_RETURN) && (CCI.Key != KeyCode.SDLK_RETURN2)) {
				if (CCI.Chr != '\0')
					Input.Append(CCI.Chr);
				if (CCI.Key == KeyCode.SDLK_BACKSPACE) {
					string In = Input.ToString(0, Input.Length - 1);
					Input.Clear();
					Input.Append(In);
				}
				CCI = ReadKey();
			}

			return Input.ToString();
		}

	}
}