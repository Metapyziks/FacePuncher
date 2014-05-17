using System.Threading;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    /// <summary>
    /// Class containing the client entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Milliseconds between redraws.
        /// </summary>
        const int RenderPeriod = 125;

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        static void Main(string[] args)
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            Display.Initialize(96, 32);

            Thread.Sleep(2000);

            using (var server = new ServerConnection("localhost", 14242)) {
                int flash = 0;

                var renderTimer = new Timer(state => {
                    if (!server.LoadedLevel) return;

                    Display.Clear();

                    lock (server.Visibility) {
                        var attribs = new DrawAttributes(flash++);
                        var rect = Display.Rect + server.PlayerPosition - Display.Center;

                        foreach (var vis in server.Visibility) {
                            vis.Draw(rect, Position.Zero, attribs, server.Time);
                        }
                    }

                    Display.Refresh();
                }, null, 0, RenderPeriod);

                while (server.ProcessPacket());
            }
        }
    }
}
