using System.Threading;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            Display.Initialize(96, 32);

            using (var server = new ServerConnection("localhost", 14242)) {

                var halfSize = new Position(Display.Width / 2, Display.Height / 2);

                int flash = 0;
                var renderTimer = new Timer(state => {
                    Display.Clear();

                    lock (server.Level) {
                        if (server.Player != null) {
                            var attribs = new DrawAttributes(server.Time, flash++);
                            var rect = Display.Rect + server.Player.Position - halfSize;

                            foreach (var vis in server.Visibility) {
                                vis.Draw(rect, Position.Zero, attribs);
                            }
                        }
                    }

                    Display.Refresh();
                }, null, 0, 125);

                server.ProcessPackets();
            }
        }
    }
}
