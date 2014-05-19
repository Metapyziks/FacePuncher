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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

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

            while (true)
            {
                if (_clients.Count >= _capacity)
                {
                    await Task.Delay(100);
                }
                else
                {
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

            while (true)
            {
                if (_clients.Count > 0)
                {
                    _level.Think();
                    await Task.Delay(10);
                }
                else
                {
                    await Task.Delay(100);
                }

                Console.WriteLine(_level.Time);
                Console.CursorTop -= 1;

                await Task.Yield();
            }
        }
    }
}
