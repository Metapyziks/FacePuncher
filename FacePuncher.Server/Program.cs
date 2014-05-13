using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FacePuncher.Geometry;

namespace FacePuncher.Server
{
    class Program
    {
        static TcpListener _listener;

        static void Main(string[] args)
        {
            var gen = new LevelGenerator();
            var level = gen.Generate(0);

            _listener = new TcpListener(IPAddress.Any, 14242);
            _listener.Start();

            do {
                var client = _listener.AcceptTcpClient();
            } while (true);
        }
    }
}
