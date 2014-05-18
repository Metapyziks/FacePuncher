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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FacePuncher.Geometry.RoomPlacements
{
    class Default : RoomPlacement
    {
        private class RoomGeneratorInfo
        {
            public RoomGenerator Generator { get; private set; }

            [ScriptDefinable]
            public int Frequency { get; set; }

            public void Initialize(XElement elem)
            {
                Generator = RoomGenerator.Get(elem.Attribute("class").Value);

                Definitions.LoadProperties(this, elem);
            }
        }

        private struct Hub
        {
            public Position Position;
            public int Density;
        }

        private List<RoomGeneratorInfo> _roomGenerators;

        [ScriptDefinable]
        public int MinArea { get; set; }

        [ScriptDefinable]
        public int MaxArea { get; set; }

        [ScriptDefinable]
        public int MinHubs { get; set; }

        [ScriptDefinable]
        public int MaxHubs { get; set; }

        [ScriptDefinable]
        public float MinConnectivity { get; set; }

        [ScriptDefinable]
        public float MaxConnectivity { get; set; }

        [ScriptDefinable]
        public int MinHubDensity { get; set; }

        [ScriptDefinable]
        public int MaxHubDensity { get; set; }

        public Default()
        {
            MinArea = 1000;
            MaxArea = 1000;

            MinHubs = 1;
            MaxHubs = 1;

            MinConnectivity = 0f;
            MaxConnectivity = 1f;

            MinHubDensity = 64;
            MaxHubDensity = 64;

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

        protected RoomGenerator GetRandomGenerator(Random rand)
        {
            int total = _roomGenerators.Sum(x => x.Frequency);
            int index = rand.Next(total);

            return _roomGenerators.First(x => (index -= x.Frequency) < 0).Generator;
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

            if (size <= 2 || rand.NextDouble() < 0.125) {
                return new Rectangle(start, start + depth + direc * size);
            } else if (size == 3 || rand.NextDouble() < 0.75) {
                min = 1 + rand.Next(size - 2);
                max = min + 1;
            } else {
                min = 1 + rand.Next(size - 3);
                max = min + 2;
            }

            return new Rectangle(start + direc * min, start + depth + direc * max);
        }

        private bool CanGenerateDoor(RoomPlan a, RoomPlan b)
        {
            if (a.Index >= b.Index) return false;
            if (!a.Rect.IsAdjacent(b.Rect)) return false;

            var i = a.Rect.Intersection(b.Rect);

            return i.Width >= 3 || i.Height >= 3;
        }

        private int Separation(RoomPlan a, RoomPlan b)
        {
            var path = Tools.AStar<RoomPlan>(a, b,
                x => x.Doors.Keys.Select(y => Tuple.Create(y, (x.Rect.TopLeft - y.Rect.TopLeft).ManhattanLength)),
                x => x.Rect.TopLeft + new Position(x.Rect.Width / 2, x.Rect.Height / 2));

            return path.Length;
        }

        private int _progressLeft;
        private void ProgressStart(String message)
        {
            Console.Write(message);

            _progressLeft = Console.CursorLeft;
            _prevProgress = 0;

            Console.Write("0%");
        }

        private int _prevProgress;
        private void ProgressUpdate(int cur, int dest)
        {
            int perc = (cur * 100) / dest;

            if (perc != _prevProgress) {
                _prevProgress = perc;
                Console.CursorLeft = _progressLeft;
                Console.Write("{0}%", perc);
            }
        }

        private void ProgressEnd()
        {
            ProgressUpdate(1, 1);
            Console.WriteLine();
        }

        public override IEnumerable<RoomPlan> Generate(Level level, Random rand)
        {
            int destArea = rand.Next(MinArea, MaxArea);

            var plans = new List<RoomPlan>();

            int range = (int) Math.Sqrt(destArea);
            var hubs = new Hub[Math.Max(1, rand.Next(MinHubs, MaxHubs + 1))];

            for (int i = 0; i < hubs.Length; ++i) {
                hubs[i].Position = new Position(rand.Next(-range, range), rand.Next(-range, range));
                hubs[i].Density = rand.Next(MinHubDensity, MaxHubDensity);
            }

            ProgressStart("Creating rooms: ");

            int area = 0;
            while (area < destArea) {
                var generator = GetRandomGenerator(rand);
                var size = new Rectangle(Position.Zero, generator.RoomLayout.GenerateSize(rand));

                var hub = hubs[rand.Next(hubs.Length)];
                var hubPos = hub.Position;

                var best = Rectangle.Zero;
                RoomPlan neighbour = null;

                if (plans.Count > 0) {
                    int bestDist = 0;

                    int tries = 0;
                    while (best == Rectangle.Zero || ++tries <= hub.Density) {
                        var othr = plans[rand.Next(plans.Count)];
                        var rect = GenerateAdjacentRect(othr.Rect, size, rand);

                        if (plans.Any(x => x.Rect.Intersects(rect))) continue;

                        int dist = (rect.NearestPosition(hubPos) - hubPos).ManhattanLength;
                        if (best != Rectangle.Zero && dist >= bestDist) continue;

                        best = rect;
                        bestDist = dist;
                        neighbour = othr;

                        tries = 0;
                    }
                } else {
                    best = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
                }

                var info = new RoomPlan(best, generator, plans.Count);
                if (neighbour != null) {
                    var door = GenerateDoor(info.Rect, neighbour.Rect, rand);

                    info.AddDoor(neighbour, door);
                    neighbour.AddDoor(info, door);
                }

                plans.Add(info);
                area += best.Area;

                ProgressUpdate(area, destArea);
            }

            ProgressEnd();

            var spareDoors = plans
                .SelectMany(x => plans
                    .Where(y => CanGenerateDoor(x, y) && !x.Doors.ContainsKey(y))
                    .Select(y => Tuple.Create(x, y)))
                .ToList();

            ProgressStart("Creating Doors: ");

            int doorCount = Tools.Clamp((int) (rand.NextFloat(MinConnectivity, MaxConnectivity) * spareDoors.Count), 0, spareDoors.Count);
            for (int d = 0; d < doorCount; ++d) {
                Tuple<RoomPlan, RoomPlan> door = null;
                int bestScore = 0;
                foreach (var elem in spareDoors) {
                    var score = Separation(elem.Item1, elem.Item2);
                    score = score * score + rand.Next(50);

                    if (score > bestScore) {
                        door = elem;
                        bestScore = score;
                    }
                }

                spareDoors.Remove(door);

                var rect = GenerateDoor(door.Item1.Rect, door.Item2.Rect, rand);
                
                door.Item1.AddDoor(door.Item2, rect);
                door.Item2.AddDoor(door.Item1, rect);

                ProgressUpdate(d + 1, doorCount);
            }

            ProgressEnd();

            return plans;
        }
    }
}
