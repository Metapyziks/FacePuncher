/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

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
        /// Sends a partially observable level state update to the client.
        /// </summary>
        /// <param name="timeOffset">How far into the future the level time
        /// sent should be offset.</param>
        /// <param name="level">Level to send.</param>
        public void SendVisibleLevelState(ulong timeOffset = 0)
        {
            var time = Level.Time + timeOffset;

            var stream = _socket.GetStream();
            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                writer.Write((byte) PacketType.LevelState);

                writer.Write(time);
                writer.Write(Player.Position);

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
                            tile.Appearance.WriteToStream(stream);
                        }
                    }
                }

                writer.Flush();
            }
        }

        /// <summary>
        /// Sends a request for a key input and returns the response.
        /// </summary>
        /// <param name="validKeys">A set of valid keys that may be used.</param>
        /// <returns>A key entered by the client.</returns>
        public ConsoleKey ReadInput(ConsoleKey[] validKeys)
        {
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
        }
    }
}
