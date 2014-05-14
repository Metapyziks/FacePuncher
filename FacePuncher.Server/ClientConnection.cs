using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    class ClientConnection
    {
        const int MaxVisibilityRange = 12;

        private TcpClient _socket;
        private List<RoomVisibility> _visibility;

        public Entity Player { get; private set; }

        public ClientConnection(TcpClient socket, Level level)
        {
            _socket = socket;
            _visibility = level
                .Select(x => new RoomVisibility(x))
                .ToList();

            Player = Entity.Create("player");
            Player.GetComponent<PlayerControl>()
                .SetClient(this);

            var rooms = level
                .Where(x => x.Any(y => y.State == TileState.Floor))
                .ToArray();

            var room = rooms[(int) (DateTime.Now.Ticks % rooms.Length)];

            var tiles = room
                .Where(x => x.State == TileState.Floor)
                .ToArray();

            Player.Place(tiles[(int) (DateTime.Now.Ticks % tiles.Length)]);
        }

        private void SendEntity(BinaryWriter writer, Entity ent)
        {
            writer.Write(ent.ID);
            writer.Write(ent.ClassName);

            // TODO: Send component information?
        }

        public void SendVisibleLevelState(Level level, ulong time)
        {
            try {
                var stream = _socket.GetStream();
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                    writer.Write((byte) PacketType.LevelState);

                    writer.Write(time);
                    writer.Write(Player.ID);

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

                                writer.Write((ushort) tile.EntityCount);
                                foreach (var ent in tile) {
                                    SendEntity(writer, ent);
                                }
                            }
                        }
                    }

                    writer.Flush();
                }
            } catch { }
        }

        public ConsoleKey ReadInput(ConsoleKey[] validKeys)
        {
            try {
                var stream = _socket.GetStream();

                using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                    writer.Write((byte) PacketType.InputRequest);
                    writer.Write((ushort) validKeys.Length);

                    foreach (var key in validKeys) {
                        writer.Write((ushort) key);
                    }

                    writer.Flush();
                }

                using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                    return (ConsoleKey) reader.ReadUInt16();
                }
            } catch {
                return validKeys.First();
            }
        }
    }
}
