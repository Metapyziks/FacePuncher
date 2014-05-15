using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    /// <summary>
    /// Handles communication with an individual client.
    /// </summary>
    class ClientConnection
    {
        /// <summary>
        /// Maximum range of a clients visibility.
        /// 
        /// TODO: Move to become a property of the player character.
        /// </summary>
        const int MaxVisibilityRange = 12;

        private TcpClient _socket;
        private RoomVisibility[] _visibility;

        /// <summary>
        /// The client's player entity.
        /// </summary>
        public Entity Player { get; private set; }

        /// <summary>
        /// The current level the player is within.
        /// </summary>
        public Level Level { get; private set; }

        /// <summary>
        /// Creates a new ClientConnection instance using a socket,
        /// additionally creating a player entity for the client in
        /// the specified level.
        /// </summary>
        /// <param name="socket">Socket connected to the client.</param>
        /// <param name="level">Level to create a player in.</param>
        public ClientConnection(TcpClient socket, Level level)
        {
            _socket = socket;
            Level = level;

            _visibility = level
                .Select(x => new RoomVisibility(x))
                .ToArray();

            Player = Entity.Create("player");
            Player.GetComponent<PlayerControl>().Client = this;

            var rooms = level
                .Where(x => x.Any(y => y.State == TileState.Floor))
                .ToArray();

            var room = rooms[(int) (DateTime.Now.Ticks % rooms.Length)];

            var tiles = room
                .Where(x => x.State == TileState.Floor)
                .ToArray();

            Player.Place(tiles[Tools.Random.Next(tiles.Length)]);
        }

        /// <summary>
        /// Sends a single entity to the client.
        /// </summary>
        /// <param name="writer">Writer to write the entity to.</param>
        /// <param name="ent">Entity to send to the client.</param>
        private void SendEntity(BinaryWriter writer, Entity ent)
        {
            writer.Write(ent.ID);
            writer.Write(ent.ClassName);

            // TODO: Send component information?
        }

        /// <summary>
        /// Sends a partially observable level state update to the client.
        /// </summary>
        /// <param name="timeOffset">How far into the future the level time
        /// sent should be offset.</param>
        /// <param name="level">Level to send.</param>
        public void SendVisibleLevelState(ulong timeOffset = 0)
        {
            var time = Level.Time + timeOffset;

            try {
                var stream = _socket.GetStream();
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                    writer.Write((byte) PacketType.LevelState);

                    writer.Write(time);
                    writer.Write(Player.ID);

                    lock (Level) {
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

            // TODO: Forcibly kick the client or something.
            } catch { }
        }

        /// <summary>
        /// Sends a request for a key input and returns the response.
        /// </summary>
        /// <param name="validKeys">A set of valid keys that may be used.</param>
        /// <returns>A key entered by the client.</returns>
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

                ConsoleKey response;

                using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                    response = (ConsoleKey) reader.ReadUInt16();
                }

                if (!validKeys.Contains(response)) {
                    throw new Exception("Invalid key sent by client");
                }

                return response;
            } catch {
                // TODO: This is pretty nasty. Don't do this.
                return validKeys.First();
            }
        }
    }
}
