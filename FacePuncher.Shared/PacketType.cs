namespace FacePuncher
{
    /// <summary>
    /// Used to specify the nature of the following packet
    /// during communication between the server and client.
    /// </summary>
    public enum PacketType : byte
    {
        LevelState = 1,
        InputRequest = 2
    }
}
