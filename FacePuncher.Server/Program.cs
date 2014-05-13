using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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

        static void ListenerLoop()
        {
            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            while (true) {
                if (_clients.Count >= _capacity) {
                    Thread.Sleep(100);
                } else {
                    _clients.Add(new Client(_listener.AcceptTcpClient(), _level));
                }
            }

            _listener.Stop();
        }

        static void Main(string[] args)
        {
            var gen = new LevelGenerator();
            _level = gen.Generate(0);

            _capacity = 16;

            _clients = new List<Client>();

            _listenThread = new Thread(ListenerLoop);
            _listenThread.Start();

            while (true) {
                if (_clients.Count > 0) {

                } else {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
