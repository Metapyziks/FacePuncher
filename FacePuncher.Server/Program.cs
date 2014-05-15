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
                    client.SendVisibleLevelState(_level);
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
            var gen = new LevelGenerator();
            _level = gen.Generate(0);

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
