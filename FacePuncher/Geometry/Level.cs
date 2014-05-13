using System.Collections.Generic;

namespace FacePuncher.Geometry
{
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
    }
}
