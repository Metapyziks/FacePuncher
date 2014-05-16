using System;
using System.Collections.Generic;
using System.Linq;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Records the visibility of tiles within a room.
    /// </summary>
    public class RoomVisibility
    {
        private ulong[,] _mask;

        /// <summary>
        /// Room this instance is recording the visibility of.
        /// </summary>
        public Room Room { get; private set; }

        /// <summary>
        /// Gets the latest time any tile within the room was seen.
        /// </summary>
        public ulong LastVisibleTime { get; private set; }

        /// <summary>
        /// Creates a new RoomVisibility instance to track the visibility
        /// of tiles within a specified room.
        /// </summary>
        /// <param name="room">Room to track the visibility of.</param>
        public RoomVisibility(Room room)
        {
            Room = room;
            LastVisibleTime = 0;

            _mask = new ulong[room.Width, room.Height];
        }
        
        /// <summary>
        /// Marks a specific tile within the room as being visible
        /// at the given time.
        /// </summary>
        /// <param name="pos">Position of the tile to mark as visible.</param>
        /// <param name="time">Time at which the tile was seen.</param>
        public void Reveal(Position pos, ulong time)
        {
            _mask[pos.X, pos.Y] = Math.Max(_mask[pos.X, pos.Y], time);
            LastVisibleTime = Math.Max(LastVisibleTime, time);
        }

        /// <summary>
        /// Updates the visibility of each tile within the room from a specified
        /// origin of sight, with a given maximum view radius.
        /// </summary>
        /// <param name="origin">Position of the observer.</param>
        /// <param name="maxRadius">Maximum view radius.</param>
        /// <param name="time">Time at which visibility is being tested.</param>
        /// <returns>True if any tiles are currently visible, and false otherwise.</returns>
        public bool UpdateVisibility(Position origin, int maxRadius, ulong time)
        {
            if ((Room.Rect.NearestPosition(origin) - origin).LengthSquared > maxRadius * maxRadius) {
                return false;
            }

            foreach (var pos in Room.Rect.Positions) {
                if ((pos - origin).LengthSquared > maxRadius * maxRadius) continue;

                var rel = pos - Room.Rect.TopLeft;

                if (_mask[rel.X, rel.Y] >= time) continue;

                foreach (var mid in origin.BresenhamLine(pos)) {
                    rel = mid - Room.Rect.TopLeft;

                    if (Room.RelativeRect.Intersects(rel)) {
                        Reveal(rel, time);
                    }

                    if (Room[rel].State != TileState.Floor) break;
                }
            }

            return LastVisibleTime >= time;
        }

        /// <summary>
        /// Gets a set of all tiles within the room that were visible at the
        /// specified time.
        /// </summary>
        /// <param name="time">Time at which to compare visibility.</param>
        /// <returns>A set of visible tiles.</returns>
        public IEnumerable<Tile> GetVisible(ulong time)
        {
            if (LastVisibleTime < time) return Enumerable.Empty<Tile>();

            return Room.RelativeRect.Positions
                .Where(p => _mask[p.X, p.Y] >= time)
                .Select(p => Room[p]);
        }

        /// <summary>
        /// Gets a set of all tiles within the room that were not visible at
        /// the specified time but have been seen in the past.
        /// </summary>
        /// <param name="time">Time at which to compare visibility.</param>
        /// <returns>A set of previously seen tiles.</returns>
        public IEnumerable<Tile> GetRemembered(ulong time)
        {
            return Room.RelativeRect.Positions
                .Where(p => _mask[p.X, p.Y] != 0 && _mask[p.X, p.Y] < time)
                .Select(p => Room[p]);
        }
    }
}
