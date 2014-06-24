/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * Copyright (c) 2014 Saša Barišić [cartman300@net.hr]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using FacePuncher.Geometry;
using FacePuncher.Graphics;
using FacePuncher.UI;

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
            try {
                var context = new SynchronizationContext();
                context.Send((x) => TaskMain().Wait(), null);
            } catch (AggregateException E) {
                throw E.InnerException;
            }
        }

        static async Task TaskMain()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Definitions.LoadFromDirectory("Data", DefinitionsNamespace.Client);

            Interface.Display.Initialize(96, 32);

            ServerConnection server = null;

            UIManager = new UIManager();

            var select = (Frame) Widget.Create("ServerSelect", "serverselect");

            ((Button) select.Children["btn_connect"]).Used += (sender, e) => {
                var txtHost = ((TextBox) select.Children["txt_hostname"]);
                var txtPort = ((TextBox) select.Children["txt_port"]);

                int port;
                if (!int.TryParse(txtPort.Text, out port) || port >= ushort.MaxValue) return;

                UIManager.IsInputBlocked = true;

                server = new ServerConnection(txtHost.Text, port);
                server.Run();

                UIManager.RemoveChild(select);
                UIManager.CalculateSelectableWidgets();
            };

            ((Button) select.Children["btn_quit"]).Used += (sender, e) => {
                Environment.Exit(0);
            };

            UIManager.AddChild(select);
            UIManager.CalculateSelectableWidgets();

            var direc = Direction.None;
            var action = Interaction.None;

            while (true) {
                if (server == null) {
                    Draw(null);
                    await Task.Delay(100);
                } else if (Interface.Input.TryReadMovement(out direc)) {
                    server.SendIntent(new MoveIntent(direc));
                } else if (Interface.Input.TryReadInteraction(out action)) {
                    server.SendIntent(new InteractIntent(action));
                } else {
                    await Task.Delay(100);
                }

                await Task.Yield();
            }
        }

        //TODO: Restore framerate
        static int _flash = 0;
        internal static void Draw(ServerConnection server)
        {
            Interface.Display.Clear();

            if (server != null) {
                // removed Level lock
                var attribs = new DrawAttributes(_flash++);
                var rect = Interface.Display.Rect + server.PlayerPosition - Interface.Display.Center;

                foreach (var vis in server.Visibility) {
                    vis.Draw(rect, Position.Zero, attribs, server.Time);
                }
            }

            // Render user interface
            if (UIManager != null)
                UIManager.Draw();

            Interface.Display.Refresh();
        }
    }
}
