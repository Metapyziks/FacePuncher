using FacePuncher.Geometry;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher
{
    [ProtoContract]
    [ProtoInclude(1, typeof(MoveIntent))]
    public abstract class Intent
    {
        public static void HandleIntent<THandled>(ref Intent intent, Func<THandled, bool> handler) where THandled : Intent
        {
            var castIntent = intent as THandled;
            if (castIntent != null)
            {
                if (handler(castIntent))
                { intent = null; }
            }
        }
    }

    [ProtoContract]
    public class MoveIntent : Intent
    {
        [ProtoMember(1)]
        public Direction Direction { get; set; }
        public MoveIntent() : this(Direction.None) { }
        public MoveIntent(Direction direction)
        { Direction = direction; }
    }
}
