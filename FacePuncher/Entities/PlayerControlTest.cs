using System;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    class PlayerControlTest : Component
    {
        public override void OnThink(ulong time)
        {
            var direc = Direction.None;

            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.NumPad7:
                    direc = Direction.NorthWest; break;
                case ConsoleKey.NumPad8:
                case ConsoleKey.UpArrow:
                    direc = Direction.North; break;
                case ConsoleKey.NumPad9:
                    direc = Direction.NorthEast; break;
                case ConsoleKey.NumPad4:
                case ConsoleKey.LeftArrow:
                    direc = Direction.West; break;
                case ConsoleKey.NumPad6:
                case ConsoleKey.RightArrow:
                    direc = Direction.East; break;
                case ConsoleKey.NumPad1:
                    direc = Direction.SouthWest; break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.DownArrow:
                    direc = Direction.South; break;
                case ConsoleKey.NumPad3:
                    direc = Direction.SouthEast; break;
            }

            Entity.Move(Tile.GetNeighbour(direc));
        }
    }
}
