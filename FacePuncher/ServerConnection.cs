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

using FacePuncher.Geometry;
using FacePuncher.Graphics;
using FacePuncher.Network;
using FacePuncher.UI;

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

        public double Time { get; private set; }

        public Frame InventoryView { get; private set; }

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
            InventoryView = null;
        }

        public void CloseInventory()
        {
            InventoryView = null;
        }

        /// <summary>
        /// Reads a partially observable level state update from the server.
        /// </summary>
        private async Task ReadVisibleLevelState(NetworkStream stream)
        {
            Time = await _stream.ReadDouble();
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

            Interface.Display.Write(0, 0, String.Format("{0:F2}", Time));
        }

        private async Task ReadInventoryContents(NetworkStream stream)
        {
            var inv = await InventoryListing.Read(stream);

            InventoryView = Widget.Create<Frame>("InventoryListing", "inventory");

            int i = 0;
            foreach (var item in inv) {
                var entry = Widget.Create<Panel>("InventoryEntry", "entry_" + i++);

                entry.Position = new Position(1, i * 2 + 2);
                ((Label) entry["name"]).Text = item.Name;
                ((Label) entry["weight"]).Text = item.Weight.ToString("F2");
                ((Label) entry["value"]).Text = item.Value.ToString();
                ((Label) entry["material"]).Text = item.Material ?? "";

                InventoryView.AddChild(entry);
            }

            Program.Draw(this);
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
                case ServerPacketType.InventoryContents:
                    await ReadInventoryContents(_stream);
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
