using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using FacePuncher.Entities;
using FacePuncher.Geometry;
using System.Threading.Tasks;
using FacePuncher.Network;

namespace FacePuncher
{
    /// <summary>
    /// Handles communication with a game server.
    /// </summary>
    class ServerConnection : Connection, IDisposable
    {
        private List<RoomVisibility> _visibility;

        private uint _playerID;

        /// <summary>
        /// Gets the partially observed state of the current level
        /// the local player is within.
        /// </summary>
        public Level Level { get; private set; }

        /// <summary>
        /// Gets the local player's entity.
        /// </summary>
        public Entity Player { get; private set; }

        /// <summary>
        /// Gets the server time corresponding to the most recent game
        /// state update.
        /// </summary>
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
            Level = new Level();

            _visibility = new List<RoomVisibility>();
        }

        /// <summary>
        /// Reads a single entity from the server.
        /// </summary>
        /// <param name="reader">Reader to read the entity from.</param>
        /// <returns>An entity received from the server.</returns>
        private async Task<Entity> ReadEntity(NetworkStream stream)
        {
            uint id = await stream.ReadUInt32();
            string className = await stream.ReadString();

            if (id == _playerID && Player != null)
            {
                return Player;
            }

            // TODO: Temporary, should cache entities.
            var ent = Entity.Create(id, className);

            if (id == _playerID) Player = ent;

            return ent;
        }

        /// <summary>
        /// Reads a partially observable level state update from the server.
        /// </summary>
        private async Task ReadVisibleLevelState(NetworkStream stream)
        {
            Time = await stream.ReadUInt64();
            _playerID = await stream.ReadUInt32();

            // removed Level lock
            {
                int roomCount = await stream.ReadInt32();
                for (int i = 0; i < roomCount; ++i)
                {
                    // Each room is identified by its rectangle.
                    var rect = await stream.ReadRectangle();

                    var vis = _visibility.FirstOrDefault(x => x.Room.Rect == rect);

                    if (vis == null)
                    {
                        var room = Level.CreateRoom(rect);
                        vis = new RoomVisibility(room);

                        _visibility.Add(vis);
                    }

                    int tileCount = await stream.ReadInt32();
                    for (int j = 0; j < tileCount; ++j)
                    {
                        var pos = await stream.ReadPosition();
                        var state = (TileState)await stream.ReadByteAsync();
                        var tile = vis.Room[pos];

                        vis.Reveal(pos, Time);
                        tile.State = state;

                        // The fact that this here mutates is seriously annoying.
                        var ents = tile.ToArray();
                        foreach (var ent in ents)
                        {
                            ent.Remove();
                        }

                        var entCount = await stream.ReadUInt16();
                        for (int k = 0; k < entCount; ++k)
                        {
                            var ent = await ReadEntity(stream);
                            ent.Place(tile);
                        }
                    }
                }
            }

            Program.Draw(this);

            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.Write(Time);
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
