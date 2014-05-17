using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FacePuncher.Geometry;
using FacePuncher.Network;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FacePuncher.Graphics
{
    public class TileAppearance : IEnumerable<EntityAppearance>
    {
        public Position Position { get; private set; }

        public char Symbol { get; set; }

        public ConsoleColor ForeColor { get; set; }

        public ConsoleColor BackColor { get; set; }

        public EntityAppearance[] Entities { get; set; }

        public int EntityCount { get { return Entities.Length; } }
        
        public EntityAppearance this[int index]
        {
            get { return Entities[index]; }
        }

        public TileAppearance(Position pos)
        {
            Position = pos;

            Symbol = '?';
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Entities = new EntityAppearance[0];
        }

        public static async Task<TileAppearance> Read(Position pos, NetworkStream stream)
        {
            var appearance = await stream.ReadAppearance();

            var entities = new EntityAppearance[await stream.ReadByteAsync()];
            for (int i = 0; i < entities.Length; ++i) {
                entities[i] = await EntityAppearance.Read(stream);
            }

            return new TileAppearance(pos)
            {
                //TODO: this is a bit hacky
                Symbol = appearance.Item1,
                ForeColor = appearance.Item2,
                BackColor = appearance.Item3,
                Entities = entities
            };
        }

        public void WriteToStream(Stream stream)
        {
            if (EntityCount >= 256) {
                throw new Exception("Entity count on one tile larger than 255. If this "
                    + "ever happens naturally use something larger than a byte to send "
                    + "entity counts in TileAppearance.");
            }

            stream.WriteAppearance(Symbol, ForeColor, BackColor);
            stream.WriteByte((byte) EntityCount);

            foreach (var ent in this) {
                ent.WriteToStream(stream);
            }
        }

        public IEnumerator<EntityAppearance> GetEnumerator()
        {
            return Entities.AsEnumerable<EntityAppearance>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Entities.GetEnumerator();
        }
    }
}
