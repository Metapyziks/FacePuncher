using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher.Server
{
    static class Program
    {
        static int _capacity;

        static TcpListener _listener;
        static Thread _listenThread;

        static Level _level;
        static List<Client> _clients;

        static ulong _time;

        static void ListenerLoop()
        {
            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            while (true) {
                if (_clients.Count >= _capacity) {
                    Thread.Sleep(100);
                } else {
                    var socket = _listener.AcceptTcpClient();
                    var client = new Client(socket, _level);
                    Console.WriteLine("New client connected from {0}.", socket.Client.RemoteEndPoint);
                    _clients.Add(client);

                    client.SendVisibleLevelState(_level, _time);
                }
            }

            _listener.Stop();
        }

        static void Main(string[] args)
        {
            Entity.Register("player", ent => { });
            Entity.Register("dust", ent => { });

            var gen = new LevelGenerator();
            _level = gen.Generate(0);

            _capacity = 16;
            _time = 1;

            _clients = new List<Client>();

            _listenThread = new Thread(ListenerLoop);
            _listenThread.Start();

            while (true) {
                if (_clients.Count > 0) {
                    _level.Think(_time++);
                } else {
                    Thread.Sleep(100);
                }
            }

            _listener.Stop();
        }
    }
}
