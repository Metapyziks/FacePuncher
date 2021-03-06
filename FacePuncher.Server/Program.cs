/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using FacePuncher.Geometry;

namespace FacePuncher.Server
{
    /// <summary>
    /// Class containing the server entry point.
    /// </summary>
    public static class Program
    {
        static int _capacity;

        static TcpListener _listener;

        static Level _level;
        static List<ClientConnection> _clients;

        /// <summary>
        /// Main loop for the thread that accepts new clients.
        /// </summary>
        static async void RunListenerLoop()
        {
            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            while (true) {
                if (_clients.Count >= _capacity) {
                    await Task.Delay(100);
                } else {
                    var socket = await _listener.AcceptTcpClientAsync();
                    var client = new ClientConnection(socket, _level);
                    client.Run();

                    Console.WriteLine("New client connected from {0}.", socket.Client.RemoteEndPoint);

                    _clients.Add(client);
                }
            }
        }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        public static void Main(string[] args)
        {
            var context = new SynchronizationContext();
            context.Send((x) => AsyncMain().Wait(), null);
        }

        private static async Task AsyncMain()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Definitions.LoadFromDirectory("Data", DefinitionsNamespace.Server);

            // TODO: Implement saving / loading levels here.
            var generator = LevelGenerator.Get("test");

            _level = generator.Generate();
            _level.SaveImage("level.png");

            _capacity = 16;

            _clients = new List<ClientConnection>();

            RunListenerLoop();

            var timer = new Stopwatch();
            var originTime = 0.0;

            while (true) {
                if (_clients.Count > 0) {
                    if (!timer.IsRunning) {
                        timer.Restart();
                    }

                    _level.Advance(timer.Elapsed.TotalSeconds + originTime - _level.Time);
                    await Task.Delay(10);
                } else {
                    originTime = _level.Time;
                    await Task.Delay(100);
                }

                Console.WriteLine("{0:F2}", _level.Time);
                Console.CursorTop -= 1;

                await Task.Yield();
            }
        }
    }
}
