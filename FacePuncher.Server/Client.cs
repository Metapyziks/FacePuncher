using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    class Client
    {
        private TcpClient _socket;
        private Dictionary<Room, RoomVisibility> _visibility;

        public Entity Player { get; private set; }

        public Client(TcpClient socket, Level level)
        {
            _socket = socket;
            _visibility = level.ToDictionary(x => x, x => new RoomVisibility(x));

            Player = Entity.Create("player");
            Player.Place(level
                .First(x => x.Any(y => y.State == TileState.Floor))
                .First(x => x.State == TileState.Floor));
        }
    }
}
