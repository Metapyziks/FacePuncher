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

        private Rectangle RandomAdjacentRect(Rectangle a, Rectangle b, Random rand)
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

        public override void PlaceRooms(Level level, Random rand)
        {
            int destArea = rand.Next(MinimumArea, MaximumArea);

            var rects = new List<Tuple<Rectangle, String>>();

            var hubs = new Position[rand.Next(MinimumHubs, MaximumHubs - MinimumHubs + 1)];

            int range = (int) Math.Sqrt(destArea);
            for (int i = 0; i < hubs.Length; ++i) {
                hubs[i] = new Position(rand.Next(-range, range), rand.Next(-range, range));
            }

            while (destArea > 0) {
                var name = GetRandomGeneratorName(rand);
                var size = new Rectangle(0, 0, rand.Next(4, 12), rand.Next(4, 12));

                var hub = hubs.Length > 0 ? hubs[rand.Next(hubs.Length)] : Position.Zero;

                var best = Rectangle.Zero;

                if (rects.Count > 0) {
                    int bestDist = 0;
                    int tries = 0;
                    while (best == Rectangle.Zero || ++tries <= 256) {
                        var othr = rects[rand.Next(rects.Count)].Item1;
                        var rect = RandomAdjacentRect(othr, size, rand);

                        if (rects.Any(x => x.Item1.Intersects(rect))) continue;

                        int dist = (rect.NearestPosition(hub) - hub).LengthSquared;
                        if (best != Rectangle.Zero && dist >= bestDist) continue;

                        best = rect;
                        bestDist = dist;
                        tries = 0;
                    }
                } else {
                    best = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
                }

                rects.Add(Tuple.Create(best, name));
                destArea -= best.Area;
            }

            foreach (var info in rects) {
                var rect = info.Item1;
                var room = level.CreateRoom(rect);

                room.CreateWall(rect - rect.TopLeft);
                room.CreateFloor(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

                foreach (var neighbour in rects.Where(x => x.Item1.Right == rect.Left)) {
                    var a = Math.Max(neighbour.Item1.Top, rect.Top) + 1;
                    var b = Math.Min(neighbour.Item1.Bottom, rect.Bottom) - 1;
                    if (b - a >= 2) {
                        room.CreateFloor(new Rectangle(0, a - rect.Top, 1, b - a));
                    }
                }

                foreach (var neighbour in rects.Where(x => x.Item1.Bottom == rect.Top)) {
                    var a = Math.Max(neighbour.Item1.Left, rect.Left) + 1;
                    var b = Math.Min(neighbour.Item1.Right, rect.Right) - 1;
                    if (b - a >= 2) {
                        room.CreateFloor(new Rectangle(a - rect.Left, 0, b - a, 1));
                    }
                }

                foreach (var neighbour in rects.Where(x => x.Item1.Left == rect.Right)) {
                    var a = Math.Max(neighbour.Item1.Top, rect.Top) + 1;
                    var b = Math.Min(neighbour.Item1.Bottom, rect.Bottom) - 1;
                    if (b - a >= 2) {
                        room.CreateFloor(new Rectangle(rect.Width - 1, a - rect.Top, 1, b - a));
                    }
                }

                foreach (var neighbour in rects.Where(x => x.Item1.Top == rect.Bottom)) {
                    var a = Math.Max(neighbour.Item1.Left, rect.Left) + 1;
                    var b = Math.Min(neighbour.Item1.Right, rect.Right) - 1;
                    if (b - a >= 2) {
                        room.CreateFloor(new Rectangle(a - rect.Left, rect.Height - 1, b - a, 1));
                    }
                }
            }
        }
    }
}
