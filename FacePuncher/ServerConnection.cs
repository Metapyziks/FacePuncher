/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Tamme Schichler (tammeschichler@googlemail.com)
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Geometry;
using System.Threading.Tasks;
using FacePuncher.Network;
using FacePuncher.Graphics;

namespace FacePuncher
{
    /// <summary>
    /// Handles communication with a game server.
    /// </summary>
    class ServerConnection : Connection, IDisposable
    {
        private List<RoomVisibility> _visibility;

        public Position PlayerPosition { get; private set; }

        public bool LoadedLevel { get; private set; }

        public ulong Time { get; private set; }

        /// <summary>
        /// Gets a set of visibility masks for rooms that are either
        /// currently visible or have been seen in the past.
        /// </summary>
        public IEnumerable<RoomVisibility> Visibility { get { return _visibility; } }

        /// <summary>
        /// Creates a new connection to the specified host and port.
        /// </summary>
        /// <param name="hostname">Hostname of the desired server.</param>
        /// <param name="port">Port number of the desired server.</param>
        public ServerConnection(String hostname, int port)
            : base(new TcpClient(hostname, port))
        {
            _visibility = new List<RoomVisibility>();
        }

        /// <summary>
        /// Reads a partially observable level state update from the server.
        /// </summary>
        private async Task ReadVisibleLevelState(NetworkStream stream)
        {
            Time = await _stream.ReadUInt64();
            PlayerPosition = await _stream.ReadPosition();

            // removed lock
            int roomCount = await _stream.ReadInt32();
            for (int i = 0; i < roomCount; ++i)
            {
                // Each room is identified by its rectangle.
                var rect = await _stream.ReadRectangle();

                var vis = _visibility.FirstOrDefault(x => x.Rect == rect);

                if (vis == null)
                {
                    vis = new RoomVisibility(rect);
                    _visibility.Add(vis);
                }

                int tileCount = await _stream.ReadInt32();
                for (int j = 0; j < tileCount; ++j)
                {
                    var pos = await _stream.ReadPosition();
                    vis.Reveal(pos, await TileAppearance.Read(pos, stream), Time);
                }
            }

            LoadedLevel = true;


            Program.Draw(this);

            Interface.Display.Write(0, 0, Time.ToString());
        }

        /// <summary>
        /// Processes a single packet incident from the server.
        /// </summary>
        protected override async Task HandlePushedPacket()
        {
            var packetType = (ServerPacketType)await _stream.ReadByteAsync();

            switch (packetType)
            {
                case ServerPacketType.LevelState:
                    await ReadVisibleLevelState(_stream);
                    break;
                default:
                    throw new Exception("Unexpected packet type");
            }
        }

        internal void SendIntent(Intent intent)
        {
            _stream.Write((byte)0); // Pushed packet
            _stream.Write((byte)ClientPacketType.PlayerIntent);
            _stream.WriteProtoBuf(intent);
            _stream.Flush();
        }
    }
}
