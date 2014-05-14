using System.Linq;

namespace FacePuncher.Entities
{
    class Vermin : Component
    {
        private ulong _nextMove;

        public ulong MovePeriod { get; private set; }

        public override void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            if (elem.HasElement("MovePeriod")) {
                MovePeriod = elem.Element<ulong>("MovePeriod");
            }
        }

        public override void OnThink(ulong time)
        {
            if (MovePeriod > 0 && time >= _nextMove) {
                _nextMove += MovePeriod;

                if (Tools.Random.NextDouble() < 0.5) return;

                var valid = Tools.Directions.Where(x => Entity.CanMove(x)).ToArray();
                Entity.Move(valid[Tools.Random.Next(valid.Length)]);
            }
        }
    }
}
