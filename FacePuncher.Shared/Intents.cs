using FacePuncher.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher
{
    [Serializable]
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

    [Serializable]
    public class MoveIntent : Intent
    {
        public Direction Direction { get; set; }
        public MoveIntent(Direction direction)
        { Direction = direction; }
    }
}
