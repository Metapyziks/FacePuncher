using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher.Graphics
{
    public class TileAppearance : IEnumerable<EntityAppearance>
    {
        private EntityAppearance[] _ents;

        public char Symbol { get; set; }

        public ConsoleColor ForeColor { get; set; }

        public ConsoleColor BackColor { get; set; }

        public EntityAppearance[] Entities { get; set; }

        public int EntityCount { get { return Entities.Length; } }
        
        public TileAppearance(char symbol, ConsoleColor foreColor, ConsoleColor backColor, EntityAppearance[] ents)
        {
            Symbol = symbol;
            ForeColor = foreColor;
            BackColor = backColor;
            Entities = ents;
        }

        public TileAppearance(Stream stream)
        {
            char symbol; ConsoleColor foreColor, backColor;

            stream.ReadAppearance(out symbol, out foreColor, out backColor);

            Symbol = symbol;
            ForeColor = foreColor;
            BackColor = backColor;

            Entities = new EntityAppearance[stream.ReadByte()];
            for (int i = 0; i < EntityCount; ++i) {
                Entities[i] = new EntityAppearance(stream);
            }
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
            return _ents.AsEnumerable<EntityAppearance>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _ents.GetEnumerator();
        }
    }
}
