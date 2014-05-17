namespace FacePuncher.Geometry
{
    /// <summary>
    /// Enumeration of each of the eight
    /// directions an entity may move to
    /// a neighbouring tile.
    /// </summary>
    /// <remarks>
    /// Values are organised like this:
    /// 
    /// 0 | 1 | 2
    /// 3 | 4 | 5
    /// 6 | 7 | 8
    /// </remarks>
    public enum Direction
    {
        NorthWest = 0,
        North = 1,
        NorthEast = 2,
        West = 3,
        None = 4,
        East = 5,
        SouthWest = 6,
        South = 7,
        SouthEast = 8
    }
}
