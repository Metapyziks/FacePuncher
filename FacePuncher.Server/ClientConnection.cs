/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using FacePuncher.Entities;
using FacePuncher.Geometry;
using FacePuncher.Network;

namespace FacePuncher
{
    /// <summary>
    /// Handles communication with an individual client.
    /// </summary>
    class ClientConnection : Connection
    {
        /// <summary>
        /// Maximum range of a clients visibility.
        /// 
        /// TODO: Move to become a property of the player character.
        /// </summary>
        const float MaxVisibilityRange = 8.5f;

        private TcpClient _socket;
        private Dictionary<Room, RoomVisibility> _visibility;

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
            : base(socket)
        {
            _socket = socket;
            Level = level;

            _visibility = level
                .ToDictionary(x => x, x => new RoomVisibility(x));

            Player = Entity.Create("player");
            Player.GetComponent<PlayerControl>().Client = this;

            var rooms = level
                .Where(x => x.Any(y => y.State == TileState.Floor))
                .ToArray();

            var room = rooms[(int)(DateTime.Now.Ticks % rooms.Length)];

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
        public void SendVisibleLevelState(double timeOffset = 0)
        {
            _stream.Write((byte)0); // Pushed packet
            _stream.Write((byte)ServerPacketType.LevelState);

            var time = Level.Time + timeOffset;

            _stream.Write(time);
            _stream.Write(Player.Position);

            lock (Level)
            {
                foreach (var tile in Visibility.Cast(Player.Tile, MaxVisibilityRange)) {
                    if (tile.Room == null) continue;
                    _visibility[tile.Room].Reveal(tile, time);
                }

                var visibleRooms = _visibility.Values
                    .Where(x => x.LastVisibleTime == time)
                    .ToArray();

                _stream.Write(visibleRooms.Length);
                foreach (var vis in visibleRooms)
                {
                    _stream.Write(vis.Room.Rect);

                    var visibleTiles = vis.GetVisible(time).ToArray();

                    _stream.Write(visibleTiles.Length);
                    foreach (var tile in visibleTiles)
                    {
                        _stream.Write(tile.RelativePosition);
                        tile.Appearance.WriteToStream(_stream);
                    }
                }
            }

            _stream.Flush();
        }

        protected override async Task HandlePushedPacket()
        {
            switch ((ClientPacketType)await _stream.ReadByteAsync())
            {
                case ClientPacketType.PlayerIntent:
                    Player.GetComponent<PlayerControl>().Intent = await _stream.ReadProtoBuf<Intent>();
                    break;
                default:
                    break;
            }
        }
    }
}
