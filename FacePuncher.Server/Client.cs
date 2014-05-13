using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    class Client
    {
        const int MaxVisibilityRange = 12;

        private TcpClient _socket;
        private List<RoomVisibility> _visibility;

        public Entity Player { get; private set; }

        public Client(TcpClient socket, Level level)
        {
            _socket = socket;
            _visibility = level
                .Select(x => new RoomVisibility(x))
                .ToList();

            Player = Entity.Create("player");

            var rooms = level
                .Where(x => x.Any(y => y.State == TileState.Floor))
                .ToArray();

            var room = rooms[(int) (DateTime.Now.Ticks % rooms.Length)];

            var tiles = room
                .Where(x => x.State == TileState.Floor)
                .ToArray();

            Player.Place(tiles[(int) (DateTime.Now.Ticks % tiles.Length)]);
        }

        public void SendVisibleLevelState(Level level, ulong time)
        {
            var stream = _socket.GetStream();
            var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);

            writer.Write(time);
            writer.Write(Player.Position);

            lock (level) {
                var visibleRooms = _visibility
                    .Where(x => x.UpdateVisibility(Player.Position, MaxVisibilityRange, time))
                    .ToArray();

                writer.Write(visibleRooms.Length);
                foreach (var vis in visibleRooms) {
                    writer.Write(vis.Room.Rect);

                    var visibleTiles = vis.GetVisible(time).ToArray();

                    writer.Write(visibleTiles.Length);
                    foreach (var tile in visibleTiles) {
                        writer.Write(tile.RelativePosition);
                        writer.Write((byte) tile.State);
                    }
                }
            }

            writer.Flush();
            writer.Close();
        }
    }
}
