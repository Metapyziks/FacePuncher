using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// A class representing a game level as a collection of rooms.
    /// </summary>
    class Level : IEnumerable<Room>
    {
        private List<Room> _rooms;

        /// <summary>
        /// Constructs an empty level.
        /// </summary>
        public Level()
        {
            _rooms = new List<Room>();
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }
    }
}
