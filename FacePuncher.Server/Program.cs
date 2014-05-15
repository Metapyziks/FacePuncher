using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using FacePuncher.Geometry;
using System.Threading.Tasks;

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

        static ulong _time;

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
            AsyncMain().Wait();
        }
        private static async Task AsyncMain()
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Server);

            // TODO: Implement saving / loading levels here.
            var gen = new LevelGenerator();
            _level = gen.Generate(0);

            _capacity = 16;
            _time = 1;

            _clients = new List<ClientConnection>();

            RunListenerLoop();

            while (true)
            {
                if (_clients.Count > 0)
                {
                    _level.Think(_time++);
                }
                else
                {
                    await Task.Delay(100);
                }

                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                Console.Write(_time);
            }
        }
    }
}
