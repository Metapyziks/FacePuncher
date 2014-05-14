using System.Linq;

namespace FacePuncher.Entities
{
    class VerminControl : AgentControl
    {
        public override void OnThink(ulong time)
        {
            if (!CanMove(time) || Tools.Random.NextDouble() < 0.5) return;

            var valid = Tools.Directions.Where(x => Entity.CanMove(x)).ToArray();

            Move(valid[Tools.Random.Next(valid.Length)], time);
        }
    }
}
