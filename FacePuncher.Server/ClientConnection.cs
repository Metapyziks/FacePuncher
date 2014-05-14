using System;
using System.Collections.Generic;
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
        /// Creates a new ClientConnection instance using a socket,
        /// additionally creating a player entity for the client in
        /// the specified level.
        /// </summary>
        /// <param name="socket">Socket connected to the client.</param>
        /// <param name="level">Level to create a player in.</param>
        public ClientConnection(TcpClient socket, Level level)
        {
            _socket = socket;

            // Initialize a room visibility object for each room.
            _visibility = level
                .Select(x => new RoomVisibility(x))
                .ToArray();

            // Create a player entity and attach control to this client.
            Player = Entity.Create("player");
            Player.GetComponent<PlayerControl>()
                .SetClient(this);

            // Find all rooms that the player may be placed in.
            var rooms = level
                .Where(x => x.Any(y => y.State == TileState.Floor))
                .ToArray();

            // Select a random room to place the player in.
            var room = rooms[(int) (DateTime.Now.Ticks % rooms.Length)];

            // Find all tiles within that room that the player may be
            // placed on.
            var tiles = room
                .Where(x => x.State == TileState.Floor)
                .ToArray();

            // Place the player on a random tile.
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
        /// <param name="level">Level to send.</param>
        /// <param name="time">Current game time.</param>
        public void SendVisibleLevelState(Level level, ulong time)
        {
            try {
                var stream = _socket.GetStream();
                using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                    // Send packet identifier.
                    writer.Write((byte) PacketType.LevelState);

                    // Send general game info.
                    writer.Write(time);
                    writer.Write(Player.ID);

                    lock (level) {
                        // Update the visibility of each room, and prepare
                        // an array of visible rooms to send.
                        var visibleRooms = _visibility
                            .Where(x => x.UpdateVisibility(Player.Position, MaxVisibilityRange, time))
                            .ToArray();

                        // Send each of the visible rooms.
                        writer.Write(visibleRooms.Length);
                        foreach (var vis in visibleRooms) {
                            // Send the room's rectangle to identify it.
                            writer.Write(vis.Room.Rect);

                            var visibleTiles = vis.GetVisible(time).ToArray();

                            // Send each visible tile in the room.
                            writer.Write(visibleTiles.Length);
                            foreach (var tile in visibleTiles) {
                                writer.Write(tile.RelativePosition);
                                writer.Write((byte) tile.State);

                                // Send each entity on the tile.
                                writer.Write((ushort) tile.EntityCount);
                                foreach (var ent in tile) {
                                    SendEntity(writer, ent);
                                }
                            }
                        }
                    }

                    // Make sure the update is sent now.
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
                    // Send packet identifier.
                    writer.Write((byte) PacketType.InputRequest);

                    // Send the array of valid keys.
                    writer.Write((ushort) validKeys.Length);
                    foreach (var key in validKeys) {
                        writer.Write((ushort) key);
                    }

                    // Make sure the update is sent now.
                    writer.Flush();
                }

                ConsoleKey response;

                // Receive the response.
                using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                    response = (ConsoleKey) reader.ReadUInt16();
                }

                // Check if the client attempted to screw us.
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
