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

namespace FacePuncher.Geometry
{
    static partial class Visibility
    {
        public static IEnumerable<Tile> Cast(Tile tile, int radius)
        {
            var pos = tile.Position + new Vector2(0.5f, 0.5f);

            return new[] { tile }
                .Concat(Cast(tile, pos, Vector2.UnitX, Vector2.UnitY, radius))
                .Concat(Cast(tile, pos, Vector2.UnitY, -Vector2.UnitX, radius))
                .Concat(Cast(tile, pos, -Vector2.UnitX, -Vector2.UnitY, radius))
                .Concat(Cast(tile,  pos, -Vector2.UnitY,  Vector2.UnitX, radius));
        }

        private static IEnumerable<Tile> Cast(Tile tile, Vector2 origin, Vector2 l, Vector2 r, int radius)
        {
            const float error = 1 / 256f;

            var orig = tile;

            int xorg = (int) Math.Floor(origin.X); int xmin = tile.X;
            int yorg = (int) Math.Floor(origin.Y); int ymin = tile.Y;
            
            var room = tile.Room;

            var lprp = new Vector2(-l.Y, l.X);
            var rprp = new Vector2(r.Y, -r.X);

            if (r.Dot(lprp) < 0) yield break;
            if (l.Dot(rprp) < 0) yield break;

            int xadd = (l + r).X >= 0 ? 1 : -1;
            int yadd = (l + r).Y >= 0 ? 1 : -1;

            var loff = new Position((yadd + 1) / 2, 1 - (xadd + 1) / 2);
            var roff = new Position(1 - loff.X, 1 - loff.Y);

            bool rightLeading = xadd != yadd;

            int ymax = yorg + (radius + 1) * yadd;
            for (int y = ymin; y != ymax; y += yadd) {
                int dy = y - yorg;

                for (int x = y == ymin ? xmin : xorg; ; x += xadd) {
                    int dx = x - xorg;
                    if (dx * dx + dy * dy > radius * radius) break;

                    var ldif = loff + new Vector2(x, y) - origin;
                    var rdif = roff + new Vector2(x, y) - origin;

                    if (rdif.Dot(lprp) <= error) {
                        if (rightLeading) continue;
                        else break;
                    }

                    if (ldif.Dot(rprp) <= error) {
                        if (rightLeading) break;
                        else continue;
                    }

                    if (x == orig.X && y == orig.Y) continue;

                    tile = room[x - room.Left, y - room.Top];

                    yield return tile;

                    if (tile.State != TileState.Floor) {
                        foreach (var t in Cast(tile, origin, l, ldif, radius)) {
                            yield return t;
                        }

                        foreach (var t in Cast(tile, origin, rdif, r, radius)) {
                            yield return t;
                        }

                        yield break;
                    }

                    room = tile.Room;
                }
            }
        }
    }
}
