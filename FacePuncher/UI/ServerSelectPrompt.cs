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

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher.UI
{
    class AttemptConnectEventArgs : EventArgs
    {
        public String HostName { get; private set; }

        public int Port { get; private set; }

        public AttemptConnectEventArgs(String hostName, int port)
        {
            HostName = hostName;
            Port = port;
        }
    }

    class ServerSelectPrompt : Frame
    {
        private static readonly Position Size = new Position(32, 12);

        public event EventHandler<AttemptConnectEventArgs> Connect;

        public ServerSelectPrompt(String name)
            : base(name, Display.Center - new Position(Size.X / 2, Size.Y / 2),
                Size.X, Size.Y, "Connect to Server")
        {
            var lblHostName = new Label("lbl_hostname", Position + new Position(2, 2), "Host name", ConsoleColor.DarkGray);
            var txtHostName = new TextBox("txt_hostname", Position + new Position(3, 4), 28, "localhost");

            var lblPort = new Label("lbl_port", Position + new Position(2, 6), "Port number", ConsoleColor.DarkGray);
            var txtPort = new TextBox("txt_port", Position + new Position(3, 8), 5, "14242");

            var btnConnect = new Button("btn_connect", Position + new Position(3, 10), "Connect");

            btnConnect.Used += (sender, e) => {
                int port;
                if (!int.TryParse(txtPort.Text, out port) || port >= ushort.MaxValue) return;
                Connect(this, new AttemptConnectEventArgs(txtHostName.Text, port));
            };

            AddChild(lblHostName);
            AddChild(txtHostName);

            AddChild(lblPort);
            AddChild(txtPort);

            AddChild(btnConnect);
        }
    }
}
