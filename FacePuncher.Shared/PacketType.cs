namespace FacePuncher
{
    /// <summary>
    /// Used to specify the nature of the following packet
    /// during communication between the server and client.
    /// </summary>
    public enum ServerPacketType : byte
    {
        LevelState = 1
    }

    /// <summary>
    /// Used to specify the nature of the following packet
    /// during communication between the client and server.
    /// </summary>
    public enum ClientPacketType : byte
    {
        PlayerIntent = 1
    }
}
