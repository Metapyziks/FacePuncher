/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

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
