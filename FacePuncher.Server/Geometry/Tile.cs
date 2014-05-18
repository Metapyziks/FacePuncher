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

using FacePuncher.Entities;
using FacePuncher.Graphics;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Enumeration of the possible solidity states of a tile.
    /// </summary>
    public enum TileState
    {
        Void = 0,
        Wall = 1,
        Floor = 2
    }

    /// <summary>
    /// Class representing an individual tile in a room.
    /// </summary>
    public class Tile : IEnumerable<Entity>
    {
        static readonly char[] _sWallTiles;

        /// <summary>
        /// Default void tile to be used when requesting a tile outside
        /// of any rooms in a level.
        /// </summary>
        public static readonly Tile Default = new Tile(null, 0, 0);

        static Tile()
        {
            _sWallTiles = new char[256];

            char[] chars = new[] {
                '\u006f', '\u006f', '\u006f', '\u00c9',
                '\u006f', '\u00cd', '\u00bb', '\u00cb',
                '\u006f', '\u00c8', '\u00ba', '\u00cc',
                '\u00bc', '\u00ca', '\u00b9', '\u00ce'
            };

            for (int i = 0; i < 256; ++i) {
                int diags = i >> 4;
                int flats = i & 0xf;

                int corns = flats & (flats >> 1 | flats << 3) & diags;
                flats ^= corns & (corns << 1 | corns >> 3);

                _sWallTiles[i] = chars[flats];
            }
        }

        private TileState _state;
        private List<Entity> _entities;
        private TileAppearance _appearance;
        private ulong _lastAppearance;

        /// <summary>
        /// Parent room containing this tile.
        /// </summary>
        public Room Room { get; private set; }

        /// <summary>
        /// Position of the tile relative to its containing room.
        /// </summary>
        public Position RelativePosition { get; private set; }

        /// <summary>
        /// Horizontal position of the tile relative to its containing room.
        /// </summary>
        public int RelativeX { get { return RelativePosition.X; } }

        /// <summary>
        /// Vertical position of the tile relative to its containing room.
        /// </summary>
        public int RelativeY { get { return RelativePosition.Y; } }

        /// <summary>
        /// Position of the tile relative to the level origin.
        /// </summary>
        public Position Position { get { return Room.Rect.TopLeft + RelativePosition; } }

        /// <summary>
        /// Horizontal position of the tile relative to the level origin.
        /// </summary>
        public int X { get { return Room.Left + RelativeX; } }

        /// <summary>
        /// Vertical position of the tile relative to the level origin.
        /// </summary>
        public int Y { get { return Room.Top + RelativeY; } }

        /// <summary>
        /// Gets or sets the solidity state of the tile.
        /// </summary>
        public TileState State
        {
            get { return _state; }
            set
            {
                if (value == TileState.Void && _entities != null) {
                    RemoveEntities();
                }

                _state = value;
            }
        }

        /// <summary>
        /// Gets the set of entities currently on this tile.
        /// </summary>
        public IEnumerable<Entity> Entities { get { return _entities; } }

        /// <summary>
        /// Gets the number of entities currently on this tile.
        /// </summary>
        public int EntityCount { get { return _entities.Count; } }

        /// <summary>
        /// Gets the appearance of this tile that should be sent to clients.
        /// </summary>
        public TileAppearance Appearance
        {
            get
            {
                if (_lastAppearance < Room.Level.Time) {
                    UpdateAppearance();
                }

                return _appearance;
            }
        }
        
        /// <summary>
        /// Constructs a new tile instance within the specified room
        /// and at the given position.
        /// </summary>
        /// <param name="room">Room containing the tile.</param>
        /// <param name="relPos">Position of the tile relative to
        /// the containing room.</param>
        internal Tile(Room room, Position relPos)
        {
            Room = room;
            RelativePosition = relPos;

            State = TileState.Void;

            _entities = new List<Entity>();
            _appearance = new TileAppearance(RelativePosition);
        }

        /// <summary>
        /// Constructs a new tile instance within the specified room
        /// and at the given position.
        /// </summary>
        /// <param name="room">Room containing the tile.</param>
        /// <param name="relX">Horizontal position of the tile relative
        /// to the containing room.</param>
        /// <param name="relY">Vertical position of the tile relative
        /// to the containing room.</param>
        internal Tile(Room room, int relX, int relY)
            : this(room, new Position(relX, relY)) { }

        /// <summary>
        /// Adds an entity to the tile.
        /// </summary>
        /// <param name="ent">Entity to add to the tile.</param>
        internal void AddEntity(Entity ent)
        {
            if (State == TileState.Void) return;

            if (ent.Tile != this) return;
            if (_entities.Contains(ent)) return;

            _entities.Add(ent);
        }

        /// <summary>
        /// Removes an entity from the tile.
        /// </summary>
        /// <param name="ent">Entity to remove.</param>
        internal void RemoveEntity(Entity ent)
        {
            if (ent.Tile == this) return;
            if (!_entities.Contains(ent)) return;

            _entities.Remove(ent);
        }

        /// <summary>
        /// Removes all entities from this tile.
        /// </summary>
        public void RemoveEntities()
        {
            while (_entities.Count > 0) {
                _entities.Last().Remove();
            }
        }

        /// <summary>
        /// Gets a neighbouring tile at the given position
        /// relative to this tile.
        /// </summary>
        /// <param name="offset">Relative position of the
        /// tile to get.</param>
        /// <returns>The neighbouring tile.</returns>
        public Tile GetNeighbour(Position offset)
        {
            return Room[RelativePosition + offset];
        }

        /// <summary>
        /// Gets a neighbouring tile one unit in the
        /// given direction.
        /// </summary>
        /// <param name="dir">Direction of the tile to get.</param>
        /// <returns>The neighbouring tile.</returns>
        public Tile GetNeighbour(Direction dir)
        {
            return GetNeighbour(dir.GetOffset());
        }

        /// <summary>
        /// Instructs each entity within the tile to perform a single think step.
        /// </summary>
        public void Think()
        {
            for (int i = _entities.Count - 1; i >= 0; --i) {
                // In case more than one entity was removed from the tile
                if (i >= _entities.Count) continue;

                _entities[i].Think();
            }
        }

        /// <summary>
        /// Tests to see if an unbroken line of sight exists from the
        /// specified position to this tile within the given radius.
        /// </summary>
        /// <param name="pos">Position to perform a visibility test to.</param>
        /// <param name="maxRadius">Maximum visible distance.</param>
        /// <returns>True if the tile is visible from the given position,
        /// and false otherwise.</returns>
        public bool IsVisibleFrom(Position pos, int maxRadius)
        {
            var diff = Position - pos;

            if (diff.LengthSquared > maxRadius * maxRadius) return false;

            return pos.BresenhamLine(Position)
                .All(x => x == Position || Room[x].State == TileState.Floor);
        }

        /// <summary>
        /// Updates the appearance of this tile.
        /// </summary>
        private void UpdateAppearance()
        {
            if (_lastAppearance >= Room.Level.Time) return;
            _lastAppearance = Room.Level.Time;

            switch (State) {
                case TileState.Void:
                    _appearance.Symbol = ' ';
                    _appearance.ForeColor = _appearance.BackColor = ConsoleColor.Black;
                    _appearance.Entities = _appearance.Entities.Length == 0
                        ? _appearance.Entities : new EntityAppearance[0];
                    return;
                case TileState.Wall:
                    int adj = 
                        (GetNeighbour(Direction.East).State != TileState.Floor ? 1 : 0) |
                        (GetNeighbour(Direction.South).State != TileState.Floor ? 2 : 0) |
                        (GetNeighbour(Direction.West).State != TileState.Floor ? 4 : 0) |
                        (GetNeighbour(Direction.North).State != TileState.Floor ? 8 : 0) |
                        (GetNeighbour(Direction.SouthEast).State != TileState.Floor ? 16 : 0) |
                        (GetNeighbour(Direction.SouthWest).State != TileState.Floor ? 32 : 0) |
                        (GetNeighbour(Direction.NorthWest).State != TileState.Floor ? 64 : 0) |
                        (GetNeighbour(Direction.NorthEast).State != TileState.Floor ? 128 : 0);

                    _appearance.Symbol = _sWallTiles[adj];
                    _appearance.ForeColor = ConsoleColor.Gray;
                    _appearance.BackColor = ConsoleColor.Black;
                    break;
                case TileState.Floor:
                    _appearance.Symbol = '+';
                    _appearance.ForeColor = ConsoleColor.DarkGray;
                    _appearance.BackColor = ConsoleColor.Black;
                    break;
            }

            var drawables = _entities
                .Where(x => x.HasComponent<Drawable>())
                .Select(x => x.GetComponent<Drawable>())
                .ToArray();

            if (drawables.Length > 0) {
                var layer = (DrawableLayer) drawables.Max(x => x.GetLayer());

                _appearance.Entities = drawables
                    .Where(x => x.GetLayer() == layer)
                    .Select(x => x.GetAppearance())
                    .ToArray();
            } else if (_appearance.EntityCount > 0) {
                _appearance.Entities = new EntityAppearance[0];
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through each entity on this tile.
        /// </summary>
        /// <returns>An enumerator that iterates through each entity on this tile.</returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through each entity on this tile.
        /// </summary>
        /// <returns>An enumerator that iterates through each entity on this tile.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }
    }
}
