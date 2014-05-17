using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FacePuncher.Geometry.RoomPlacements
{
    class Default : RoomPlacement
    {
        private class RoomGeneratorInfo
        {
            public String ClassName { get; private set; }

            [ScriptDefinable]
            public int Frequency { get; set; }

            public void Initialize(XElement elem)
            {
                ClassName = elem.Attribute("class").Value;

                Definitions.LoadProperties(this, elem);
            }
        }

        private class RoomInfo
        {
            public Rectangle Rect { get; private set; }

            public String Type { get; private set; }

            public Dictionary<RoomInfo, Rectangle> Doors { get; private set; }

            public RoomInfo(Rectangle rect, String type)
            {
                Rect = rect;
                Type = type;
                Doors = new Dictionary<RoomInfo, Rectangle>();
            }

            public void AddDoor(RoomInfo room, Rectangle rect)
            {
                Doors.Add(room, rect.Intersection(Rect));
            }
        }

        private List<RoomGeneratorInfo> _roomGenerators;

        [ScriptDefinable]
        public int MinimumArea { get; set; }

        [ScriptDefinable]
        public int MaximumArea { get; set; }

        [ScriptDefinable]
        public int MinimumHubs { get; set; }

        [ScriptDefinable]
        public int MaximumHubs { get; set; }

        public Default()
        {
            MinimumArea = 1000;
            MaximumArea = 1000;

            MinimumHubs = 1;
            MaximumHubs = 1;

            _roomGenerators = new List<RoomGeneratorInfo>();
        }

        public override void LoadFromDefinition(XElement elem)
        {
            base.LoadFromDefinition(elem);

            foreach (var room in elem.Elements("Room")) {
                var info = new RoomGeneratorInfo();
                info.Initialize(room);
                _roomGenerators.Add(info);
            }
        }

        protected String GetRandomGeneratorName(Random rand)
        {
            int total = _roomGenerators.Sum(x => x.Frequency);
            int index = rand.Next(total);

            return _roomGenerators.First(x => (index -= x.Frequency) < 0).ClassName;
        }

        private Rectangle GenerateAdjacentRect(Rectangle a, Rectangle b, Random rand)
        {
            var dir = Position.Zero;
            int diff = 0, min, max;

            switch (rand.Next(4)) {
                case 0: {
                    diff = a.Height - b.Height;
                    dir = Position.UnitY;
                    b += a.TopLeft - b.TopRight;
                } break;
                case 1: {
                    diff = a.Width - b.Width;
                    dir = Position.UnitX;
                    b += a.TopLeft - b.BottomLeft;
                } break;
                case 2: {
                    diff = a.Height - b.Height;
                    dir = Position.UnitY;
                    b += a.TopRight - b.TopLeft;
                } break;
                case 3: {
                    diff = a.Width - b.Width;
                    dir = Position.UnitX;
                    b += a.BottomLeft - b.TopLeft;
                } break;
            }

            min = Math.Min(0, diff);
            max = Math.Max(0, diff);

            return b + dir * rand.Next(min, max + 1);
        }

        private Rectangle GenerateDoor(Rectangle a, Rectangle b, Random rand)
        {
            int min, max;
            var start = Position.Zero;
            var depth = Position.Zero;
            var direc = Position.Zero;

            if (a.Right == b.Left) {
                min = Math.Max(a.Top, b.Top) + 1;
                max = Math.Min(a.Bottom, b.Bottom) - 1;
                start = new Position(a.Right - 1, min);
                depth = Position.UnitX * 2; direc = Position.UnitY;
            } else if (a.Bottom == b.Top) {
                min = Math.Max(a.Left, b.Left) + 1;
                max = Math.Min(a.Right, b.Right) - 1;
                start = new Position(min, a.Bottom - 1);
                depth = Position.UnitY * 2; direc = Position.UnitX;
            } else if (a.Left == b.Right) {
                min = Math.Max(a.Top, b.Top) + 1;
                max = Math.Min(a.Bottom, b.Bottom) - 1;
                start = new Position(b.Right - 1, min);
                depth = Position.UnitX * 2; direc = Position.UnitY;
            } else if (a.Top == b.Bottom) {
                min = Math.Max(a.Left, b.Left) + 1;
                max = Math.Min(a.Right, b.Right) - 1;
                start = new Position(min, b.Bottom - 1);
                depth = Position.UnitY * 2; direc = Position.UnitX;
            } else {
                throw new Exception("Could not generate door.");
            }

            int size = max - min;

            if (size <= 2 || rand.NextDouble() < 0.25) {
                return new Rectangle(start, start + depth + direc * size);
            } else if (size == 3 || rand.NextDouble() < 0.5) {
                min = 1 + rand.Next(size - 2);
                max = min + 1;
            } else {
                min = 1 + rand.Next(size - 3);
                max = min + 2;
            }


            return new Rectangle(start + direc * min, start + depth + direc * max);
        }

        public override void PlaceRooms(Level level, Random rand)
        {
            int destArea = rand.Next(MinimumArea, MaximumArea);

            var rects = new List<RoomInfo>();

            var hubs = new Position[rand.Next(MinimumHubs, MaximumHubs + 1)];

            int range = (int) Math.Sqrt(destArea / 2);
            for (int i = 0; i < hubs.Length; ++i) {
                hubs[i] = new Position(rand.Next(-range, range), rand.Next(-range, range));
            }

            while (destArea > 0) {
                var name = GetRandomGeneratorName(rand);
                var size = new Rectangle(0, 0, rand.Next(4, 12), rand.Next(4, 12));

                var hub = hubs.Length > 0 ? hubs[rand.Next(hubs.Length)] : Position.Zero;

                var best = Rectangle.Zero;
                RoomInfo neighbour = null;

                if (rects.Count > 0) {
                    int bestDist = 0;

                    int tries = 0;
                    while (best == Rectangle.Zero || ++tries <= 256) {
                        var othr = rects[rand.Next(rects.Count)];
                        var rect = GenerateAdjacentRect(othr.Rect, size, rand);

                        if (rects.Any(x => x.Rect.Intersects(rect))) continue;

                        int dist = (rect.NearestPosition(hub) - hub).LengthSquared;
                        if (best != Rectangle.Zero && dist >= bestDist) continue;

                        best = rect;
                        bestDist = dist;
                        neighbour = othr;

                        tries = 0;
                    }
                } else {
                    best = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
                }

                var info = new RoomInfo(best, name);
                if (neighbour != null) {
                    var door = GenerateDoor(info.Rect, neighbour.Rect, rand);

                    info.AddDoor(neighbour, door);
                    neighbour.AddDoor(info, door);
                }

                rects.Add(info);
                destArea -= best.Area;
            }

            foreach (var info in rects) {
                var rect = info.Rect;
                var room = level.CreateRoom(rect);

                room.CreateWall(rect - rect.TopLeft);
                room.CreateFloor(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

                foreach (var door in info.Doors.Values) {
                    room.CreateFloor(door - rect.TopLeft);
                }
            }
        }
    }
}
