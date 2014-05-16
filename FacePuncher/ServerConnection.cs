using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    /// <summary>
    /// Handles communication with a game server.
    /// </summary>
    class ServerConnection : IDisposable
    {
        private TcpClient _socket;
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
        {
            _visibility = new List<RoomVisibility>();
            _socket = new TcpClient(hostname, port);
        }

        /// <summary>
        /// Reads a partially observable level state update from the server.
        /// </summary>
        private void ReadVisibleLevelState()
        {
            var stream = _socket.GetStream();
            using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                Time = reader.ReadUInt64();
                PlayerPosition = reader.ReadPosition();

                lock (_visibility) {
                    int roomCount = reader.ReadInt32();
                    for (int i = 0; i < roomCount; ++i) {
                        // Each room is identified by its rectangle.
                        var rect = reader.ReadRectangle();

                        var vis = _visibility.FirstOrDefault(x => x.Rect == rect);

                        if (vis == null) {
                            vis = new RoomVisibility(rect);
                            _visibility.Add(vis);
                        }

                        int tileCount = reader.ReadInt32();
                        for (int j = 0; j < tileCount; ++j) {
                            var pos = reader.ReadPosition();
                            vis.Reveal(pos, new TileAppearance(pos, stream), Time);
                        }
                    }

                    LoadedLevel = true;
                }
            }
        }

        /// <summary>
        /// Fulfills a key input request from the server using console input.
        /// </summary>
        private void SendInput()
        {
            ConsoleKey[] validKeys;

            var stream = _socket.GetStream();
            using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                validKeys = new ConsoleKey[reader.ReadUInt16()];
                for (int i = 0; i < validKeys.Length; ++i) {
                    validKeys[i] = (ConsoleKey) reader.ReadUInt16();
                }
            }

            // Clear any buffered key inputs.
            while (Console.KeyAvailable) Console.ReadKey(true);

            ConsoleKey key;
            do {
                key = Console.ReadKey(true).Key;
            } while (!validKeys.Contains(key));

            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                writer.Write((ushort) key);
                writer.Flush();
            }
        }

        /// <summary>
        /// Processes a single packet incident from the server.
        /// </summary>
        /// <returns>True if further packets may be accepted, false otherwise.</returns>
        public bool ProcessPacket()
        {
            var stream = _socket.GetStream();
            var packetType = (PacketType) stream.ReadByte();

            switch (packetType) {
                case PacketType.LevelState:
                    ReadVisibleLevelState(); break;
                case PacketType.InputRequest:
                    SendInput(); break;
                default:
                    throw new Exception("Unexpected packet type");
            }

            return true;
        }

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _socket.Close();
        }
    }
}
