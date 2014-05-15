using System.Threading;

using FacePuncher.Geometry;

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

            using (var server = new ServerConnection("localhost", 14242)) {
                int flash = 0;

                var renderTimer = new Timer(state => {
                    if (server.Player == null) return;

                    Display.Clear();

                    lock (server.Level) {
                        var attribs = new DrawAttributes(flash++);
                        var rect = Display.Rect + server.Player.Position - Display.Center;

                        foreach (var vis in server.Visibility) {
                            vis.Draw(rect, Position.Zero, attribs);
                        }
                    }

                    Display.Refresh();
                }, null, 0, RenderPeriod);

                while (server.ProcessPacket());
            }
        }
    }
}
