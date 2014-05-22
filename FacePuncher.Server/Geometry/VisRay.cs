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

using System.Collections.Generic;
using System.Linq;

namespace FacePuncher.Geometry
{
    static partial class Visibility
    {
        private Tile _tile;

        private Position _start;

        private Vector2 _min;
        private Vector2 _max;

        public static IEnumerable<Tile> Cast(Tile tile, int radius)
        {
            var pos = tile.Position + new Vector2(0.5f, 0.5f);

            return new[] { tile }
                .Concat(CastSE(tile.GetNeighbour(Direction.South), pos, Vector2.UnitX, Vector2.UnitY, radius));
              //.Concat(Cast(tile.GetNeighbour(Direction.West),  pos,  Vector2.UnitY, -Vector2.UnitX, radius))
              //.Concat(Cast(tile.GetNeighbour(Direction.North), pos, -Vector2.UnitX, -Vector2.UnitY, radius))
              //.Concat(Cast(tile.GetNeighbour(Direction.East),  pos, -Vector2.UnitY,  Vector2.UnitX, radius));
        }

        private static IEnumerable<Tile> CastSE(Tile tile, Vector2 origin, Vector2 l, Vector2 r, int radius)
        {
            int xorg = (int) origin.X; int xmin = tile.Y;
            int yorg = (int) origin.X; int ymin = tile.Y;
            
            var room = tile.Room;

            var lprp = new Vector2(-l.Y, l.X);
            var rprp = new Vector2(l.Y, -l.X);

            if (r.Dot(lprp) <= 0) yield break;
            if (l.Dot(rprp) <= 0) yield break;

            int ymax = ymin + radius;
            for (int y = ymin; y <= ymax; ++y) {
                int dy = y - yorg;

                for (int x = y == ymin ? xmin : xorg; ; ++x) {
                    int dx = x - xorg;
                    if (dx * dx + dy * dy > radius * radius) break;

                    var ldif = new Vector2(x + 1, y) - origin;
                    var rdif = new Vector2(x, y + 1) - origin;

                    if (ldif.Dot(lprp) < 0) break;
                    if (rdif.Dot(rprp) < 0) continue;

                    tile = room[x - room.Left, y - room.Top];
                    room = tile.Room;

                    yield return tile;

                    if (tile.State != TileState.Floor) {
                        foreach (var t in CastSE(tile, origin, l, ldif, radius)) {
                            yield return t;
                        }

                        foreach (var t in CastSE(tile, origin, rdif, r, radius)) {
                            yield return t;
                        }

                        yield break;
                    }
                }
            }
        }
    }
}
