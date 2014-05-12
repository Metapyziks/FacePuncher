using System;

namespace FacePuncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Display.Initialize(96, 32);

            for (int i = 0; i < Display.Width * Display.Height; ++i) {
                Display.SetCell(i % Display.Width, i / Display.Width, (char) i, ConsoleColor.Black, ConsoleColor.Gray);
            }

            Display.Refresh();

            Console.ReadKey();
        }
    }
}
