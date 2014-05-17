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

using System.Threading;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    /// <summary>
    /// Class containing the client entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Milliseconds between redraws.
        /// </summary>
        const int RenderPeriod = 125;

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        static void Main(string[] args)
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            Display.Initialize(96, 32);

            Thread.Sleep(2000);

            using (var server = new ServerConnection("localhost", 14242)) {
                int flash = 0;

                var renderTimer = new Timer(state => {
                    if (!server.LoadedLevel) return;

                    Display.Clear();

                    lock (server.Visibility) {
                        var attribs = new DrawAttributes(flash++);
                        var rect = Display.Rect + server.PlayerPosition - Display.Center;

                        foreach (var vis in server.Visibility) {
                            vis.Draw(rect, Position.Zero, attribs, server.Time);
                        }
                    }

                    Display.Refresh();
                }, null, 0, RenderPeriod);

                while (server.ProcessPacket());
            }
        }
    }
}
