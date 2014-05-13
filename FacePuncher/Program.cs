using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

using FacePuncher.Entities;
using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    class Program
    {
        private static TcpClient _socket;
        private static List<RoomVisibility> _visibility;

        private static Level _level;
        private static Entity _player;

        private static ulong _time;

        static void ReadVisibleLevelState()
        {
            var stream = _socket.GetStream();
            var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true);

            _time = reader.ReadUInt64();
            var plyPos = reader.ReadPosition();

            lock (_level) {
                int roomCount = reader.ReadInt32();
                for (int i = 0; i < roomCount; ++i) {
                    var rect = reader.ReadRectangle();

                    var vis = _visibility.FirstOrDefault(x => x.Room.Rect == rect);

                    if (vis == null) {
                        var room = _level.CreateRoom(rect);
                        vis = new RoomVisibility(room);

                        _visibility.Add(vis);
                    }

                    int tileCount = reader.ReadInt32();
                    for (int j = 0; j < tileCount; ++j) {
                        var pos = reader.ReadPosition();
                        var state = (TileState) reader.ReadByte();

                        vis.Reveal(pos, _time);
                        vis.Room[pos].State = state;
                    }
                }
            }

            reader.Close();

            if (_player == null) {
                _player = Entity.Create("player");
                _player.Place(_level[plyPos]);
            }
        }

        static void Main(string[] args)
        {
            Display.Initialize(96, 32);

            Entity.Register("player", ent => {
                ent.AddComponent<StaticDrawable>()
                    .SetLayer(DrawableLayer.Characters)
                    .SetSymbol('@')
                    .SetForeColor(ConsoleColor.Yellow);
            });

            var rand = new Random();

            var dustSymbols = new[] {
                ',', '.', '`', '\''
            };

            Entity.Register("dust", ent => {
                ent.AddComponent<StaticDrawable>()
                    .SetLayer(DrawableLayer.Debris)
                    .SetSymbol(dustSymbols[rand.Next(dustSymbols.Length)])
                    .SetForeColor(ConsoleColor.DarkGray);
            });

            _level = new Level();
            _visibility = new List<RoomVisibility>();

            _socket = new TcpClient();
            _socket.Connect("localhost", 14242);

            var halfSize = new Position(Display.Width / 2, Display.Height / 2);

            int flash = 0;
            var renderTimer = new Timer(state => {
                Display.Clear();

                lock (_level) {
                    if (_player != null) {
                        var attribs = new DrawAttributes(_time, flash++);
                        var rect = Display.Rect + _player.Position - halfSize;

                        foreach (var vis in _visibility) {
                            vis.Draw(rect, Position.Zero, attribs);
                        }
                    }
                }

                Display.Refresh();
            }, null, 0, 125);

            ReadVisibleLevelState();
            
            Console.ReadKey(true);
        }
    }
}
