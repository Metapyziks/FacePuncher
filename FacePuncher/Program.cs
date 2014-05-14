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
            // Load entity and other definitions from the data directory.
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            // Prepare the console window for rendering.
            Display.Initialize(96, 32);

            // Connect to a server.
            using (var server = new ServerConnection("localhost", 14242)) {
                int flash = 0;

                // Start the rendering timer.
                var renderTimer = new Timer(state => {
                    if (server.Player == null) return;

                    // Clear the render buffer.
                    Display.Clear();

                    lock (server.Level) {
                        var attribs = new DrawAttributes(server.Time, flash++);
                        var rect = Display.Rect + server.Player.Position - Display.Center;

                        // Draw from each room visibility mask.
                        foreach (var vis in server.Visibility) {
                            vis.Draw(rect, Position.Zero, attribs);
                        }
                    }

                    // Send the render buffer to the console window.
                    Display.Refresh();
                }, null, 0, RenderPeriod);

                // Receive and process server communications.
                while (server.ProcessPacket());
            }
        }
    }
}
