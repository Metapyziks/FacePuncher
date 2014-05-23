/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Saša Barišić [cartman300@net.hr]
 * Copyright (c) 2014 Michał Ferchow [deseteral@gmail.com]
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
            : base(name, Interface.Display.Center - new Position(Size.X / 2, Size.Y / 2),
                Size.X, Size.Y, "Connect to Server")
        {
            var lblHostName = new Label("lbl_hostname", new Position(2, 2), "Host name", ConsoleColor.DarkGray);
            var txtHostName = new TextBox("txt_hostname", new Position(3, 4), 28, "localhost");

            var lblPort = new Label("lbl_port", new Position(2, 6), "Port number", ConsoleColor.DarkGray);
            var txtPort = new TextBox("txt_port", new Position(3, 8), 5, "14242");

            var btnConnect = new Button("btn_connect", new Position(3, 10), "Connect");
            var btnQuit = new Button("btn_quit", new Position(btnConnect.Text.Length + 19, 10), "Quit", ConsoleColor.DarkRed);

            btnConnect.Used += (sender, e) => {
                int port;
                if (!int.TryParse(txtPort.Text, out port) || port >= ushort.MaxValue) return;
                Connect(this, new AttemptConnectEventArgs(txtHostName.Text, port));
            };

            btnQuit.Used += (sender, e) => {
                Environment.Exit(0);
            };

            AddChild(lblHostName);
            AddChild(txtHostName);

            AddChild(lblPort);
            AddChild(txtPort);

            AddChild(btnConnect);
            AddChild(btnQuit);
        }
    }
}
