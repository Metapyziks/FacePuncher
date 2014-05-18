/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Tamme Schichler (tammeschichler@googlemail.com)
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

using FacePuncher.Geometry;
using FacePuncher.Graphics;
using FacePuncher.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FacePuncher
{
    /// <summary>
    /// Class containing the client entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Milliseconds between redraws.
        /// </summary>
        const int RenderPeriod = 125;

        /// <summary>
        /// Currently used UI Manager
        /// </summary>
        static UIManager UIManager;

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        public static void Main(string[] args)
        {
            var context = new SynchronizationContext();
            context.Send((x) => TaskMain().Wait(), null);
        }
        static async Task TaskMain()
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            Display.Initialize(96, 32);

            var server = new ServerConnection("localhost", 14242);
            server.Run();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (Tools.MovementKeys.ContainsKey(key.Key)) {
                        server.SendIntent(new MoveIntent(Tools.MovementKeys[key.Key]));
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
                await Task.Yield();
            }
        }

        //TODO: Restore framerate
        static int _flash = 0;
        internal static void Draw(ServerConnection server)
        {
            Display.Clear();

            // removed Level lock
            var attribs = new DrawAttributes(_flash++);
            var rect = Display.Rect + server.PlayerPosition - Display.Center;

            foreach (var vis in server.Visibility)
            {
                vis.Draw(rect, Position.Zero, attribs, server.Time);
            }

            // Render user interface
            if (UIManager != null)
                UIManager.Draw();

            Display.Refresh();
        }
    }
}
