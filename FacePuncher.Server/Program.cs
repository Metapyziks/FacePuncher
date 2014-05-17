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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using FacePuncher.Geometry;

namespace FacePuncher.Server
{
    /// <summary>
    /// Class containing the server entry point.
    /// </summary>
    static class Program
    {
        static int _capacity;

        static TcpListener _listener;
        static Thread _listenThread;

        static Level _level;
        static List<ClientConnection> _clients;

        /// <summary>
        /// Main loop for the thread that accepts new clients.
        /// </summary>
        static void ListenerLoop()
        {
            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            while (true) {
                if (_clients.Count >= _capacity) {
                    Thread.Sleep(100);
                } else {
                    var socket = _listener.AcceptTcpClient();
                    var client = new ClientConnection(socket, _level);
                    
                    Console.WriteLine("New client connected from {0}.", socket.Client.RemoteEndPoint);
                    
                    _clients.Add(client);
                    client.SendVisibleLevelState();
                }
            }
        }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        static void Main(string[] args)
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Server);

            // TODO: Implement saving / loading levels here.
            _level = LevelGenerator.Generate("test");
            _level.SaveImage("level.png");

            _capacity = 16;

            _clients = new List<ClientConnection>();

            _listenThread = new Thread(ListenerLoop);
            _listenThread.Start();

            while (true) {
                if (_clients.Count > 0) {
                    _level.Think();
                } else {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
