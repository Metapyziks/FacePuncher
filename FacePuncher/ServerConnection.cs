using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    class ServerConnection : IDisposable
    {
        private TcpClient _socket;
        private List<RoomVisibility> _visibility;

        private uint _playerID;

        public Level Level { get; private set; }
        public Entity Player { get; private set; }
        public ulong Time { get; private set; }

        public IEnumerable<RoomVisibility> Visibility { get { return _visibility; } }
        
        public ServerConnection(String hostname, int port)
        {
            Level = new Level();
            _visibility = new List<RoomVisibility>();

            _socket = new TcpClient();
            _socket.Connect(hostname, port);
        }

        private Entity ReadEntity(BinaryReader reader)
        {
            uint id = reader.ReadUInt32();
            string className = reader.ReadString();

            if (id == _playerID && Player != null) {
                return Player;
            }

            // Temporary, should cache entities
            var ent = Entity.Create(id, className);

            if (id == _playerID) Player = ent;

            return ent;
        }

        private void ReadVisibleLevelState()
        {
            var stream = _socket.GetStream();
            using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                Time = reader.ReadUInt64();
                _playerID = reader.ReadUInt32();

                lock (Level) {
                    int roomCount = reader.ReadInt32();
                    for (int i = 0; i < roomCount; ++i) {
                        var rect = reader.ReadRectangle();

                        var vis = _visibility.FirstOrDefault(x => x.Room.Rect == rect);

                        if (vis == null) {
                            var room = Level.CreateRoom(rect);
                            vis = new RoomVisibility(room);

                            _visibility.Add(vis);
                        }

                        int tileCount = reader.ReadInt32();
                        for (int j = 0; j < tileCount; ++j) {
                            var pos = reader.ReadPosition();
                            var state = (TileState) reader.ReadByte();
                            var tile = vis.Room[pos];

                            vis.Reveal(pos, Time);
                            tile.State = state;

                            var ents = tile.ToArray();
                            foreach (var ent in ents) {
                                ent.Remove();
                            }

                            var entCount = reader.ReadUInt16();
                            for (int k = 0; k < entCount; ++k) {
                                var ent = ReadEntity(reader);
                                ent.Place(tile);
                            }
                        }
                    }
                }
            }
        }

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

            ConsoleKey key;

            while (Console.KeyAvailable) Console.ReadKey(true);

            do {
                key = Console.ReadKey(true).Key;
            } while (!validKeys.Contains(key));

            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) {
                writer.Write((ushort) key);
                writer.Flush();
            }
        }

        public void ProcessPackets()
        {
            var stream = _socket.GetStream();
            while (true) {
                var packetType = (PacketType) stream.ReadByte();

                switch (packetType) {
                    case PacketType.LevelState:
                        ReadVisibleLevelState(); break;
                    case PacketType.InputRequest:
                        SendInput(); break;
                    default:
                        throw new Exception("Unexpected packet type");
                }
            }
        }

        public void Dispose()
        {
            _socket.Close();
        }
    }
}
