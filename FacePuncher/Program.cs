using FacePuncher.Geometry;
using System.Threading.Tasks;
using System;

namespace FacePuncher
{
    /// <summary>
    /// Class containing the client entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Milliseconds between redraws.
        /// </summary>
        const int RenderPeriod = 125;

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        public static void Main(string[] args)
        {
            TaskMain().Wait();
        }
        static async Task TaskMain()
        {
            // TODO: Use a sane non-development specific path.
            Definitions.LoadFromDirectory("../../../Data", DefinitionsNamespace.Client);

            Display.Initialize(96, 32);

            var server = new ServerConnection("localhost", 14242);
            server.Run();

            while (true)
            {
                var player = server.Player;
                if (player == null)
                {
                    Console.WriteLine("Connecting...");
                    await Task.Delay(100);
                    continue;
                }
                var key = Console.ReadKey(intercept: true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        server.SendIntent(new MoveIntent(Direction.North));
                        break;
                    case ConsoleKey.DownArrow:
                        server.SendIntent(new MoveIntent(Direction.South));
                        break;
                    case ConsoleKey.LeftArrow:
                        server.SendIntent(new MoveIntent(Direction.West));
                        break;
                    case ConsoleKey.RightArrow:
                        server.SendIntent(new MoveIntent(Direction.East));
                        break;
                    default:
                        break;
                }
            }
        }

        static int _flash = 0;
        internal static void Draw(ServerConnection server)
        {
            Display.Clear();

            // removed Level lock
            var attribs = new DrawAttributes(server.Time, _flash++);
            var rect = Display.Rect + server.Player.Position - Display.Center;

            foreach (var vis in server.Visibility)
            {
                vis.Draw(rect, Position.Zero, attribs);
            }

            Display.Refresh();
        }
    }
}