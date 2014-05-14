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

        static ulong _time;

        /// <summary>
        /// Main loop for the thread that accepts new clients.
        /// </summary>
        static void ListenerLoop()
        {
            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            while (true) {
                // If we're full take a nap.
                if (_clients.Count >= _capacity) {
                    Thread.Sleep(100);
                } else {
                    // Accept a new client.
                    var socket = _listener.AcceptTcpClient();
                    var client = new ClientConnection(socket, _level);
                    
                    Console.WriteLine("New client connected from {0}.", socket.Client.RemoteEndPoint);
                    
                    // Add it to the client list and send it an
                    // initial glimpse of the world.
                    _clients.Add(client);
                    client.SendVisibleLevelState(_level, _time);
                }
            }
        }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        static void Main(string[] args)
        {
            // Load entity and other definitions from the data directory.
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Server);

            // Generate a level.
            // TODO: Implement saving / loading levels here.
            var gen = new LevelGenerator();
            _level = gen.Generate(0);

            _capacity = 16;
            _time = 1;

            _clients = new List<ClientConnection>();

            _listenThread = new Thread(ListenerLoop);
            _listenThread.Start();

            // Continually update the level while players are connected.
            while (true) {
                if (_clients.Count > 0) {
                    _level.Think(_time++);
                } else {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
